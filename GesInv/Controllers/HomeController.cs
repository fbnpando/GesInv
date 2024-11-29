using GesInv.Data;
using GesInv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GesInv.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GestionInvContext _context; 

        public HomeController(ILogger<HomeController> logger, GestionInvContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            // Total de productos
            var totalProductos = await _context.Productos.CountAsync();

            // Valor del inventario (suma de precio * stock)
            var valorInventario = await _context.Productos.SumAsync(p => p.Precio * p.Stock);

            // Productos de bajo stock (ejemplo: menor a 10 unidades)
            var bajoStock = await _context.Productos.CountAsync(p => p.Stock < 10);

            // Simular órdenes pendientes
            var ordenesPendientes = 45; // Esto puede venir de otra tabla relacionada si existe

            // Datos para los gráficos
            var ventasMensuales = new List<int> { 120, 150, 300, 200, 400, 180 }; // Simulación
            var ingresosPorCategoria = await _context.Productos
                .GroupBy(p => p.Categoria.Nombre)
                .Select(g => new { Categoria = g.Key, Ingresos = g.Sum(p => p.Precio * p.Stock) })
                .ToListAsync();

            var distribucionVentas = new List<int> { 40, 25, 20, 15 }; // Simulación
            var rendimientoPorTienda = new List<List<int>>
            {
                new List<int> { 65, 59, 90, 81, 56, 55 }, // Tienda A
                new List<int> { 28, 48, 40, 19, 96, 27 }  // Tienda B
            };

            // Pasar datos a la vista
            ViewBag.TotalProductos = totalProductos;
            ViewBag.ValorInventario = valorInventario;
            ViewBag.BajoStock = bajoStock;
            ViewBag.OrdenesPendientes = ordenesPendientes;
            ViewBag.VentasMensuales = ventasMensuales;
            ViewBag.IngresosPorCategoria = ingresosPorCategoria;
            ViewBag.DistribucionVentas = distribucionVentas;
            ViewBag.RendimientoPorTienda = rendimientoPorTienda;

            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
