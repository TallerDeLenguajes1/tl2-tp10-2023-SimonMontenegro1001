using kanban.Models;

namespace kanban.ViewModels;
public class ListarUsuariosViewModel
{
    public int Id { get; set; }
    public string NombreDeUsuario { get; set; }
    public Roles Rol {get;set;}
}