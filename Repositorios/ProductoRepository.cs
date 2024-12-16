using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using tl2_tp6_2024_s0a0m.Models;

namespace tl2_tp6_2024_s0a0m.Repositorios
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _ConnectionString;
        public ProductoRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }

        public void CrearProducto(Producto producto)
        {
            try
            {
                var query = $"INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    var command = new SqliteCommand(query, connection);

                    command.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
                    command.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqliteException ex)
            {
                // Manejo de error específico para SQLite
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Manejo de cualquier otro tipo de error
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }

        public void EliminarProducto(int id)
        {
            try
            {
                var deleteDetallesQuery = "DELETE FROM PresupuestosDetalle WHERE idProducto = @idProducto;";
                var deleteProductoQuery = "DELETE FROM Productos WHERE idProducto = @idProducto;";

                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(deleteDetallesQuery, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@idProducto", id));
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SqliteCommand(deleteProductoQuery, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@idProducto", id));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }

        public List<Producto> ListarProductos()
        {
            List<Producto> productos = new();
            try
            {
                var queryString = @"SELECT * FROM Productos;";
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    SqliteCommand command = new SqliteCommand(queryString, connection);
                    connection.Open();

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Producto();
                            producto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                            producto.Precio = Convert.ToInt32(reader["Precio"]);
                            producto.Descripcion = reader["Descripcion"].ToString();

                            productos.Add(producto);
                        }
                    }
                    connection.Close();
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
            return productos;
        }

        public void ModificarProducto(int id, Producto producto)
        {
            try
            {
                var query = $"UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @idProducto";
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    var command = new SqliteCommand(query, connection);

                    command.Parameters.Add(new SqliteParameter("@Descripcion", producto.Descripcion));
                    command.Parameters.Add(new SqliteParameter("@Precio", producto.Precio));
                    command.Parameters.Add(new SqliteParameter("@idProducto", id));

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }

        public Producto ObtenerPorId(int id)
        {
            var producto = new Producto();
            try
            {
                var query = "SELECT * FROM Productos WHERE idProducto = @idProducto";

                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@idProducto", id));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                producto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                                producto.Descripcion = reader["Descripcion"].ToString();
                                producto.Precio = Convert.ToInt32(reader["Precio"]);
                            }
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
            return producto;
        }
    }
}
