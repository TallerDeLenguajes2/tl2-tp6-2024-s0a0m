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
    readonly ProductoRepository productoR;

    public PresupuestoController(ILogger<PresupuestoController> logger)
    {
        _logger = logger;
        presupuestoR = new();
        productoR = new();
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

    [HttpPost("CrearPresupuesto")]
    public IActionResult CrearPresupuesto([FromForm] Presupuesto presupuesto)
    {
        presupuestoR.CrearPresupuesto(presupuesto);
        // hacer un metodo en el repositorio de presupuesto que reciba un objecto presupuesto con los detalles y cree todo junto
        var idPresupuesto = presupuestoR.ListarPresupuestos().MaxBy(p => p.IdPresupuesto).IdPresupuesto;
        for (int i = 0; i < presupuesto.Detalle.Count; i++)
        {
            presupuestoR.AgregarProductoYCantidad(idPresupuesto, presupuesto.Detalle[i].Producto.IdProducto, presupuesto.Detalle[i].Cantidad);
        }
        return RedirectToAction("Index", "Presupuesto");
    }

    [HttpGet("ModificarPresupuesto/{id}")]
    public IActionResult ModificarPresupuesto(int id)
    {
        var presupuesto = presupuestoR.ObtenerPresupuesto(id);
        return presupuesto.IdPresupuesto == 0 ? RedirectToAction("Index", "Presupuesto") : View(presupuesto);
    }

    [HttpPost("ModificarPresupuesto/{id}")]
    public IActionResult ModificarPresupuesto([FromForm] Presupuesto presupuesto, int id)
    {
        presupuestoR.ModificarProducto(id, presupuesto);
        return RedirectToAction("Index", "Presupuesto");
    }

    [HttpPost("EliminarPresupuesto/{id}")]
    public IActionResult EliminarPresupuesto(int id)
    {
        presupuestoR.EliminarPresupuesto(id);
        return RedirectToAction("Index", "Presupuesto");
    }

    [HttpGet("AgregarPresupuestoDetalle")]
    public IActionResult AgregarPresupuestoDetalle(int index)
    {
        var detalle = new PresupuestoDetalle();
        ViewData["Productos"] = productoR.ListarProductos(); 

        ViewBag.Index = index; 
        return PartialView("_PresupuestoDetalleForm", detalle);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
