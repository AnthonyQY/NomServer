using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NomServer.Application.DTOs.Requests;
using NomServer.Application.DTOs.Responses;
using NomServer.Application.Interfaces;
using NomServer.Core.Entities;

namespace NomServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemController(IMapper mapper, IMenuItemService menuItemService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> GetMenuItems()
    {
        var menuItems = await menuItemService.GetAllAsync();
        return Ok(mapper.Map<List<MenuItemResponseDto>>(menuItems));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "user,admin")]
    public async Task<IActionResult> GetMenuItem(string id)
    {
        var menuItem = await menuItemService.GetByIdAsync(id);
        if (menuItem == null) return NotFound();
        return Ok(mapper.Map<MenuItemResponseDto>(menuItem));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemRequestDto menuItemRequestDto)
    {
        var menuItem = await menuItemService.CreateAsync(mapper.Map<MenuItem>(menuItemRequestDto));
        return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, mapper.Map<MenuItemResponseDto>(menuItem));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateMenuItem(string id, [FromBody] MenuItemRequestDto menuItemRequestDto)
    {
        var updated = await menuItemService.UpdateAsync(id, mapper.Map<MenuItem>(menuItemRequestDto));
        if (updated == null) return NotFound();
        return Ok(mapper.Map<MenuItemResponseDto>(updated));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteMenuItem(string id)
    {
        var deleted = await menuItemService.DeleteByIdAsync(id);
        if (deleted == null) return NotFound();
        return Ok(mapper.Map<MenuItemResponseDto>(deleted));
    }
}
