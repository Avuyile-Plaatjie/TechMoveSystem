using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveSystem.Api.Data;
using TechMoveSystem.Api.Dtos;
using TechMoveSystem.Api.Models;

namespace TechMoveSystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Client>>> GetClients(
        string? searchString,
        string? regionFilter)
    {
        var query = _context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(c => c.Name.Contains(searchString));
        }

        if (!string.IsNullOrWhiteSpace(regionFilter))
        {
            query = query.Where(c => c.Region == regionFilter);
        }

        return Ok(await query.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        return client is null ? NotFound() : Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> CreateClient(ClientCreateDto dto)
    {
        var client = new Client
        {
            Name = dto.Name,
            ContactDetails = dto.ContactDetails,
            Region = dto.Region
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateClient(int id, ClientCreateDto dto)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client is null)
        {
            return NotFound();
        }

        client.Name = dto.Name;
        client.ContactDetails = dto.ContactDetails;
        client.Region = dto.Region;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}