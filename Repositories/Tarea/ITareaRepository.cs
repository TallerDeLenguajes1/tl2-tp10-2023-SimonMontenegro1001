using kanban.Models;

namespace kanban.Repository;
public interface ITareaRepository
{
    public void CreateTask(int boardId, Tarea task);
    public void UpdateTask(int taskId, Tarea task);
    public Tarea GetTaskById(int taskId);
    public List<Tarea> ListTasksByUser(int userId);
    public List<Tarea> ListTasksByBoard(int boardId);
    public void DeleteTask(int taskId);
    public void AssignUserToTask(int userId, int taskId);
}