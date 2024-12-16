namespace tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.Models;
public interface IUsuarioRepository
{
    public Usuario Obtener(string nombreUsuario, string contrasena);
}