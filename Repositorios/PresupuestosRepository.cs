using Microsoft.Data.Sqlite;
using tl2_tp5_2024_s0a0m.Models;

namespace tl2_tp5_2024_s0a0m.Repositorios;

public class PresupuestosRepository : IPresupuestosRepository
{
    private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";
    public void AgregarProductoYCantidad(int id)
    {
        throw new NotImplementedException();
    }

    public void CrearPresupuesto(Presupuesto presupuesto)
    {
        var query = $"INSERT INTO Presupuestos (idPresupuesto, NombreDestinatario, FechaCreacion) VALUES (@idPresupuesto, @NombreDestinatario, @FechaCreacion)";
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            var command = new SqliteCommand(query, connection);
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");

            command.Parameters.Add(new SqliteParameter("@idPresupuesto", presupuesto.IdPresupuesto));
            command.Parameters.Add(new SqliteParameter("@NombreDestinatario", presupuesto.NombreDestinatario));
            command.Parameters.Add(new SqliteParameter("@FechaCreacion", fechaActual));

            command.ExecuteNonQuery();

            connection.Close();
        }
    }

    public List<PresupuestoDetalle> DetallePresupuesto(int id)
    {
        var query = @"SELECT * FROM PresupuestoDetalle pd JOIN Producto p ON pd.idProducto = p.idProducto WHERE pd.idPresupuesto = @idPresupuesto;";
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

    public void EliminarPresupuesto(int id)
    {
        throw new NotImplementedException();
    }

    public List<Presupuesto> ListarPresupuestos()
    {
        var queryString = @"SELECT * FROM Presupuestos  LEFT JOIN PresupuestosDetalle USING(idPresupuesto) LEFT JOIN Productos USING(idProducto) ORDER BY idPresupuesto;";
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

    public Presupuesto ObtenerPresupuesto(int id)
    {
        var query = @"SELECT * FROM Presupuestos INNER JOIN PresupuestosDetalle USING(idPresupuesto) INNER JOIN Productos USING (idProducto) WHERE idPresupuesto = @idPresupuesto";
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