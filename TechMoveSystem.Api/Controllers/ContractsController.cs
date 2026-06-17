using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveSystem.Api.Data;
using TechMoveSystem.Api.Dtos;
using TechMoveSystem.Api.Models;

namespace TechMoveSystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ContractsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
        DateTime? startDate,
        DateTime? endDate,
        string? statusFilter)
    {
        var query = _context.Contracts
            .Include(c => c.Client)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(c => c.StartDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(c => c.EndDate <= endDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            query = query.Where(c => c.Status == statusFilter);
        }

        return Ok(await query.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Contract>> GetContract(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);

        return contract is null ? NotFound() : Ok(contract);
    }

    [HttpPost]
    public async Task<ActionResult<Contract>> CreateContract(ContractCreateDto dto)
    {
        bool clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);

        if (!clientExists)
        {
            return BadRequest("Client does not exist.");
        }

        var contract = new Contract
        {
            ClientId = dto.ClientId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            ServiceLevel = dto.ServiceLevel,
            SignedAgreementPath = dto.SignedAgreementPath
        };

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, ContractStatusUpdateDto dto)
    {
        var allowed = new[] { "Pending", "Approved", "Declined", "Active", "Expired", "On Hold" };

        if (!allowed.Contains(dto.Status))
        {
            return BadRequest("Invalid status.");
        }

        var contract = await _context.Contracts.FindAsync(id);

        if (contract is null)
        {
            return NotFound();
        }

        contract.Status = dto.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}