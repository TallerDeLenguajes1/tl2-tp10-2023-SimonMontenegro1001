using System.ComponentModel.DataAnnotations;
using kanban.Models;

namespace kanban.ViewModels
{
    public class EditarTareaViewModel
    {
        public int Id { get; set; }

        public int IdTablero { get; set; }

        [Required(ErrorMessage = "Complete el campo Estado")]
        public EstadoTarea Estado { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string Color { get; set; }

        public int? IdUsuarioAsignado { get; set; }

        public List<UsuarioDropBoxViewModel>? Usuarios { get; set; }

        public List<TableroDropBoxViewModel>? Tableros { get; set; }
    }
}
