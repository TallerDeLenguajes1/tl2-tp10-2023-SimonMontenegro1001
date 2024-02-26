using System.ComponentModel.DataAnnotations;
using kanban.Models;
namespace kanban.ViewModels;

public class CrearTareaViewModel
{
    [Required(ErrorMessage = "Complete el campo")]
    public int IdTablero { get; set; }
    public EstadoTarea Estado { get; set; }
    [Required(ErrorMessage = "Complete el campo")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "Complete el campo")]
    public string Descripcion { get; set; }
    [Required(ErrorMessage = "Complete el campo")]

    public string Color { get; set; }
    [Required(ErrorMessage = "Complete el campo")]
    public int? IdUsuarioAsignado { get; set; }
    public List<UsuarioDropBoxViewModel>? Usuarios { get; set; }
}