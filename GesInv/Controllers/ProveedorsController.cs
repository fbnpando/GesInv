using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GesInv.Models;
using GesInv.Data;

namespace GesInv.Controllers
{
    public class ProveedorsController : Controller
    {
        private readonly GestionInvContext _context;

        public ProveedorsController(GestionInvContext context)
        {
            _context = context;
        }

        // GET: Proveedors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Proveedors.ToListAsync());
        }

        // GET: Proveedors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // GET: Proveedors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProveedorId,Nombre,NombreContacto,Telefono,CorreoElectronico,Direccion")] Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // GET: Proveedors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProveedorId,Nombre,NombreContacto,Telefono,CorreoElectronico,Direccion")] Proveedor proveedor)
        {
            if (id != proveedor.ProveedorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.ProveedorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // GET: Proveedors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Proveedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar si hay movimientos de inventario relacionados con los productos del proveedor
            var productosRelacionados = await _context.Productos
                .Where(p => p.ProveedorId == id)
                .ToListAsync();

            foreach (var producto in productosRelacionados)
            {
                bool tieneMovimientos = await _context.MovimientoInventarios
                    .AnyAsync(m => m.ProductoId == producto.ProductoId);

                if (tieneMovimientos)
                {
                    // Mostrar un mensaje de error si hay movimientos asociados
                    ModelState.AddModelError("", "No se puede eliminar el proveedor porque hay movimientos de inventario asociados a sus productos.");
                    return View(await _context.Proveedors.FirstOrDefaultAsync(p => p.ProveedorId == id));
                }
            }

            // Si no hay movimientos asociados, eliminar los productos y el proveedor
            if (productosRelacionados.Any())
            {
                _context.Productos.RemoveRange(productosRelacionados);
            }

            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedors.Remove(proveedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedors.Any(e => e.ProveedorId == id);
        }
    }
}
