using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
namespace tl2_tp6_2024_s0a0m.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AccessLevelAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredAccessLevels;

    // El constructor ahora recibe un array de niveles de acceso permitidos
    public AccessLevelAuthorizeAttribute(params string[] requiredAccessLevels)
    {
        _requiredAccessLevels = requiredAccessLevels;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userAccessLevel = GetUserAccessLevel(context);

        // Primero verificamos si el usuario está autenticado
        if (!IsAuthenticated(context))
        {
            // Si no está autenticado, redirigir a la página de login
            context.Result = new RedirectToActionResult("Index", "Usuario", null);
            return;
        }

        // Si el usuario está autenticado, verificamos si tiene el nivel de acceso adecuado
        if (_requiredAccessLevels == null || !_requiredAccessLevels.Contains(userAccessLevel))
        {
            // Redirigir a la vista personalizada 403 en vez de solo devolver 403
            context.Result = new RedirectToActionResult("Error403", "Error", null);
            return;
        }

        // Si está autenticado y tiene el acceso adecuado, la acción continuará.
    }

    private static string? GetUserAccessLevel(AuthorizationFilterContext context) => context.HttpContext.Session.GetString("AccessLevel");

    private static bool IsAuthenticated(AuthorizationFilterContext context) => context.HttpContext.Session.GetString("IsAuthenticated") == "true";
}