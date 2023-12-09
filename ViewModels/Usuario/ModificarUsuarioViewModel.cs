using System.ComponentModel.DataAnnotations;
using kanban.Models;
namespace kanban.ViewModels;
public class ModificarUsuarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Complete el campo")]
    public string NombreDeUsuario { get; set; }

    [Required(ErrorMessage = "Complete el campo")]
    public string Contrasena { get; set; }

    [Required(ErrorMessage = "Complete el campo")]
    public Roles Rol { get; set; }
}
