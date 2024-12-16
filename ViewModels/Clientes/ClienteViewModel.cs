
using System.ComponentModel.DataAnnotations;
using tl2_tp6_2024_s0a0m.Models;
namespace tl2_tp6_2024_s0a0m.ViewModels;
public class ClienteViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
    public string Telefono { get; set; }

    public ClienteViewModel()
    {

    }
    public ClienteViewModel(Cliente cliente)
    {
        Nombre = cliente.Nombre;
        Email = cliente.Email;
        Telefono = cliente.Telefono;
    }

}