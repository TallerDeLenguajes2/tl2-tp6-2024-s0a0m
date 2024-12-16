using System.ComponentModel.DataAnnotations;
namespace tl2_tp6_2024_s0a0m.Models;

public class Producto
{
    public int IdProducto { get; set; }
    public string Descripcion { get; set; }
    public int Precio { get; set; }
}