using Microsoft.Data.Sqlite;
using tl2_tp6_2024_s0a0m.Models;

namespace tl2_tp6_2024_s0a0m.Repositorios;

public class PresupuestosRepository : IPresupuestosRepository
{
    private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";
    public void AgregarProductoYCantidad(int idPresupuesto, int idProducto, int cantidad)
    {
        var query = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
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

    public void CrearPresupuesto(Presupuesto presupuesto)
    {
        var query = $"INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion)";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            var command = new SqliteCommand(query, connection);
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");

            command.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));
            command.Parameters.Add(new SqliteParameter("@FechaCreacion", fechaActual));

            command.ExecuteNonQuery();

            connection.Close();
        }
    }

    public List<PresupuestoDetalle> DetallePresupuesto(int id)
    {
        var query = @"SELECT * FROM PresupuestosDetalle pd JOIN Productos p ON pd.idProducto = p.idProducto WHERE pd.idPresupuesto = @idPresupuesto;";
        List<PresupuestoDetalle> detalles = new();
        using (SqliteConnection connection = new(cadenaConexion))
        {
            SqliteCommand command = new(query, connection);
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
        return detalles;
    }

    public void EliminarDetalle(PresupuestoDetalle detalle, int idPresupuesto)
    {
        var queryPresupuesto = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
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

    public void EliminarPresupuesto(int id)
    {
        var queryDetalle = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto";
        var queryPresupuesto = "DELETE FROM Presupuestos WHERE idPresupuesto = @idPresupuesto";

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
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



    public List<Presupuesto> ListarPresupuestos()
    {
        var queryString = @"SELECT * FROM Presupuestos LEFT JOIN PresupuestosDetalle USING(idPresupuesto) LEFT JOIN Productos USING(idProducto) ORDER BY idPresupuesto;";
        List<Presupuesto> presupuestos = new();
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
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
                        // es otro presupuesto
                        var presupuesto = new Presupuesto();
                        presupuesto.NombreDestinatario = reader["NombreDestinatario"].ToString();
                        presupuesto.IdPresupuesto = idPresupuesto;
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
        return presupuestos;
    }

    public void ModificarDetalle(int idProducto, int idPresupuesto, int cantidadNueva)
    {
        var query = $"UPDATE PresupuestosDetalle SET Cantidad = @Cantidad WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto;";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
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

    // public List<Presupuesto> ListarPresupuestos()
    // {
    //     var queryString = @"SELECT * FROM Presupuestos;";
    //     List<Presupuesto> presupuestos = new();
    //     using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
    //     {
    //         SqliteCommand command = new SqliteCommand(queryString, connection);
    //         connection.Open();

    //         using (SqliteDataReader reader = command.ExecuteReader())
    //         {
    //             while (reader.Read())
    //             {
    //                 var presupuesto = new Presupuesto();
    //                 presupuesto.NombreDestinatario = reader["NombreDestinatario"].ToString();
    //                 presupuesto.IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]);
    //                 presupuestos.Add(presupuesto);
    //             }
    //         }
    //         connection.Close();
    //     }
    //     return presupuestos;
    // }

    public void ModificarProducto(int id, Presupuesto presupuesto)
    {
        var query = $"UPDATE Presupuestos SET NombreDestinatario = @NombreDestinatario WHERE idPresupuesto = @idPresupuesto ";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            var command = new SqliteCommand(query, connection);

            command.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));
            command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));

            command.ExecuteNonQuery();

            connection.Close();
        }
    }

    public Presupuesto ObtenerPresupuesto(int id)
    {
        var query = @"SELECT * FROM Presupuestos LEFT JOIN PresupuestosDetalle USING(idPresupuesto) LEFT JOIN Productos USING (idProducto) WHERE idPresupuesto = @idPresupuesto";
        var presupuesto = new Presupuesto();
        using (var connection = new SqliteConnection(cadenaConexion))
        {
            var command = new SqliteCommand(query, connection);
            command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    presupuesto.NombreDestinatario = reader["NombreDestinatario"].ToString();
                    presupuesto.IdPresupuesto = Convert.ToInt32(reader["IdPresupuesto"]);

                    if (reader["idProducto"] == DBNull.Value)
                    {
                        connection.Close();
                        return presupuesto;
                    }

                    do
                    {
                        presupuesto.Detalle.Add(
                            new PresupuestoDetalle
                            {
                                Producto = new Producto
                                {
                                    IdProducto = Convert.ToInt32(reader["idProducto"]),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    Precio = Convert.ToInt32(reader["Precio"])
                                },
                                Cantidad = Convert.ToInt32(reader["Cantidad"])
                            }
                        );
                    } while (reader.Read());
                }
            }
            connection.Close();
        }
        return presupuesto;
    }
}