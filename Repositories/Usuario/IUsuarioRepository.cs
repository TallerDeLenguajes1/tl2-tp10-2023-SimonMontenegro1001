using kanban.Models;

namespace kanban.Repository;
public interface IUsuarioRepository
{
    public void Create(Usuario user);
    public void Update(int userId, Usuario user);
    public List<Usuario> List();
    public Usuario GetById(int userId);
    public void Delete(int userId);
}