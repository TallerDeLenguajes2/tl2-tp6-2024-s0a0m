using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_tp6_2024_s0a0m.Models;
namespace tl2_tp6_2024_s0a0m.ViewModels;
public class ModificarPresupuestoViewModel
{
    public List<Producto> Productos { get; set; }
    public int ClienteId { get; set; }
    public int PresupuestoId { get; set; }
    public List<Cliente> Clientes { get; set; }
    public List<PresupuestoDetalleViewModel> Detalle { get; set; }

    public ModificarPresupuestoViewModel(List<Producto> productos, List<Cliente> clientes, List<PresupuestoDetalleViewModel> detalle, int presupuestoId)
    {
        Productos = productos;
        Clientes = clientes;
        Detalle = detalle;
        PresupuestoId = presupuestoId;
    }
    public ModificarPresupuestoViewModel()
    {
        Detalle = new();
        Clientes = new();
        Productos = new();
    }
}