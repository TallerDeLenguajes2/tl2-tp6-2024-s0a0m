using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_s0a0m.Models;
using tl2_tp6_2024_s0a0m.Repositorios;

namespace tl2_tp6_2024_s0a0m.Controllers;

[Route("[controller]")]
public class ProductoController : Controller
{
    private readonly ILogger<ProductoController> _logger;
    readonly ProductoRepository productoR;

    public ProductoController(ILogger<ProductoController> logger)
    {
        _logger = logger;
        productoR = new();
    }
    [HttpGet]
    public IActionResult Index()
    {
        var productos = productoR.ListarProductos();
        return View(productos);
    }

    [HttpGet("CrearProducto")]
    public IActionResult CrearProducto()
    {
        return View();
    }

    [HttpPost("CrearProducto")]
    public IActionResult CrearProducto([FromForm] Producto producto)
    {
        productoR.CrearProducto(producto);
        return RedirectToAction("Index", "Producto");
    }

    [HttpGet("ModificarProducto/{id}")]
    public IActionResult ModificarProducto(int id)
    {
        var producto = productoR.ObtenerPorId(id);
        return producto == default(Producto) ? RedirectToAction("Index", "Producto") : View(producto);
    }

    [HttpPost("ModificarProducto/{id}")]
    public IActionResult ModificarProducto([FromForm] Producto producto, int id)
    {
        productoR.ModificarProducto(id, producto);
        return RedirectToAction("Index", "Producto");
    }

    [HttpPost("EliminarProducto/{id}")]
    public IActionResult EliminarProducto(int id)
    {
        productoR.EliminarProducto(id);
        return RedirectToAction("Index", "Producto");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
