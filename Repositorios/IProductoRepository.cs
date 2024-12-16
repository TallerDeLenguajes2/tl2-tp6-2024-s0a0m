namespace tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.Models;
public interface IProductoRepository
{
    public void CrearProducto(Producto producto);
    public void ModificarProducto(int id, Producto producto);
    public List<Producto> ListarProductos();
    public void EliminarProducto(int id);
    public Producto ObtenerPorId(int id);

}