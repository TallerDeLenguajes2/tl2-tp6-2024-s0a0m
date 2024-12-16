namespace tl2_tp6_2024_s0a0m.Models;

public enum Rol
{
    Administrador = 0,
    Cliente = 1,
    NoLogueado = 2
}
public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string NombreUsuario { get; set; }
    public string Contrasena { get; set; }
    public Rol Rol { get; set; }
}