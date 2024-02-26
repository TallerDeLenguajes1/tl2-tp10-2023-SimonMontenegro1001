using System.ComponentModel.DataAnnotations;
using kanban.Models;

namespace kanban.ViewModels
{
    public class CambiarEstadoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Complete el campo Estado")]
        public EstadoTarea Estado { get; set; }
    }
}
