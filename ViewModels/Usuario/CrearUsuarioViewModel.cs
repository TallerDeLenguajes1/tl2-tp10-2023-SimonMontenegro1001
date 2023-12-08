using kanban.Models;
namespace kanban.ViewModels;
public class CrearUsuarioViewModel
{
    public string NombreDeUsuario { get; set; }
    public string Contrasena { get; set; }
    public Roles Rol { get; set; }
}