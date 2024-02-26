using kanban.Models;

namespace kanban.Repository;
public interface ITareaRepository
{
    public void Create(int boardId, Tarea task);
    public void Update(int taskId, Tarea task);
    public Tarea GetById(int taskId);
    public List<Tarea> ListByUser(int userId);
    public List<Tarea> ListByBoard(int boardId);
    public void Delete(int taskId);
    public void AssignUser(int userId, int taskId);
    List<Tarea> ListByUserAndBoard(int userId, int boardId);
    List<Tarea> List();

}