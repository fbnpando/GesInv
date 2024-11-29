using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GesInv.Models;
using GesInv.Data;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml;

using iText.Commons.Actions.Contexts;
namespace GesInv.Controllers
{
    public class ProductoesController : Controller
    {
        private readonly GestionInvContext _context;

        public ProductoesController(GestionInvContext context)
        {
            _context = context;

        }

        // GET: Productoes
        public async Task<IActionResult> Index()
        {
            var gestionInvContext = _context.Productos.Include(p => p.Categoria).Include(p => p.Proveedor);
            return View(await gestionInvContext.ToListAsync());
        }

        public IActionResult ExportToExcel()
        {
            // Establecer el contexto de la licencia
            ExcelPackage.LicenseContext = LicenseContext.Commercial; // O LicenseContext.NonCommercial si no tienes licencia comercial

            var productos = _context.Productos.Include(p => p.Categoria).Include(p => p.Proveedor).ToList();

            // Crear un libro de trabajo (workbook)
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Productos");

                // Llenar los encabezados de la tabla
                worksheet.Cells[1, 1].Value = "Imagen";
                worksheet.Cells[1, 2].Value = "Producto";
                worksheet.Cells[1, 3].Value = "Descripción";
                worksheet.Cells[1, 4].Value = "Precio";
                worksheet.Cells[1, 5].Value = "Stock";
                worksheet.Cells[1, 6].Value = "Categoría";
                worksheet.Cells[1, 7].Value = "Proveedor";

                int row = 2;

                foreach (var producto in productos)
                {
                    // Si el producto tiene una imagen asociada, agregarla en la celda
                    if (!string.IsNullOrEmpty(producto.Imagen))
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", producto.Imagen.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            using (var imageStream = new FileStream(imagePath, FileMode.Open))
                            {
                                // Insertar imagen en la celda
                                var image = worksheet.Drawings.AddPicture($"Imagen_{row}", imageStream);
                                image.SetSize(50, 50); // Ajustar tamaño de la imagen
                                image.SetPosition(row - 1, 0, 0, 0); // Posicionar imagen en la celda
                            }
                        }
                    }

                    worksheet.Cells[row, 2].Value = producto.Nombre;
                    worksheet.Cells[row, 3].Value = producto.Descripcion;
                    worksheet.Cells[row, 4].Value = producto.Precio;
                    worksheet.Cells[row, 5].Value = producto.Stock;
                    worksheet.Cells[row, 6].Value = producto.Categoria?.Nombre;
                    worksheet.Cells[row, 7].Value = producto.Proveedor?.Nombre;

                    row++;
                }

                // Guardar el archivo en un MemoryStream y enviarlo como archivo de descarga
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos_con_imagenes.xlsx");
            }
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.ProductoId == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // GET: Productoes/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre");
            ViewData["ProveedorId"] = new SelectList(_context.Proveedors, "ProveedorId", "Nombre");
            return View();
        }

        // POST: Productoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoId,Nombre,Descripcion,Precio,Stock,CategoriaId,ProveedorId")] Producto producto, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    // Verificar que el directorio wwwroot/images existe
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    // Crear un nombre único para la imagen
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);

                    // Guardar la imagen en wwwroot/images
                    var filePath = Path.Combine(imagePath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(stream);
                    }

                    // Guardar la ruta relativa del archivo en el modelo
                    producto.Imagen = "/images/" + fileName;
                }

                // Agregar el producto a la base de datos
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar los datos necesarios
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedors, "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedors, "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // POST: Productoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,Nombre,Descripcion,Precio,Stock,CategoriaId,ProveedorId")] Producto producto)
        {
            if (id != producto.ProductoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            ViewData["ProveedorId"] = new SelectList(_context.Proveedors, "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.ProductoId == id);

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movimientos = _context.MovimientoInventarios.Where(m => m.ProductoId == id).ToList();

            if (movimientos.Any())
                _context.MovimientoInventarios.RemoveRange(movimientos);

            var producto = await _context.Productos.FindAsync(id);

            if (producto != null)
                _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }

        // GET: api/Productoes
        [HttpGet("/api/productos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosApi()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .ToListAsync();
        }

        // Exportar Factura en PDF
        public IActionResult ExportFactura(int id)
        {
            var producto = _context.Productos.Include(p => p.Categoria).Include(p => p.Proveedor)
                .FirstOrDefault(p => p.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Configurar fuentes y colores
                XFont titleFont = new XFont("Arial", 20, XFontStyle.Bold);
                XFont subtitleFont = new XFont("Arial", 14, XFontStyle.Regular);
                XFont contentFont = new XFont("Arial", 12, XFontStyle.Regular);
                XBrush headerBrush = new XSolidBrush(XColor.FromArgb(0xFF7986CB)); // Azul oscuro
                XBrush bodyBrush = new XSolidBrush(XColor.FromArgb(0xFFE8EAF6)); // Azul claro

                // Cargar el logotipo
                XImage logoImage = XImage.FromFile("wwwroot/images/logo.png");

                // Dibujar el encabezado
                gfx.DrawRectangle(headerBrush, 0, 0, page.Width, 150);
                gfx.DrawString("FACTURA", titleFont, XBrushes.White, new XPoint(50, 50));
                gfx.DrawString("GestionInventario Inc.", subtitleFont, XBrushes.White, new XPoint(50, 90));
                gfx.DrawString("Carretera Muelle 38, 37531 Avila, Avila", contentFont, XBrushes.White, new XPoint(50, 120));
                gfx.DrawImage(logoImage, 500, 20, 100, 50); // Dibujar el logotipo

                // Dibujar la información del producto
                gfx.DrawRectangle(bodyBrush, 50, 200, page.Width - 100, 250);
                DrawWatermark(gfx, page);

                // Aquí se obtiene el nombre y demás detalles del producto usando la instancia "producto"
                gfx.DrawString($"Producto: {producto.Nombre}", subtitleFont, XBrushes.Black, new XPoint(100, 230));
                gfx.DrawString($"Descripción: {producto.Descripcion}", contentFont, XBrushes.Black, new XPoint(100, 260));
                gfx.DrawString($"Precio: ${producto.Precio.ToString("F2")}", contentFont, XBrushes.Black, new XPoint(100, 290));
                gfx.DrawString($"Stock: {producto.Stock}", contentFont, XBrushes.Black, new XPoint(100, 320));

                // Dibujar el pie de página
                XFont footerFont = new XFont("Arial", 10, XFontStyle.Italic);
                gfx.DrawString("Gracias por su compra.", footerFont, XBrushes.Black, new XPoint(50, page.Height - 50));

                // Dibujar la marca de agua
                document.Save(memoryStream, false);
                memoryStream.Position = 0;
                return File(memoryStream.ToArray(), "application/pdf", "Factura_" + producto.Nombre + ".pdf");
            }
        }

        private void DrawWatermark(XGraphics gfx, PdfPage page)
        {
            string watermarkText = "GestionInventario";
            XFont watermarkFont = new XFont("Arial", 80, XFontStyle.Bold);
            XBrush watermarkBrush = new XSolidBrush(XColor.FromArgb(0xFFD0D0D0)); // Color gris claro

            double angle = -45;
            gfx.RotateTransform(angle);

            // Establecer la posición de la marca de agua
            gfx.DrawString(watermarkText, watermarkFont, watermarkBrush, new XPoint(-350, page.Height - 350)); // Ajusta el valor según sea necesario

            gfx.RotateTransform(-angle);
        }

        // GET: api/Productoes/5
        [HttpGet("/api/productos/{id}")]
        public async Task<ActionResult<Producto>> GetProductoApi(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null)
                return NotFound();

            return producto;
        }

    }
}
