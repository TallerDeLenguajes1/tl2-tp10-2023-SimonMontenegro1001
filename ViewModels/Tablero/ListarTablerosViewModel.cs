using kanban.Repository;
namespace kanban.ViewModels;
public class ListarTablerosViewModel
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string UsuarioPropietario { get; set; }
    public int Id { get; set; }
    private readonly IUsuarioRepository usuarioRepository;

    public ListarTablerosViewModel(string nombre, string descipcion, int id, int idUsuarioPropietario)
    {
        usuarioRepository = new UsuarioRepository();
        var user = usuarioRepository.GetById(idUsuarioPropietario);
        if (user.Id == 0) UsuarioPropietario = "";
        else UsuarioPropietario = user.NombreDeUsuario;
        Nombre = nombre;
        Descripcion = descipcion;
        Id = id;
    }
}