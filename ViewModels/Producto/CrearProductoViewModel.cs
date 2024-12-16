using System.ComponentModel.DataAnnotations;
using tl2_tp6_2024_s0a0m.Models;

namespace tl2_tp6_2024_s0a0m.ViewModels
{
    public class ProductoViewModel
    {
        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public int Precio { get; set; }

        public ProductoViewModel()
        {

        }

        public ProductoViewModel(Producto p)
        {
            Descripcion = p.Descripcion;
            Precio = p.Precio;
        }
    }
}
