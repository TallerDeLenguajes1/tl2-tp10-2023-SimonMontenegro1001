namespace kanban.Models;

public enum Roles
{
    Operador,
    Administrador
}

public class Usuario
{
    public int Id { get; set; }
    public string NombreDeUsuario { get; set; }
    public string Contrasena { get; set; }
    public Roles Rol { get; set; }
}
