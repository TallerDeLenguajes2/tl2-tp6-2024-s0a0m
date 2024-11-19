namespace tl2_tp6_2024_s0a0m.Models;

public class Presupuesto
{

    public int IdPresupuesto { get; set; }
    public Cliente Cliente { get; set; }
    public List<PresupuestoDetalle> Detalle { get; set; }

    public Presupuesto()
    {
        Detalle = new List<PresupuestoDetalle>();
        Cliente = new Cliente();
    }

    public void MontoPresupuesto()
    {

    }

    public void MontoPresupuestoConIv()
    {

    }

    public void CantidadProductos()
    {

    }
}