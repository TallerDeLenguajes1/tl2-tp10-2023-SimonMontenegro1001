using System.ComponentModel.DataAnnotations;
namespace kanban.ViewModels;
public class CambiarContrasenaViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Complete el campo")]
    public string ContrasenaAnterior { get; set; }
    [Required(ErrorMessage = "Complete el campo")]
    public string NuevaContrasena { get; set; }
    [Required(ErrorMessage = "Complete el campo")]
    public string ConfirmarNuevaContrasena { get; set; }
}