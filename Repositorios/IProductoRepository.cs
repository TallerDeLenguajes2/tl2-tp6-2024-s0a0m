namespace tl2_tp5_2024_s0a0m.Repositorios;
using tl2_tp5_2024_s0a0m.Models;
public interface IProductoRepository
{
    /*
        ● Crear un nuevo Producto. (recibe un objeto Producto)
        ● Modificar un Producto existente. (recibe un Id y un objeto Producto)
        ● Listar todos los Productos registrados. (devuelve un List de Producto)
        ● Obtener detalles de un Productos por su ID. (recibe un Id y devuelve un
        Producto)
        ● Eliminar un Producto por ID
    */
    
    public void CrearProducto(Producto producto);
    public void ModificarProducto(int id, Producto producto);
    public List<Producto> ListarProductos();
    public Producto DetalleProducto(int id);
    public void EliminarProducto(int id);
    
}