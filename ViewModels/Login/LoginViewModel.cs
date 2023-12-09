using System.ComponentModel.DataAnnotations;
namespace kanban.ViewModels;
public class LoginViewModel
{
    [Required(ErrorMessage = "Campo requerido")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Campo requerido")]
    public string Password { get; set; }
}