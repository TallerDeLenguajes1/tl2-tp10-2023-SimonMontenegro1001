using kanban.Models;

namespace kanban.Repository;
public interface IUsuarioRepository
{
    public void CreateUser(Usuario user);
    public void UpdateUser(int userId, Usuario user);
    public List<Usuario> ListUsers();
    public Usuario GetUserById(int userId);
    public void DeleteUser(int userId);
}