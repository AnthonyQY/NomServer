using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NomServer.Application.DTOs.Responses;
using NomServer.Application.Interfaces;
using NomServer.Core.Entities;
using NomServer.Core.Enums;
using AutoMapper;
using NomServer.Application.DTOs.Requests;

namespace NomServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> GetOrders([FromQuery] string? username, [FromQuery] OrderEnums.OrderStatus? orderStatus)
    {
        List<OrderResponseDto> orders;

        if (User.IsInRole("admin"))
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                orders = orderStatus.HasValue
                    ? mapper.Map<List<OrderResponseDto>>(await orderService.GetAllByUsernameAndStatusAsync(username, orderStatus.Value))
                    : mapper.Map<List<OrderResponseDto>>(await orderService.GetAllByUserNameAsync(username));
            }
            else
            {
                orders = orderStatus.HasValue
                    ? mapper.Map<List<OrderResponseDto>>(await orderService.GetAllByStatusAsync(orderStatus.Value))
                    : mapper.Map<List<OrderResponseDto>>(await orderService.GetAllAsync());
            }
        }
        else
        {
            if (User.Identity?.Name == null)
                return BadRequest("JWT token is missing a claim for user name.");

            orders = orderStatus.HasValue
                ? mapper.Map<List<OrderResponseDto>>(await orderService.GetAllByUsernameAndStatusAsync(User.Identity.Name, orderStatus.Value))
                : mapper.Map<List<OrderResponseDto>>(await orderService.GetAllByUserNameAsync(User.Identity.Name));
        }

        return Ok(orders);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var order = await orderService.GetByIdAsync(id);
        if (order == null) return NotFound();

        if (!User.IsInRole("admin") && order.Username != User.Identity?.Name)
            return Forbid();

        return Ok(mapper.Map<OrderResponseDto>(order));
    }

    [HttpPost]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto orderRequestDto)
    {
        var order = mapper.Map<Order>(orderRequestDto);
        order.Status = (int) OrderEnums.OrderStatus.Placed;
        order.DatePlaced = DateTime.UtcNow;
        order.Username = User.Identity?.Name ?? throw new InvalidOperationException("User identity missing");
        var created = await orderService.CreateAsync(order);
        return CreatedAtAction(nameof(GetOrder), new { id = created.Id }, mapper.Map<OrderResponseDto>(created));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateOrder(string id, [FromBody] OrderRequestDto orderRequestDto)
    {
        var updated = await orderService.UpdateAsync(id, mapper.Map<Order>(orderRequestDto));
        if (updated == null) return NotFound();
        return Ok(mapper.Map<OrderResponseDto>(updated));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> DeleteOrder(string id)
    {
        var order = await orderService.GetByIdAsync(id);
        if (order == null) return NotFound();

        if (!User.IsInRole("admin") && order.Username != User.Identity?.Name)
            return Forbid();

        var deleted = await orderService.DeleteByIdAsync(id);
        return Ok(mapper.Map<OrderResponseDto>(deleted));
    }
}