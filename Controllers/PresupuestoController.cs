using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_s0a0m.Models;
using tl2_tp6_2024_s0a0m.Repositorios;

namespace tl2_tp6_2024_s0a0m.Controllers;

[Route("[controller]")]
public class PresupuestoController : Controller
{
    private readonly ILogger<PresupuestoController> _logger;
    readonly PresupuestosRepository presupuestoR;

    public PresupuestoController(ILogger<PresupuestoController> logger)
    {
        _logger = logger;
        presupuestoR = new();
    }
    [HttpGet]
    public IActionResult Index()
    {
        var productos = presupuestoR.ListarPresupuestos();
        return View(productos);
    }

    [HttpGet("CrearPresupuesto")]
    public IActionResult CrearPresupuesto()
    {
        return View();
    }

    [HttpPost("/CrearPresupuesto")]
    public IActionResult CrearPresupuesto([FromForm] Presupuesto presupuesto)
    {
        presupuestoR.CrearPresupuesto(presupuesto);
        return RedirectToAction("Index", "Presupuesto");
    }

    [HttpGet("ModificarPresupuesto/{id}")]
    public IActionResult ModificarPresupuesto(int id)
    {
        var presupuesto = presupuestoR.ObtenerPresupuesto(id);
        return presupuesto == default(Presupuesto) ? RedirectToAction("Index", "Presupuesto") : View(presupuesto);
    }

    [HttpPost("ModificarPresupuesto/{id}")]
    public IActionResult ModificarPresupuesto([FromForm] Producto presupuesto, int id)
    {
        // presupuestoR.ModificarProducto(id, producto);
        return RedirectToAction("Index", "Presupuesto");
    }

    [HttpPost("EliminarPresupuesto/{id}")]
    public IActionResult EliminarPresupuesto(int id)
    {
        presupuestoR.EliminarPresupuesto(id);
        return RedirectToAction("Index", "Presupuesto");
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
