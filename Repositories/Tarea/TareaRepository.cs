using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository
{
    public class TareaRepository : ITareaRepository
    {
        private readonly string? _connectionString;

        public TareaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AssignUser(int userId, int taskId)
        {
            try
            {
                var queryString = @"UPDATE Tarea SET id_usuario_asignado = @userId WHERE id = @taskId;";

                using var connection = new SQLiteConnection(_connectionString);
                using var command = new SQLiteCommand(queryString, connection);
                connection.Open();

                command.Parameters.Add(new SQLiteParameter("@userId", userId));
                command.Parameters.Add(new SQLiteParameter("@taskId", taskId));

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar usuario a la tarea.", ex);
            }
        }

        public void Create(int boardId, Tarea task)
        {
            try
            {
                var query = $"INSERT INTO tarea (id_tablero, nombre, estado, descripcion, color, id_usuario_asignado) VALUES (@id_tablero, @name, @estado, @descripcion, @color, @usuario)";
                using SQLiteConnection connection = new(_connectionString);

                connection.Open();
                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@id_tablero", boardId)); // porque le estamos mandando por parametro 
                command.Parameters.Add(new SQLiteParameter("@name", task.Nombre));
                command.Parameters.Add(new SQLiteParameter("@estado", (int)task.Estado));
                command.Parameters.Add(new SQLiteParameter("@descripcion", task.Descripcion));
                command.Parameters.Add(new SQLiteParameter("@color", task.Color));
                command.Parameters.Add(new SQLiteParameter("@usuario", task.IdUsuarioAsignado));

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la tarea.", ex);
            }
        }

        public void Delete(int taskId)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM tarea WHERE Id = @taskId";
                command.Parameters.Add(new SQLiteParameter("@taskId", taskId));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la tarea.", ex);
            }
        }

        public Tarea GetById(int taskId)
        {
            try
            {
                var task = new Tarea();

                using var connection = new SQLiteConnection(_connectionString);
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM tarea WHERE id = @taskId";
                command.Parameters.Add(new SQLiteParameter("@taskId", taskId));
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        task.Id = Convert.ToInt32(reader["id"]);
                        task.IdTablero = Convert.ToInt32(reader["id_tablero"]);
                        task.Nombre = reader["nombre"].ToString();
                        task.Estado = (EstadoTarea)Convert.ToInt32(reader["estado"]);
                        task.Descripcion = reader["descripcion"].ToString();
                        task.Color = reader["color"].ToString();
                        task.IdUsuarioAsignado = Convert.ToInt32(reader["id_usuario_asignado"]);
                    }
                }

                connection.Close();
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la tarea por ID.", ex);
            }
        }

        public List<Tarea> ListByBoard(int boardId)
        {
            try
            {
                var queryString = @"SELECT * FROM Tarea WHERE id_tablero = @boardId;";
                var tasks = new List<Tarea>();

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    var command = new SQLiteCommand(queryString, connection);
                    command.Parameters.Add(new SQLiteParameter("boardId", boardId));
                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new Tarea
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                IdTablero = Convert.ToInt32(reader["id_tablero"]),
                                Nombre = reader["nombre"].ToString(),
                                Estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                                Descripcion = reader["descripcion"].ToString(),
                                Color = reader["color"].ToString(),
                                IdUsuarioAsignado = Convert.ToInt32(reader["id_usuario_asignado"])
                            };
                            tasks.Add(task);
                        }
                    }

                    connection.Close();
                }

                return tasks;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de tareas por tablero.", ex);
            }
        }

        public List<Tarea> ListByUser(int userId)
        {
            try
            {
                var queryString = @"SELECT * FROM tarea WHERE id_usuario_asignado = @userId;";
                var tasks = new List<Tarea>();

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    var command = new SQLiteCommand(queryString, connection);
                    command.Parameters.Add(new SQLiteParameter("@userId", userId));
                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new Tarea()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                IdTablero = Convert.ToInt32(reader["id_tablero"]),
                                Nombre = reader["nombre"].ToString(),
                                Estado = (EstadoTarea)Convert.ToInt32(reader["estado"]),
                                Descripcion = reader["descripcion"].ToString(),
                                Color = reader["color"].ToString(),
                                IdUsuarioAsignado = Convert.ToInt32(reader["id_usuario_asignado"])
                            };
                            tasks.Add(task);
                        }
                    }

                    connection.Close();
                }

                return tasks;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de tareas por usuario.", ex);
            }
        }

        public void Update(int taskId, Tarea task)
        {
            try
            {
                var queryString = @"UPDATE tarea SET id_usuario_asignado = @userId, id_tablero = @boardId, nombre = @name, estado = @status, descripcion = @description, color = @color
                        WHERE id = @taskId;";

                using var connection = new SQLiteConnection(_connectionString);
                using var command = new SQLiteCommand(queryString, connection);

                connection.Open();

                command.Parameters.Add(new SQLiteParameter("@userId", task.IdUsuarioAsignado));
                command.Parameters.Add(new SQLiteParameter("@boardId", task.IdTablero));
                command.Parameters.Add(new SQLiteParameter("@name", task.Nombre));
                command.Parameters.Add(new SQLiteParameter("@status", task.Estado));
                command.Parameters.Add(new SQLiteParameter("@description", task.Descripcion));
                command.Parameters.Add(new SQLiteParameter("@color", task.Color));
                command.Parameters.Add(new SQLiteParameter("@taskId", taskId));

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la tarea.", ex);
            }
        }
    }
}
