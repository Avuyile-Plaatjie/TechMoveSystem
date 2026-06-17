using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveSystem.Models;
using TechMoveSystem.Services.Api;

namespace TechMoveSystem.Controllers;

public class ContractsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHostEnvironment _environment;
    public ContractsController(IHttpClientFactory httpClientFactory, IWebHostEnvironment environment)
    {
        _httpClientFactory = httpClientFactory;
        _environment = environment;
    }
    private HttpClient Api => _httpClientFactory.CreateClient("TechMoveApi");

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string statusFilter)
    {
        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
        ViewBag.StatusFilter = statusFilter;
        var url = $"api/contracts?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&statusFilter={Uri.EscapeDataString(statusFilter ?? "")}";
        var contracts = await Api.GetFromJsonAsync<List<Contract>>(url) ?? new();
        return View(contracts);
    }

    public async Task<IActionResult> Create()
    {
        var clients = await Api.GetFromJsonAsync<List<Client>>("api/clients") ?? new();
        ViewBag.ClientId = new SelectList(clients, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract, IFormFile pdfFile)
    {
        ModelState.Remove("Client"); ModelState.Remove("ServiceRequests"); ModelState.Remove("SignedAgreementPath");
        if (pdfFile is null || pdfFile.Length == 0) ModelState.AddModelError("", "You must upload a signed PDF agreement.");
        if (pdfFile is not null && Path.GetExtension(pdfFile.FileName).ToLower() != ".pdf") ModelState.AddModelError("", "Only PDF uploads are permitted.");

        if (!ModelState.IsValid)
        {
            var clients = await Api.GetFromJsonAsync<List<Client>>("api/clients") ?? new();
            ViewBag.ClientId = new SelectList(clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "contracts");
        Directory.CreateDirectory(uploadsFolder);
        var fileName = Guid.NewGuid() + "_" + Path.GetFileName(pdfFile!.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);
        await using (var fileStream = new FileStream(filePath, FileMode.Create)) await pdfFile.CopyToAsync(fileStream);
        contract.SignedAgreementPath = "/uploads/contracts/" + fileName;

        var response = await Api.PostAsJsonAsync("api/contracts", new { contract.ClientId, contract.StartDate, contract.EndDate, contract.Status, contract.ServiceLevel, contract.SignedAgreementPath });
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
        var clientsAgain = await Api.GetFromJsonAsync<List<Client>>("api/clients") ?? new();
        ViewBag.ClientId = new SelectList(clientsAgain, "Id", "Name", contract.ClientId);
        return View(contract);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var response = await Api.PatchAsJsonAsync($"api/contracts/{id}/status", new ContractStatusUpdateDto(status));
        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode ? "Contract status updated." : await response.Content.ReadAsStringAsync();
        return RedirectToAction(nameof(Index));
    }
}
