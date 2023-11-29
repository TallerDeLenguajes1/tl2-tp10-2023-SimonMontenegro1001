namespace kanban.Controllers.helpers;

public static class LoginHelper
{
    public static bool IsLogged(HttpContext context)
    {
        if (context.Session.GetString("id") != null)
            return true;

        return false;
    }
    public static bool IsAdmin(HttpContext context)
    {
        if (context.Session.GetString("rol") == "Administrador")
            return true;

        return false;
    }
    public static int GetUserId(HttpContext context)
    {
        var id = context.Session.GetString("id");
        if (string.IsNullOrEmpty(id)) return 0;
        return int.Parse(id);
    }

}