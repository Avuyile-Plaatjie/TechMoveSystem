using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveSystem.Data;
using TechMoveSystem.Models;
using TechMoveSystem.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TechMoveSystem.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(ApplicationDbContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        
        public async Task<IActionResult> Index(string searchString, string statusFilter)
        {
            
            var requestsQuery = _context.ServiceRequests
                .Include(s => s.Contract)
                    .ThenInclude(c => c.Client)
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(searchString))
            {
                requestsQuery = requestsQuery.Where(s => s.Description.Contains(searchString));
            }

            
            if (!string.IsNullOrEmpty(statusFilter))
            {
                requestsQuery = requestsQuery.Where(s => s.Status == statusFilter);
            }

            
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentStatus = statusFilter;

            return View(await requestsQuery.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                    .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

       
        public async Task<IActionResult> Create(int? contractId)
        {
            if (contractId == null) return NotFound();

            var contract = await _context.Contracts.FindAsync(contractId);

            
            if (contract == null || contract.Status == "Expired" || contract.Status == "On Hold")
            {
                TempData["ErrorMessage"] = "Workflow Blocked: Cannot initiate service requests against an Expired or Suspended contract framework.";
                return RedirectToAction("Index", "Contracts");
            }

            ViewBag.ContractId = contractId;
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,CostInUsd,ContractId")] ServiceRequest serviceRequest)
        {
            
            ModelState.Remove("Contract");

           
            try
            {
                decimal rate = await _currencyService.GetUsdToZarRate();
                serviceRequest.CostInZar = serviceRequest.CostInUsd * rate;
            }
            catch (Exception)
            {
                
                decimal fallbackRate = 18.50m;
                serviceRequest.CostInZar = serviceRequest.CostInUsd * fallbackRate;
            }

           
            serviceRequest.Status = "Pending";

            if (ModelState.IsValid)
            {
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.ContractId = serviceRequest.ContractId;
            return View(serviceRequest);
        }
    }
}