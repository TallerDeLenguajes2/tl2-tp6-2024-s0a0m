using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_tp6_2024_s0a0m.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace tl2_tp6_2024_s0a0m.ViewModels;
public class PresupuestoViewModel
{
    [BindNever]
    public List<Cliente> Clientes { get; set; }
    [Required(ErrorMessage = "Debe seleccionar un cliente.")]
    public int ClienteId { get; set; }
    public List<PresupuestoDetalleViewModel> Detalle { get; set; }
    public PresupuestoViewModel()
    {
        Detalle = new();
        Clientes = new();
    }

    public PresupuestoViewModel(List<Cliente> clientes)
    {
        this.Clientes = clientes;
        Detalle = new();
    }
}