using kanban.Models;
namespace kanban.ViewModels;

public class CrearTareaViewModel
{
    public int Id { get; set; }
    public int IdTablero { get; set; }
    public EstadoTarea Estado { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }

    public string Color { get; set; }
    public int? IdUsuarioAsignado { get; set; }
}