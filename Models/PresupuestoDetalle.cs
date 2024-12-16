namespace tl2_tp6_2024_s0a0m.Models;

public class PresupuestoDetalle
{
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }

    public PresupuestoDetalle()
    {
        Producto = new();
    }

}