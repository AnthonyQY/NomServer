using Microsoft.AspNetCore.Mvc;
using NomServer.Application.DTOs.Requests;
using NomServer.Application.Interfaces;

namespace NomServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var authBundle = await authService.RegisterAsync(request.Name, ["user", "admin"]);
        return Ok(authBundle);
    }

    [HttpPost("recover")]
    public async Task<IActionResult> Recover([FromBody] RecoverRequestDto request)
    {
        var authBundle = await authService.RecoverAsync(request.Name, request.RecoveryCode);
        return Ok(authBundle);
    }
}
