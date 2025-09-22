using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NomServer.Application.DTOs;
using NomServer.Application.Interfaces;
using NomServer.Application.Services;

namespace NomServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var authBundle = await authService.RegisterAsync(request.Name);
        return Ok(authBundle);
    }

    [HttpPost("recover")]
    public async Task<IActionResult> Recover([FromBody] RecoverDto request)
    {
        var authBundle = await authService.RecoverAsync(request.Name, request.RecoveryCode);
        return Ok(authBundle);
    }
}
