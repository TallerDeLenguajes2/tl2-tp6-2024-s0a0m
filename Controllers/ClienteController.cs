using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_s0a0m.Models;
using tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.ViewModels;
using tl2_tp6_2024_s0a0m.Filters;

namespace tl2_tp6_2024_s0a0m.Controllers;

[Route("[controller]")]
public class ClienteController : Controller
{
    private readonly ILogger<ClienteController> _logger;
    private readonly IClienteRepository _clienteR;

    public ClienteController(ILogger<ClienteController> logger, IClienteRepository clienteR)
    {
        _logger = logger;
        _clienteR = clienteR;
    }

    [HttpGet]
    [AccessLevelAuthorize("Administrador", "Cliente")]
    public IActionResult Index()
    {
        try
        {
            var clientes = _clienteR.ListarClientes();
            var viewModel = clientes.Select(c => new ClienteViewModel(c)).ToList();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al listar clientes.");
            return RedirectToAction("Error");
        }
    }

    [HttpGet("CrearCliente")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult CrearCliente()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al cargar la vista de creación de cliente.");
            return RedirectToAction("Error");
        }
    }

    [HttpPost("CrearCliente")]
    [AccessLevelAuthorize("Administrador")]
    public IActionResult CrearCliente([FromForm] ClienteViewModel clienteViewModel)
    {
        try
        {
            if (!ModelState.IsValid) return View(clienteViewModel);

            var cliente = new Cliente
            {
                Nombre = clienteViewModel.Nombre,
                Telefono = clienteViewModel.Telefono,
                Email = clienteViewModel.Email
            };

            _clienteR.CrearCliente(cliente);

            return RedirectToAction("Index", "Cliente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString(), "Error al crear un nuevo cliente.");
            return RedirectToAction("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
