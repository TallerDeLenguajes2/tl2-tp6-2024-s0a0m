using Microsoft.Data.Sqlite;
using tl2_tp6_2024_s0a0m.Models;

namespace tl2_tp6_2024_s0a0m.Repositorios;

public class ClienteRepository : IClienteRepository
{
    private readonly string _ConnectionString;
    public ClienteRepository(string ConnectionString)
    {
        _ConnectionString = ConnectionString;
    }

    public Cliente ObtenerPorId(int id)
    {
        var cliente = new Cliente();
        var query = "SELECT * FROM Clientes WHERE ClienteId = @ClienteId";

        try
        {
            using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@ClienteId", id));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente.ClienteId = Convert.ToInt32(reader["ClienteId"]);
                            cliente.Nombre = reader["Nombre"].ToString();
                            cliente.Email = reader["Email"].ToString();
                            cliente.Telefono = reader["Telefono"].ToString();
                        }
                        else
                        {
                            throw new Exception("Cliente inexistente.");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener cliente por id: " + ex.Message);
        }

        return cliente;
    }

    public List<Cliente> ListarClientes()
    {
        var queryString = @"SELECT * FROM Clientes;";
        List<Cliente> clientes = new();
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
            {
                SqliteCommand command = new SqliteCommand(queryString, connection);
                connection.Open();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cliente = new Cliente();
                        cliente.ClienteId = Convert.ToInt32(reader["ClienteId"]);
                        cliente.Nombre = reader["Nombre"].ToString();
                        cliente.Email = reader["Email"].ToString();
                        cliente.Telefono = reader["Telefono"].ToString();

                        clientes.Add(cliente);
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al listar clientes: " + ex.Message);
        }

        return clientes;
    }

    public void CrearCliente(Cliente cliente)
    {
        var query = $"INSERT INTO Clientes (Nombre, Email, Telefono) VALUES (@Nombre, @Email, @Telefono)";
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);

                command.Parameters.Add(new SqliteParameter("@Nombre", cliente.Nombre));
                command.Parameters.Add(new SqliteParameter("@Email", cliente.Email));
                command.Parameters.Add(new SqliteParameter("@Telefono", cliente.Telefono));

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear cliente: " + ex.Message);
        }
    }
}
