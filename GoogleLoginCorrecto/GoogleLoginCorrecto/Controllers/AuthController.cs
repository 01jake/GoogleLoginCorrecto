using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public record UserInfo(string Name, string Email, bool IsAuthenticated);

    [HttpGet("user")]
    public ActionResult<UserInfo> GetUser()
    {
        // HttpContext.User es el objeto que representa al usuario de la sesión actual (cookie).
        if (User.Identity?.IsAuthenticated == true)
        {
            // El usuario SÍ está autenticado. Devolvemos sus datos.
            var authenticatedUser = new UserInfo(
                User.Identity.Name ?? "Anónimo",
                User.FindFirstValue(ClaimTypes.Email) ?? "",
                true
            );
            return Ok(authenticatedUser);
        }

        // El usuario NO está autenticado. Devolvemos un objeto que lo indica.
        // NO devolvemos un 401, porque eso puede causar problemas. Simplemente informamos el estado.
        var anonymousUser = new UserInfo("", "", false);
        return Ok(anonymousUser);
    }
}