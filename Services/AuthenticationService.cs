
using tl2_tp6_2024_s0a0m.Repositorios;

public interface IAuthenticationService
{
    bool Login(string username, string password);
    void Logout();
    bool IsAuthenticated();
}


public class AuthenticationService : IAuthenticationService
{
    private readonly IUsuarioRepository _userR;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpContext context;

    public AuthenticationService(IUsuarioRepository userR, IHttpContextAccessor httpContextAccessor)
    {
        _userR = userR;
        _httpContextAccessor = httpContextAccessor;
        context = _httpContextAccessor.HttpContext;
    }

    public bool Login(string username, string password)
    {

        var user = _userR.Obtener(username, password);
        if (user.IdUsuario != 0)
        {
            context.Session.SetString("IsAuthenticated", "true");
            context.Session.SetString("User", username);
            context.Session.SetString("AccessLevel", user.Rol.ToString());
            return true;
        }

        return false;
    }

    public void Logout()
    {
        context.Session.Remove("IsAuthenticated");
        context.Session.Remove("User");
        context.Session.Remove("AccessLevel");
    }

    public bool IsAuthenticated()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context == null)
        {
            throw new InvalidOperationException("HttpContext no está disponible.");
        }

        return context.Session.GetString("IsAuthenticated") == "true";
    }
}