using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechMoveSystem.Data;
using TechMoveSystem.Models;

namespace TechMoveSystem.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(string searchString, string regionFilter)
        {
            var clients = from c in _context.Clients select c;

            
            if (!string.IsNullOrEmpty(searchString))
            {
                clients = clients.Where(s => s.Name.Contains(searchString));
            }

           
            if (!string.IsNullOrEmpty(regionFilter))
            {
                clients = clients.Where(x => x.Region == regionFilter);
            }

            
            ViewBag.CurrentRegionFilter = regionFilter;
            ViewBag.CurrentSearch = searchString;

            return View(await clients.ToListAsync());
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ContactDetails,Region")] Client client)
        {
           
            ModelState.Remove("Contracts");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    
                    ModelState.AddModelError("", $"Database Error: Unable to save changes. {ex.Message}");
                }
            }

            
            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Failure Target: {error.ErrorMessage}");
                }
            }

            return View(client);
        }

       
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);

          
            if (client == null)
            {
                TempData["ErrorMessage"] = $"System Routing Mismatch: Client record with ID #{id} does not exist in the ledger database context.";
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            
            ModelState.Remove("Contracts");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                    if (!_context.Clients.Any(e => e.Id == client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Concurrency Error: Unable to save edit modifications. {ex.Message}");
                }
            }
            return View(client);
        }
    }
}