namespace tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.Models;
public interface IClienteRepository
{
    public List<Cliente> ListarClientes();
    public Cliente ObtenerPorId(int id);
    public void CrearCliente(Cliente cliente);
}