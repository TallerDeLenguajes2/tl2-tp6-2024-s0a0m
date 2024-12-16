using tl2_tp6_2024_s0a0m.Models;
using System.ComponentModel.DataAnnotations;
namespace tl2_tp6_2024_s0a0m.ViewModels;
public class UsuarioViewModel
{
    [Required(ErrorMessage = "Debe ingresar el Nombre de Usuario!")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "El Nombre de Usuario debe tener entre 3 y 20 caracteres!")]
    public string NombreUsuario { get; set; }

    [Required(ErrorMessage = "Debe ingresar la Contraseña!")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "La Contraseña debe tener entre 3 y 20 caracteres!")]
    public string Contrasena { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }

    public UsuarioViewModel()
    {
        IsAuthenticated = false;
    }
    public UsuarioViewModel(Usuario usuario)
    {
        NombreUsuario = usuario.NombreUsuario;
        Contrasena = usuario.Contrasena;
        IsAuthenticated = false;
    }

}