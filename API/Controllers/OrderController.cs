using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NomServer.Application.Interfaces;
using NomServer.Application.Services;
using NomServer.Core.Enums;

namespace NomServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IConfiguration configuration, IOrderService orderService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "user")]
    public async Task<IActionResult> GetAllOrdersByUsername([FromQuery] OrderEnums.OrderStatus? orderStatus)
    {
        if (User.Identity?.Name == null)
            return BadRequest("JWT token is missing a claim for user name.");

        return orderStatus.HasValue
            ? Ok(await orderService.GetAllByUsernameAndStatusAsync(User.Identity.Name, orderStatus.Value))
            : Ok(await orderService.GetAllByUserNameAsync(User.Identity.Name));
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderEnums.OrderStatus? orderStatus)
    {
        return orderStatus.HasValue
            ? Ok(await orderService.GetAllByStatusAsync(orderStatus.Value))
            : Ok(await orderService.GetAllAsync());
    }
}
