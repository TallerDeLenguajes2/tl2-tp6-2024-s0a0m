using Microsoft.Data.Sqlite;
using tl2_tp6_2024_s0a0m.Models;

namespace tl2_tp6_2024_s0a0m.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _ConnectionString;
        public UsuarioRepository(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }

        public Usuario Obtener(string nombreUsuario, string contrasena)
        {
            var usuario = new Usuario();

            try
            {
                using (var conexion = new SqliteConnection(_ConnectionString))
                {
                    var consulta = @"SELECT * FROM Usuarios 
                                    WHERE NombreUsuario = @nombreUsuario 
                                    AND Contrasena = @contrasena";

                    conexion.Open();

                    var comando = new SqliteCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    comando.Parameters.AddWithValue("@contrasena", contrasena);

                    using (var lectorDatos = comando.ExecuteReader())
                    {
                        if (lectorDatos.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(lectorDatos["IdUsuario"]),
                                Nombre = Convert.ToString(lectorDatos["Nombre"]),
                                NombreUsuario = Convert.ToString(lectorDatos["NombreUsuario"]),
                                Contrasena = Convert.ToString(lectorDatos["Contrasena"]),
                                Rol = (Rol)Convert.ToInt32(lectorDatos["Rol"])
                            };
                        }
                    }

                    conexion.Close();
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

            return usuario;
        }
    }
}
