using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveSystem.Api.Data;
using TechMoveSystem.Api.Dtos;
using TechMoveSystem.Api.Models;
using TechMoveSystem.Api.Services;

namespace TechMoveSystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/service-requests")]
public class ServiceRequestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CurrencyService _currencyService;
    public ServiceRequestsController(ApplicationDbContext context, CurrencyService currencyService)
    {
        _context = context;
        _currencyService = currencyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetRequests(string? searchString, string? statusFilter)
    {
        var query = _context.ServiceRequests.Include(s => s.Contract).ThenInclude(c => c.Client).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchString)) query = query.Where(s => s.Description.Contains(searchString));
        if (!string.IsNullOrWhiteSpace(statusFilter)) query = query.Where(s => s.Status == statusFilter);
        return Ok(await query.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceRequest>> GetRequest(int id)
    {
        var request = await _context.ServiceRequests.Include(s => s.Contract).ThenInclude(c => c.Client).FirstOrDefaultAsync(s => s.Id == id);
        return request is null ? NotFound() : Ok(request);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceRequest>> CreateRequest(ServiceRequestCreateDto dto)
    {
        var contract = await _context.Contracts.FindAsync(dto.ContractId);
        if (contract is null) return BadRequest("Contract does not exist.");
        if (contract.Status is "Expired" or "On Hold") return BadRequest("Cannot initiate service requests against an expired or suspended contract.");
        var rate = await _currencyService.GetUsdToZarRate();
        var request = new ServiceRequest
        {
            Description = dto.Description,
            CostInUsd = dto.CostInUsd,
            CostInZar = dto.CostInUsd * rate,
            ContractId = dto.ContractId,
            Status = "Pending"
        };
        _context.ServiceRequests.Add(request);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, request);
    }
}
