using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_s0a0m.Models;
using tl2_tp6_2024_s0a0m.Repositorios;

namespace tl2_tp6_2024_s0a0m.Controllers;

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
