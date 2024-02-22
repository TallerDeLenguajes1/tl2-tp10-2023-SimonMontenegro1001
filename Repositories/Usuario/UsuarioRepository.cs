using System;
using System.Collections.Generic;
using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string? _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Usuario user)
        {
            try
            {
                var query = $"INSERT INTO usuario (nombre_de_usuario, contrasena, rol) VALUES (@username, @contrasena, @rol)";
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                var command = new SQLiteCommand(query, connection);

                command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));
                command.Parameters.Add(new SQLiteParameter("@contrasena", user.Contrasena));
                command.Parameters.Add(new SQLiteParameter("@rol", user.Rol));

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el usuario.", ex);
            }
        }

        public void Delete(int userId)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM usuario WHERE Id = @userId";
                command.Parameters.Add(new SQLiteParameter("@userId", userId));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el usuario.", ex);
            }
        }

        public Usuario GetById(int userId)
        {
            try
            {
                var user = new Usuario();

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM usuario WHERE id = @userId";
                    command.Parameters.Add(new SQLiteParameter("@userId", userId));
                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Id = Convert.ToInt32(reader["id"]);
                            user.NombreDeUsuario = reader["nombre_de_usuario"].ToString();
                            user.Rol = (Roles)Convert.ToInt32(reader["rol"]);
                            user.Contrasena = reader["contrasena"].ToString();
                        }
                    }

                    connection.Close();
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el usuario por ID.", ex);
            }
        }

        public List<Usuario> List()
        {
            try
            {
                var queryString = @"SELECT * FROM usuario;";
                var users = new List<Usuario>();
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    var command = new SQLiteCommand(queryString, connection);
                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new Usuario
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                NombreDeUsuario = reader["nombre_de_usuario"].ToString(),
                                Rol = (Roles)Convert.ToInt32(reader["rol"])
                            };
                            users.Add(user);
                        }
                    }

                    connection.Close();
                }

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de usuarios.", ex);
            }
        }

        public void Update(int userId, Usuario user)
        {
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE usuario SET nombre_de_usuario = @username, contrasena = @contrasena, rol = @rol WHERE id = @userId";
                command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));
                command.Parameters.Add(new SQLiteParameter("@contrasena", user.Contrasena));
                command.Parameters.Add(new SQLiteParameter("@rol", user.Rol));
                command.Parameters.Add(new SQLiteParameter("@userId", userId));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el usuario.", ex);
            }
        }

        public Usuario GetByUsername(string username)
        {
            try
            {
                var user = new Usuario();

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM usuario WHERE nombre_de_usuario = @nombre_de_usuario";
                    command.Parameters.Add(new SQLiteParameter("@nombre_de_usuario", username));
                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Id = Convert.ToInt32(reader["id"]);
                            user.NombreDeUsuario = reader["nombre_de_usuario"].ToString();
                            user.Rol = (Roles)Convert.ToInt32(reader["rol"]);
                            user.Contrasena = reader["contrasena"].ToString();
                        }
                    }

                    connection.Close();
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el usuario por nombre de usuario.", ex);
            }
        }
    }
}
