using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using tl2_tp6_2024_s0a0m.Models;

namespace tl2_tp6_2024_s0a0m.Repositorios
{
    public class PresupuestosRepository : IPresupuestosRepository
    {
        private readonly string _ConnectionString;
        public PresupuestosRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }

        public void AgregarProductoYCantidad(int idPresupuesto, int idProducto, int cantidad)
        {
            var query = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
                        command.Parameters.Add(new SqliteParameter("@idProducto", idProducto));
                        command.Parameters.Add(new SqliteParameter("@Cantidad", cantidad));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar el producto: {ex.Message}");
            }
        }

        public void CrearPresupuesto(Presupuesto presupuesto)
        {
            var query = $"INSERT INTO Presupuestos (ClienteId, FechaCreacion) VALUES (@ClienteId, @FechaCreacion)";
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    var command = new SqliteCommand(query, connection);
                    string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");

                    command.Parameters.Add(new SqliteParameter("@ClienteId", presupuesto.Cliente.ClienteId));
                    command.Parameters.Add(new SqliteParameter("@FechaCreacion", fechaActual));

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el presupuesto: {ex.Message}");
            }
        }

        public List<PresupuestoDetalle> DetallePresupuesto(int id)
        {
            var query = @"SELECT * FROM PresupuestosDetalle pd JOIN Productos p ON pd.idProducto = p.idProducto WHERE pd.idPresupuesto = @idPresupuesto;";
            List<PresupuestoDetalle> detalles = new List<PresupuestoDetalle>();
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                    connection.Open();
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var detalle = new PresupuestoDetalle
                            {
                                Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                Producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(reader["idProducto"]),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    Precio = Convert.ToInt32(reader["Precio"])
                                }
                            };

                            detalles.Add(detalle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los detalles del presupuesto: {ex.Message}");
            }
            return detalles;
        }

        public void EliminarDetalle(PresupuestoDetalle detalle, int idPresupuesto)
        {
            var queryPresupuesto = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(queryPresupuesto, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
                        command.Parameters.Add(new SqliteParameter("@idProducto", detalle.Producto.IdProducto));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el detalle: {ex.Message}");
            }
        }

        public void EliminarPresupuesto(int id)
        {
            var queryDetalle = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto";
            var queryPresupuesto = "DELETE FROM Presupuestos WHERE idPresupuesto = @idPresupuesto";
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqliteCommand(queryDetalle, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SqliteCommand(queryPresupuesto, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el presupuesto: {ex.Message}");
            }
        }

        public List<Presupuesto> ListarPresupuestos()
        {
            var queryString = @"SELECT * FROM Presupuestos LEFT JOIN PresupuestosDetalle USING(idPresupuesto) LEFT JOIN Productos USING(idProducto) LEFT JOIN Clientes USING(ClienteId) ORDER BY idPresupuesto;";
            List<Presupuesto> presupuestos = new List<Presupuesto>();
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    SqliteCommand command = new SqliteCommand(queryString, connection);
                    connection.Open();

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        var idPresupuestoActual = -1;
                        while (reader.Read())
                        {
                            var idPresupuesto = Convert.ToInt32(reader["idPresupuesto"]);

                            if (idPresupuesto != idPresupuestoActual)
                            {
                                var presupuesto = new Presupuesto
                                {
                                    IdPresupuesto = idPresupuesto,
                                    Cliente = new Cliente
                                    {
                                        ClienteId = Convert.ToInt32(reader["ClienteId"]),
                                        Nombre = reader["Nombre"].ToString(),
                                        Email = reader["Email"].ToString(),
                                        Telefono = reader["Telefono"].ToString()
                                    }
                                };
                                presupuestos.Add(presupuesto);

                                if (reader["idProducto"] != DBNull.Value)
                                {
                                    var detallePresupuesto = new PresupuestoDetalle
                                    {
                                        Producto = new Producto
                                        {
                                            IdProducto = Convert.ToInt32(reader["idProducto"]),
                                            Descripcion = reader["Descripcion"].ToString(),
                                            Precio = Convert.ToInt32(reader["Precio"])
                                        },
                                        Cantidad = Convert.ToInt32(reader["Cantidad"])
                                    };
                                    presupuestos.Last().Detalle.Add(detallePresupuesto);
                                }

                                idPresupuestoActual = idPresupuesto;
                            }
                            else
                            {
                                var detallePresupuesto = new PresupuestoDetalle
                                {
                                    Producto = new Producto
                                    {
                                        IdProducto = Convert.ToInt32(reader["idProducto"]),
                                        Descripcion = reader["Descripcion"].ToString(),
                                        Precio = Convert.ToInt32(reader["Precio"])
                                    },
                                    Cantidad = Convert.ToInt32(reader["Cantidad"])
                                };
                                presupuestos.Last().Detalle.Add(detallePresupuesto);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar los presupuestos: {ex.Message}");
            }
            return presupuestos;
        }

        public void ModificarCliente(int idPresupuesto, int idCliente)
        {
            var query = $"UPDATE Presupuestos SET ClienteId = @ClienteId WHERE idPresupuesto = @idPresupuesto";
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    var command = new SqliteCommand(query, connection);

                    command.Parameters.Add(new SqliteParameter("@ClienteId", idCliente));
                    command.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar el cliente: {ex.Message}");
            }
        }

        public void ModificarDetalle(int idProducto, int idPresupuesto, int cantidadNueva)
        {
            var query = $"UPDATE PresupuestosDetalle SET Cantidad = @Cantidad WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();
                    var command = new SqliteCommand(query, connection);

                    command.Parameters.Add(new SqliteParameter("@Cantidad", cantidadNueva));
                    command.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
                    command.Parameters.Add(new SqliteParameter("@idProducto", idProducto));
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar el detalle: {ex.Message}");
            }
        }

        public Presupuesto ObtenerPorId(int id)
        {
            var query = @"SELECT * FROM Presupuestos LEFT JOIN PresupuestosDetalle USING(idPresupuesto) LEFT JOIN Productos USING (idProducto) LEFT JOIN Clientes USING(ClienteId) WHERE idPresupuesto = @idPresupuesto";
            var presupuesto = new Presupuesto();
            try
            {
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            presupuesto.Cliente = new Cliente
                            {
                                ClienteId = Convert.ToInt32(reader["ClienteId"]),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString()
                            };

                            presupuesto.IdPresupuesto = Convert.ToInt32(reader["IdPresupuesto"]);

                            if (reader["idProducto"] == DBNull.Value)
                            {
                                connection.Close();
                                return presupuesto;
                            }

                            do
                            {
                                presupuesto.Detalle.Add(new PresupuestoDetalle
                                {
                                    Producto = new Producto
                                    {
                                        IdProducto = Convert.ToInt32(reader["idProducto"]),
                                        Descripcion = reader["Descripcion"].ToString(),
                                        Precio = Convert.ToInt32(reader["Precio"])
                                    },
                                    Cantidad = Convert.ToInt32(reader["Cantidad"])
                                });
                            } while (reader.Read());
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el presupuesto por ID: {ex.Message}");
            }
            return presupuesto;
        }
    }
}
