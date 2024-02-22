using kanban.Models;

namespace kanban.Repository;
public interface ITableroRepository
{
    public Tablero Create(Tablero board);
    public void Update(int boardId, Tablero board);
    public List<Tablero> List();
    public Tablero GetById(int boardId);
    public void Delete(int boardId);
    public List<Tablero> ListUserBoards(int userId);
    public List<Tablero> ListUserAssignedBoards(int userId);
}