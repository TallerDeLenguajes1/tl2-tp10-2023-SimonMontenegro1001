using kanban.Models;
namespace kanban.ViewModels;
public class ModificarUsuarioViewModel
{
    public int Id { get; set; }
    public string NombreDeUsuario { get; set; }
    public string Contrasena { get; set; }
    public Roles Rol { get; set; }
}
