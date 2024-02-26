using System.ComponentModel.DataAnnotations;
namespace kanban.ViewModels;
public class CrearTableroViewModel
{
    [Required(ErrorMessage = "Campo requerido")]
    public int IdUsuarioPropietario { get; set; }
    
    [Required(ErrorMessage = "Campo requerido")]
    public string Nombre { get; set; }
    
    [Required(ErrorMessage = "Campo requerido")]
    public string Descripcion { get; set; }
    public List<UsuarioDropBoxViewModel>? Usuarios { get; set; }
}

