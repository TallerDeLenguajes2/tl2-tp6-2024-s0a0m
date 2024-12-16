using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_s0a0m.Models;
using tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.ViewModels;
using tl2_tp6_2024_s0a0m.Filters;

namespace tl2_tp6_2024_s0a0m.Controllers;

[Route("[controller]")]
public class ProductoController : Controller
{
    private readonly ILogger<ProductoController> _logger;
    private readonly IProductoRepository _productoR;

    public ProductoController(ILogger<ProductoController> logger, IProductoRepository productoR)
    {
        _logger = logger;
        _productoR = productoR;
    }

    [HttpGet]
    [AccessLevelAuthorize("Administrador", "Cliente")]
    public IActionResult Index()
    {
        try
        {
            var productos = _productoR.ListarProductos();
            return View(productos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al listar productos.");
            return RedirectToAction("Error");
        }
    }

    [HttpGet("CrearProducto")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult CrearProducto()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al cargar la vista de creación de producto.");
            return RedirectToAction("Error");
        }
    }

    [HttpPost("CrearProducto")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult CrearProducto([FromForm] ProductoViewModel productoViewModel)
    {
        try
        {
            if (!ModelState.IsValid) return View(productoViewModel);

            var producto = new Producto
            {
                Descripcion = productoViewModel.Descripcion,
                Precio = productoViewModel.Precio
            };
            _productoR.CrearProducto(producto);

            return RedirectToAction("Index", "Producto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al crear un nuevo producto.");
            return RedirectToAction("Error");
        }
    }

    [HttpGet("ModificarProducto/{id}")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult ModificarProducto(int id)
    {
        try
        {
            var producto = _productoR.ObtenerPorId(id);
            if (producto == null || producto.IdProducto == 0)
                return RedirectToAction("Index", "Producto");

            var viewModel = new ProductoViewModel(producto);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), $"Error al cargar la vista de modificación para el producto con ID {id}.");
            return RedirectToAction("Error");
        }
    }

    [HttpPost("ModificarProducto/{id}")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult ModificarProducto([FromForm] ProductoViewModel productoViewModel, int id)
    {
        try
        {
            if (!ModelState.IsValid) return View(productoViewModel);

            var producto = new Producto
            {
                Descripcion = productoViewModel.Descripcion,
                Precio = productoViewModel.Precio
            };

            _productoR.ModificarProducto(id, producto);
            return RedirectToAction("Index", "Producto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), $"Error al modificar el producto con ID {id}.");
            return RedirectToAction("Error");
        }
    }

    [HttpPost("EliminarProducto/{id}")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult EliminarProducto(int id)
    {
        try
        {
            _productoR.EliminarProducto(id);
            return RedirectToAction("Index", "Producto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), $"Error al eliminar el producto con ID {id}.");
            return RedirectToAction("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
