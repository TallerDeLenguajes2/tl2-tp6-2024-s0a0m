namespace tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.Models;
public interface IPresupuestosRepository
{
    public void CrearPresupuesto(Presupuesto presupuesto);
    public List<Presupuesto> ListarPresupuestos();
    public List<PresupuestoDetalle> DetallePresupuesto(int id);
    public void AgregarProductoYCantidad(int idPresupuesto, int idProducto, int cantidad);
    public void EliminarPresupuesto(int id);
    public Presupuesto ObtenerPorId(int id);
    public void ModificarDetalle(int idProducto, int idPresupuesto, int cantidadNueva);
    public void EliminarDetalle(PresupuestoDetalle detalle, int idPresupuesto);
    public void ModificarCliente(int idPresupeusto, int idCliente);

}