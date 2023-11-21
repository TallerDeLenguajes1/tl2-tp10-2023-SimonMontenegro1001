using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string connectionString = "Data Source=DB/kanban.db;Cache=Shared";
    public void CreateUser(Usuario user)
    {
        var query = $"INSERT INTO usuario (nombre_de_usuario) VALUES (@username)";
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        var command = new SQLiteCommand(query, connection);

        command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));

        command.ExecuteNonQuery();

        connection.Close();
    }

    public void DeleteUser(int userId)
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM usuario WHERE Id = @userId";
        command.Parameters.Add(new SQLiteParameter("@userId", userId));
        command.ExecuteNonQuery();
    }

    public Usuario GetUserById(int userId)
    {

        var user = new Usuario();

        using (var connection = new SQLiteConnection(connectionString))
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
                }
            }
            connection.Close();
        }
        return user;
    }

    public List<Usuario> ListUsers()
    {
        var queryString = @"SELECT * FROM usuario;";
        var users = new List<Usuario>();
        using (var connection = new SQLiteConnection(connectionString))
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
                        NombreDeUsuario = reader["nombre_de_usuario"].ToString()
                    };
                    users.Add(user);
                }
            }
            connection.Close();
        }
        return users;
    }

    public void UpdateUser(int userId, Usuario user)
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE usuario SET nombre_de_usuario = @username WHERE id = @userId";
        command.Parameters.Add(new SQLiteParameter("@username", user.NombreDeUsuario));
        command.Parameters.Add(new SQLiteParameter("@userId", userId));
        command.ExecuteNonQuery();
    }

}