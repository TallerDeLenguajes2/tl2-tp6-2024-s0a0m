namespace tl2_tp5_2024_s0a0m.Models;

public class Presupuesto
{

    public int IdPresupuesto { get; set; }
    public string NombreDestinatario { get; set; }
    public List<PresupuestoDetalle> Detalle { get; set; }

    public Presupuesto()
    {
        Detalle = new List<PresupuestoDetalle>();
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