using kanban.Repository;
namespace kanban.ViewModels;
public class ListarTablerosViewModel
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string UsuarioPropietario { get; set; }
    public int Id { get; set; }
    private readonly IUsuarioRepository _usuarioRepository;

    public ListarTablerosViewModel(string nombre, string descipcion, int id, int idUsuarioPropietario, IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
        var user = _usuarioRepository.GetById(idUsuarioPropietario);
        if (user.Id == 0) UsuarioPropietario = "";
        else UsuarioPropietario = user.NombreDeUsuario;
        Nombre = nombre;
        Descripcion = descipcion;
        Id = id;
    }
}