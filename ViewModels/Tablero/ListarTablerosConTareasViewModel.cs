namespace kanban.ViewModels;

public class ListarTablerosConTareasViewModel
{
    public string NombreTablero { get; set; }
    public bool EsPropietario { get; set; }
    public List<ListarTareasViewModel> Tareas { get; set; }
}
