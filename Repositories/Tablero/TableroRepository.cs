using System.Data.SQLite;
using kanban.Models;

namespace kanban.Repository;

public class TableroRepository : ITableroRepository
{
    private readonly string connectionString = "Data Source=DB/kanban.db;Cache=Shared";

    public Tablero Create(Tablero board)
    {
        var queryString = @"INSERT INTO tablero (id_usuario_propietario, nombre, descripcion) VALUES (@ownerUserId, @name, @description);";

        using var connection = new SQLiteConnection(connectionString);
        connection.Open();

        var command = new SQLiteCommand(queryString, connection);

        command.Parameters.Add(new SQLiteParameter("@ownerUserId", board.IdUsuarioPropietario));
        command.Parameters.Add(new SQLiteParameter("@name", board.Nombre));
        command.Parameters.Add(new SQLiteParameter("@description", board.Descripcion));

        command.ExecuteNonQuery();
        connection.Close();

        // wtf xd

        return board;
    }

    public void Delete(int boardId)
    {
        var queryString = @"DELETE FROM tablero WHERE id = @boardId;";

        using (SQLiteConnection connection = new(connectionString))
        {
            connection.Open();

            using (var command = new SQLiteCommand(queryString, connection))
            {
                command.Parameters.Add(new SQLiteParameter("@boardId", boardId));

                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public Tablero GetById(int boardId)
    {
        var queryString = @"SELECT * FROM tablero WHERE id = @boardId;";
        var board = new Tablero();

        using (var connection = new SQLiteConnection(connectionString))
        {
            SQLiteCommand command = new SQLiteCommand(queryString, connection);
            command.Parameters.Add(new SQLiteParameter("boardId", boardId));
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

    public List<Tablero> List()
    {
        var queryString = @"SELECT * FROM tablero;";
        var boards = new List<Tablero>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            SQLiteCommand command = new SQLiteCommand(queryString, connection);
            connection.Open();

            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Tablero tablero = new Tablero()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Nombre = reader["nombre"].ToString(),
                        Descripcion = reader["descripcion"].ToString(),
                        IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"])

                    };
                    boards.Add(tablero);
                }
            }
            connection.Close();
        }
        return boards;
    }

    public List<Tablero> ListUserBoards(int userId)
    {
        var queryString = @"SELECT * FROM tablero WHERE id_usuario_propietario = @userId;";
        var boards = new List<Tablero>();

        using (var connection = new SQLiteConnection(connectionString))
        {
            var command = new SQLiteCommand(queryString, connection);
            command.Parameters.Add(new SQLiteParameter("userId", userId));
            connection.Open();

            using (SQLiteDataReader reader = command.ExecuteReader())
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

    public void Update(int boardId, Tablero board)
    {
        var queryString = @"UPDATE Tablero SET nombre = @name, descripcion = @description, id_usuario_propietario = @ownerUserId WHERE id = @boardId;";

        using var connection = new SQLiteConnection(connectionString);
        SQLiteCommand command = new SQLiteCommand(queryString, connection);
        connection.Open();

        command.Parameters.Add(new SQLiteParameter("@name", board.Nombre));
        command.Parameters.Add(new SQLiteParameter("@description", board.Descripcion));
        command.Parameters.Add(new SQLiteParameter("@ownerUserId", board.IdUsuarioPropietario));
        command.Parameters.Add(new SQLiteParameter("@boardId", boardId));

        command.ExecuteNonQuery();
        connection.Close();
    }
}