using Microsoft.AspNetCore.Mvc;
using NomServer.Application.DTOs.Requests;
using NomServer.Application.Interfaces;

namespace NomServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, AppSettings appSettings) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var authBundle = await authService.RegisterAsync(registerRequestDto.Name, ["user"]);
        return Ok(authBundle);
    }

    [HttpPost("admin")]
    public async Task<IActionResult> Register([FromBody] RegisterAdminRequestDto registerRequestDto)
    {
        if (registerRequestDto.Key != appSettings.Application.Key) return Unauthorized();
        var authBundle = await authService.RegisterAsync(registerRequestDto.Name, ["user", "admin"]);
        return Ok(authBundle);
    }

    [HttpPost("recover")]
    public async Task<IActionResult> Recover([FromBody] RecoverRequestDto recoverRequestDto)
    {
        var authBundle = await authService.RecoverAsync(recoverRequestDto.Name, recoverRequestDto.RecoveryCode);
        return Ok(authBundle);
    }
}
