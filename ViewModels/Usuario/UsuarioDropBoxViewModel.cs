
namespace kanban.ViewModels;
public class UsuarioDropBoxViewModel
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        
        public UsuarioDropBoxViewModel(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }