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
        // Agrupar detalles por producto y sumar las cantidades
        var detallesAgrupados = presupuesto.Detalle
            .GroupBy(d => d.Producto.IdProducto)
            .Select(g => new PresupuestoDetalle
            {
                Producto = new Producto { IdProducto = g.Key },
                Cantidad = g.Sum(d => d.Cantidad)
            })
            .ToList();

        // Reemplazar los detalles originales con los agrupados
        presupuesto.Detalle = detallesAgrupados;

        // Crear el presupuesto en la base de datos
        presupuestoR.CrearPresupuesto(presupuesto);

        // Obtener el Id del presupuesto recién creado
        var idPresupuesto = presupuestoR.ListarPresupuestos()
                                         .MaxBy(p => p.IdPresupuesto)
                                         .IdPresupuesto;

        // Agregar los detalles del presupuesto
        foreach (var detalle in presupuesto.Detalle)
        {
            presupuestoR.AgregarProductoYCantidad(idPresupuesto, detalle.Producto.IdProducto, detalle.Cantidad);
        }

        return RedirectToAction("Index", "Presupuesto");
    }


    [HttpGet("ModificarPresupuesto/{id}")]
    public IActionResult ModificarPresupuesto(int id)
    {
        var presupuesto = presupuestoR.ObtenerPresupuesto(id);
        if (presupuesto.IdPresupuesto == 0) return RedirectToAction("Index", "Presupuesto");

        ViewData["Productos"] = productoR.ListarProductos();
        return View(presupuesto);
    }

    [HttpPost("ModificarPresupuesto/{id}")]
    public IActionResult ModificarPresupuesto([FromForm] Presupuesto presupuesto, int id)
    {
        // Obtener el presupuesto actual desde el repositorio
        var presupuestoActual = presupuestoR.ObtenerPresupuesto(id);
        if (presupuestoActual.IdPresupuesto == 0) return BadRequest();

        // Actualizar la información general del presupuesto
        presupuestoR.ModificarProducto(id, presupuesto);

        // Identificar los detalles que deben eliminarse
        var detallesAEliminar = presupuestoActual.Detalle
            .Where(detalleExistente => !presupuesto.Detalle
                .Any(detalleNuevo => detalleNuevo.Producto.IdProducto == detalleExistente.Producto.IdProducto))
            .ToList();

        foreach (var detalle in detallesAEliminar)
        {
            presupuestoR.EliminarDetalle(detalle, presupuesto.IdPresupuesto);
        }

        // Identificar y actualizar detalles modificados
        foreach (var detalleNuevo in presupuesto.Detalle)
        {
            var detalleExistente = presupuestoActual.Detalle
                .FirstOrDefault(detalle => detalle.Producto.IdProducto == detalleNuevo.Producto.IdProducto);

            if (detalleExistente != null)
            {
                // Si el detalle ya existe pero ha cambiado, actualizamos su cantidad
                if (detalleExistente.Cantidad != detalleNuevo.Cantidad)
                {
                    presupuestoR.ModificarDetalle(detalleNuevo.Producto.IdProducto, presupuesto.IdPresupuesto, detalleNuevo.Cantidad);
                }
            }
            else
            {
                // Si el detalle no existe, agregarlo como nuevo
                presupuestoR.AgregarProductoYCantidad(id, detalleNuevo.Producto.IdProducto, detalleNuevo.Cantidad);
            }
        }

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
