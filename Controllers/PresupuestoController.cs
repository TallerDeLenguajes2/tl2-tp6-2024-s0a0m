using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_s0a0m.Models;
using tl2_tp6_2024_s0a0m.ViewModels;
using tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.Filters;

namespace tl2_tp6_2024_s0a0m.Controllers;

[Route("")]
public class PresupuestoController : Controller
{
    private readonly ILogger<PresupuestoController> _logger;
    readonly IPresupuestosRepository _presupuestoR;
    readonly IClienteRepository _clienteR;
    readonly IProductoRepository _productoR;

    public PresupuestoController(ILogger<PresupuestoController> logger, IPresupuestosRepository presupuestoR, IClienteRepository clienteR, IProductoRepository productoR)
    {
        _logger = logger;
        _presupuestoR = presupuestoR;
        _productoR = productoR;
        _clienteR = clienteR;
    }

    [HttpGet]
    [AccessLevelAuthorize("Administrador", "Cliente")]
    public IActionResult Index()
    {
        try
        {
            var productos = _presupuestoR.ListarPresupuestos();
            return View(productos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al obtener los presupuestos.");
            return View("Error");
        }
    }

    [HttpGet("CrearPresupuesto")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult CrearPresupuesto()
    {
        try
        {
            var clientes = _clienteR.ListarClientes();
            var viewModel = new PresupuestoViewModel(clientes);

            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al obtener los clientes.");
            return View("Error");
        }
    }

    [HttpPost("CrearPresupuesto")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult CrearPresupuesto([FromForm] PresupuestoViewModel presupuestoViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores en el formulario. Por favor, corrígelos.";
                return RedirectToAction("CrearPresupuesto");
            }

            var cliente = _clienteR.ObtenerPorId(presupuestoViewModel.ClienteId);
            if (cliente.ClienteId == 0) return View(presupuestoViewModel);

            var detallesAgrupados = presupuestoViewModel.Detalle
                .GroupBy(d => d.IdProducto)
                .Select(g => new PresupuestoDetalleViewModel
                {
                    IdProducto = g.Key,
                    Cantidad = g.Sum(d => d.Cantidad)
                })
                .ToList();

            presupuestoViewModel.Detalle = detallesAgrupados;

            var presupuesto = new Presupuesto
            {
                Cliente = cliente
            };

            _presupuestoR.CrearPresupuesto(presupuesto);

            var idPresupuesto = _presupuestoR.ListarPresupuestos()
                                             .MaxBy(p => p.IdPresupuesto)
                                             .IdPresupuesto;

            foreach (var detalle in presupuestoViewModel.Detalle)
            {
                _presupuestoR.AgregarProductoYCantidad(idPresupuesto, detalle.IdProducto, detalle.Cantidad);
            }

            return RedirectToAction("Index", "Presupuesto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al crear el presupuesto.");
            TempData["ErrorMessage"] = "Hubo un error al crear el presupuesto.";
            return RedirectToAction("CrearPresupuesto");
        }
    }

    [HttpGet("ModificarPresupuesto/{id}")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult ModificarPresupuesto(int id)
    {
        try
        {
            var presupuesto = _presupuestoR.ObtenerPorId(id);
            if (presupuesto.IdPresupuesto == 0) return RedirectToAction("Index", "Presupuesto");

            var productos = _productoR.ListarProductos();
            var clientes = _clienteR.ListarClientes();

            var detalleViewModel = presupuesto.Detalle.Select(d => new PresupuestoDetalleViewModel(d)).ToList();

            var viewData = new ModificarPresupuestoViewModel(productos, clientes, detalleViewModel, presupuesto.IdPresupuesto);
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(viewData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al obtener los detalles del presupuesto.");
            return View("Error");
        }
    }

    [HttpPost("ModificarPresupuesto/{id}")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult ModificarPresupuesto([FromForm] ModificarPresupuestoViewModel presupuestoViewModel, int id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay errores en el formulario. Por favor, corrígelos.";
                return RedirectToAction("ModificarPresupuesto", new { id });
            }

            var presupuestoActual = _presupuestoR.ObtenerPorId(id);
            if (presupuestoActual.IdPresupuesto == 0) return BadRequest();

            if (presupuestoViewModel.ClienteId != 0) _presupuestoR.ModificarCliente(presupuestoActual.IdPresupuesto, presupuestoViewModel.ClienteId);

            var detallesAEliminar = presupuestoActual.Detalle
                .Where(detalleExistente => !presupuestoViewModel.Detalle
                    .Any(detalleNuevo => detalleNuevo.IdProducto == detalleExistente.Producto.IdProducto))
                .ToList();

            foreach (var detalle in detallesAEliminar)
            {
                _presupuestoR.EliminarDetalle(detalle, presupuestoViewModel.PresupuestoId);
            }

            foreach (var detalleNuevo in presupuestoViewModel.Detalle)
            {
                var detalleExistente = presupuestoActual.Detalle
                    .FirstOrDefault(detalle => detalle.Producto.IdProducto == detalleNuevo.IdProducto);

                if (detalleExistente != null)
                {
                    if (detalleExistente.Cantidad != detalleNuevo.Cantidad)
                    {
                        _presupuestoR.ModificarDetalle(detalleNuevo.IdProducto, presupuestoViewModel.PresupuestoId, detalleNuevo.Cantidad);
                    }
                }
                else
                {
                    _presupuestoR.AgregarProductoYCantidad(id, detalleNuevo.IdProducto, detalleNuevo.Cantidad);
                }
            }

            return RedirectToAction("Index", "Presupuesto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al modificar el presupuesto.");
            TempData["ErrorMessage"] = "Hubo un error al modificar el presupuesto.";
            return RedirectToAction("ModificarPresupuesto", new { id });
        }
    }

    [HttpPost("EliminarPresupuesto/{id}")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult EliminarPresupuesto(int id)
    {
        try
        {
            _presupuestoR.EliminarPresupuesto(id);
            return RedirectToAction("Index", "Presupuesto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al eliminar el presupuesto.");
            return RedirectToAction("Index", "Presupuesto");
        }
    }

    [HttpGet("AgregarPresupuestoDetalle")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult AgregarPresupuestoDetalle(int index)
    {
        try
        {
            var detalle = new PresupuestoDetalle();
            ViewData["Productos"] = _productoR.ListarProductos();

            var viewModel = new PresupuestoDetalleViewModel(detalle);

            ViewBag.Index = index;
            return PartialView("_PresupuestoDetalleForm", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al agregar detalle al presupuesto.");
            return PartialView("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
