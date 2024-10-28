namespace tl2_tp5_2024_s0a0m.Repositorios;
using tl2_tp5_2024_s0a0m.Models;
public interface IPresupuestosRepository
{
    /*
        ● Crear un nuevo Presupuesto. (recibe un objeto Presupuesto)
        ● Listar todos los Presupuestos registrados. (devuelve un List de Presupuestos)
        ● Obtener detalles de un Presupuesto por su ID. (recibe un Id y devuelve un
        Presupuesto con sus Presupuestos y cantidades)
        ● Agregar un producto y una cantidad a un presupuesto (recibe un Id)
        ● Eliminar un Presupuesto por ID
    */

    public void CrearPresupuesto(Presupuesto presupuesto);
    public List<Presupuesto> ListarPresupuestos();
    public List<PresupuestoDetalle> DetallePresupuesto(int id);
    public void AgregarProductoYCantidad(int id);
    public void EliminarPresupuesto(int id);
    public Presupuesto ObtenerPresupuesto(int id);

}