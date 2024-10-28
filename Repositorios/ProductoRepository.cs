using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using tl2_tp5_2024_s0a0m.Models;

namespace tl2_tp5_2024_s0a0m.Repositorios;

public class ProductoRepository : IProductoRepository
{
    private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";
    public void CrearProducto(Producto producto) {
        var query = $"INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            var command = new SqliteCommand(query, connection);

            command.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
            command.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));

            command.ExecuteNonQuery();

            connection.Close();   
        }
    }

    public Producto DetalleProducto(int id)
    {
        throw new NotImplementedException();
    }

    public void EliminarProducto(int id)
    {
        throw new NotImplementedException();
    }

    public List<Producto> ListarProductos()
    {
        var queryString = @"SELECT * FROM Productos;";
        List<Producto> productos = new();
            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                SqliteCommand command = new SqliteCommand(queryString, connection);
                connection.Open();
            
                using(SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var producto = new Producto();
                        producto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                        producto.Descripcion = reader["Descripcion"].ToString();

                        // hacer el *** while

                        productos.Add(producto);
                    }
                }
                connection.Close();
            }
            return productos;
    }

    public void ModificarProducto(int id, Producto producto)
    {

        var query = $"UPDATE Productos SET Descripcion = @Descripcion WHERE idProducto = @idProducto ";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            var command = new SqliteCommand(query, connection);

            command.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
            command.Parameters.Add(new SqliteParameter("@idProducto", id));

            command.ExecuteNonQuery();

            connection.Close();   
        }
    }
}