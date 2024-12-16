using tl2_tp6_2024_s0a0m.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace tl2_tp6_2024_s0a0m.ViewModels;
public class PresupuestoDetalleViewModel
{
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }

    public PresupuestoDetalleViewModel()
    {

    }
    public PresupuestoDetalleViewModel(PresupuestoDetalle pd)
    {
        IdProducto = pd.Producto.IdProducto;
        Cantidad = pd.Cantidad;
    }
}