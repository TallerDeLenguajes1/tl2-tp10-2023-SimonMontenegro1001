
namespace kanban.ViewModels;
public class TableroDropBoxViewModel
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        
        public TableroDropBoxViewModel(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }