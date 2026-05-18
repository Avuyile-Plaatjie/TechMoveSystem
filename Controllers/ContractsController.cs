using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveSystem.Data;
using TechMoveSystem.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TechMoveSystem.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string statusFilter)
        {
            var contractsQuery = _context.Contracts.Include(c => c.Client).AsQueryable();

            
            if (startDate.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.StartDate >= startDate.Value);
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            }

           
            if (endDate.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.EndDate <= endDate.Value);
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            }

           
            if (!string.IsNullOrEmpty(statusFilter))
            {
                contractsQuery = contractsQuery.Where(c => c.Status == statusFilter);
                ViewBag.StatusFilter = statusFilter;
            }

            return View(await contractsQuery.ToListAsync());
        }

        
        public IActionResult Create()
        {
            
            ViewBag.ClientId = new SelectList(_context.Clients.ToList(), "Id", "Name");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract, IFormFile pdfFile)
        {
            
            ModelState.Remove("Client");
            ModelState.Remove("ServiceRequests");
            ModelState.Remove("SignedAgreementPath");

            if (pdfFile != null && pdfFile.Length > 0)
            {
                
                var fileExtension = Path.GetExtension(pdfFile.FileName).ToLower();
                if (fileExtension == ".pdf")
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/contracts");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(pdfFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(fileStream);
                    }

                    contract.SignedAgreementPath = "/uploads/contracts/" + fileName;
                }
                else
                {
                    ModelState.AddModelError("", "File Validation Failure: Only PDF uploads are permitted.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Assignment Requirement: You must upload a signed PDF agreement to register a contract.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.ClientId = new SelectList(_context.Clients.ToList(), "Id", "Name", contract.ClientId);
            return View(contract);
        }
    }
}