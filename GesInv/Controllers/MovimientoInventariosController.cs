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
    public class MovimientoInventariosController : Controller
    {
        private readonly GestionInvContext _context;

        public MovimientoInventariosController(GestionInvContext context)
        {
            _context = context;
        }

        // GET: MovimientoInventarios
        public async Task<IActionResult> Index()
        {
            var gestionInvContext = _context.MovimientoInventarios.Include(m => m.Producto);
            return View(await gestionInvContext.ToListAsync());
        }

        // GET: MovimientoInventarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimientoInventario = await _context.MovimientoInventarios
                .Include(m => m.Producto)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
            if (movimientoInventario == null)
            {
                return NotFound();
            }

            return View(movimientoInventario);
        }

        // GET: MovimientoInventarios/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId");
            return View();
        }

        // POST: MovimientoInventarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovimientoId,ProductoId,Cantidad,FechaMovimiento,TipoMovimiento,Notas")] MovimientoInventario movimientoInventario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movimientoInventario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", movimientoInventario.ProductoId);
            return View(movimientoInventario);
        }

        // GET: MovimientoInventarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimientoInventario = await _context.MovimientoInventarios.FindAsync(id);
            if (movimientoInventario == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", movimientoInventario.ProductoId);
            return View(movimientoInventario);
        }

        // POST: MovimientoInventarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovimientoId,ProductoId,Cantidad,FechaMovimiento,TipoMovimiento,Notas")] MovimientoInventario movimientoInventario)
        {
            if (id != movimientoInventario.MovimientoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movimientoInventario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovimientoInventarioExists(movimientoInventario.MovimientoId))
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", movimientoInventario.ProductoId);
            return View(movimientoInventario);
        }

        // GET: MovimientoInventarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimientoInventario = await _context.MovimientoInventarios
                .Include(m => m.Producto)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
            if (movimientoInventario == null)
            {
                return NotFound();
            }

            return View(movimientoInventario);
        }

        // POST: MovimientoInventarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Obtener y eliminar los movimientos relacionados con el producto
            var movimientos = _context.MovimientoInventarios.Where(m => m.ProductoId == id);
            _context.MovimientoInventarios.RemoveRange(movimientos);

            // Obtener y eliminar el producto
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MovimientoInventarioExists(int id)
        {
            return _context.MovimientoInventarios.Any(e => e.MovimientoId == id);
        }
    }
}
