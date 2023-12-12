using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository
{
    public class TableroRepository : ITableroRepository
    {
        private readonly string? _connectionString;

        public TableroRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Tablero Create(Tablero nuevoTablero)
        {
            try
            {
                var query = @"INSERT INTO tablero (id_usuario_propietario, nombre, descripcion) VALUES (@idUsuarioPropietario, @nombre, @descripcion);";

                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                using var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@idUsuarioPropietario", nuevoTablero.IdUsuarioPropietario));
                command.Parameters.Add(new SQLiteParameter("@nombre", nuevoTablero.Nombre));
                command.Parameters.Add(new SQLiteParameter("@descripcion", nuevoTablero.Descripcion));

                command.ExecuteNonQuery();
                connection.Close();

                return nuevoTablero;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el tablero.", ex);
            }
        }

        public void Delete(int idTablero)
        {
            try
            {
                var query = @"DELETE FROM tablero WHERE id = @idTablero;";

                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.Add(new SQLiteParameter("@idTablero", idTablero));

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el tablero.", ex);
            }
        }

        public Tablero GetById(int idTablero)
        {
            try
            {
                var query = @"SELECT * FROM tablero WHERE id = @idTablero;";
                var board = new Tablero();

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    SQLiteCommand command = new(query, connection);
                    command.Parameters.Add(new SQLiteParameter("@idTablero", idTablero));
                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            board.Id = Convert.ToInt32(reader["id"]);
                            board.Nombre = reader["nombre"].ToString();
                            board.Descripcion = reader["descripcion"].ToString();
                            board.IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]);
                        }
                    }
                    connection.Close();
                }
                return board;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el tablero.", ex);
            }
        }

        public List<Tablero> List()
        {
            try
            {
                var query = @"SELECT * FROM tablero;";
                var boards = new List<Tablero>();

                using (SQLiteConnection connection = new(_connectionString))
                {
                    SQLiteCommand comando = new(query, connection);
                    connection.Open();

                    using (SQLiteDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tablero board = new()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Nombre = reader["nombre"].ToString(),
                                Descripcion = reader["descripcion"].ToString(),
                                IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"])
                            };
                            boards.Add(board);
                        }
                    }
                    connection.Close();
                }
                return boards;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de tableros.", ex);
            }
        }

        public List<Tablero> ListUserBoards(int idUsuario)
        {
            try
            {
                var query = @"SELECT * FROM tablero WHERE id_usuario_propietario = @idUsuario;";
                var boards = new List<Tablero>();

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    var comando = new SQLiteCommand(query, connection);
                    comando.Parameters.Add(new SQLiteParameter("@idUsuario", idUsuario));
                    connection.Open();

                    using (SQLiteDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var board = new Tablero()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Nombre = reader["nombre"].ToString(),
                                Descripcion = reader["descripcion"].ToString(),
                                IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"])
                            };
                            boards.Add(board);
                        }
                    }
                    connection.Close();
                }
                return boards;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de tableros de usuario.", ex);
            }
        }

        public void Update(int idTablero, Tablero tablero)
        {
            try
            {
                var query = @"UPDATE tablero SET nombre = @nombre, descripcion = @descripcion, id_usuario_propietario = @idUsuarioPropietario WHERE id = @idTablero;";

                using var conexion = new SQLiteConnection(_connectionString);
                SQLiteCommand command = new(query, conexion);
                conexion.Open();

                command.Parameters.Add(new SQLiteParameter("@nombre", tablero.Nombre));
                command.Parameters.Add(new SQLiteParameter("@descripcion", tablero.Descripcion));
                command.Parameters.Add(new SQLiteParameter("@idUsuarioPropietario", tablero.IdUsuarioPropietario));
                command.Parameters.Add(new SQLiteParameter("@idTablero", idTablero));

                command.ExecuteNonQuery();
                conexion.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el tablero.", ex);
            }
        }
    }
}
