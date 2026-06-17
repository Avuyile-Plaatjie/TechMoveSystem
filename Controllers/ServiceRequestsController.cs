using Microsoft.AspNetCore.Mvc;
using TechMoveSystem.Models;

namespace TechMoveSystem.Controllers;

public class ServiceRequestsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ServiceRequestsController(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;
    private HttpClient Api => _httpClientFactory.CreateClient("TechMoveApi");

    public async Task<IActionResult> Index(string searchString, string statusFilter)
    {
        ViewBag.CurrentSearch = searchString;
        ViewBag.CurrentStatus = statusFilter;
        var url = $"api/service-requests?searchString={Uri.EscapeDataString(searchString ?? "")}&statusFilter={Uri.EscapeDataString(statusFilter ?? "")}";
        var requests = await Api.GetFromJsonAsync<List<ServiceRequest>>(url) ?? new();
        return View(requests);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var request = await Api.GetFromJsonAsync<ServiceRequest>($"api/service-requests/{id}");
        return request is null ? NotFound() : View(request);
    }

    public async Task<IActionResult> Create(int? contractId)
    {
        if (contractId is null) return NotFound();
        var contract = await Api.GetFromJsonAsync<Contract>($"api/contracts/{contractId}");
        if (contract is null || contract.Status is "Expired" or "On Hold")
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
        ModelState.Remove("Contract"); ModelState.Remove("Status");
        if (!ModelState.IsValid) { ViewBag.ContractId = serviceRequest.ContractId; return View(serviceRequest); }
        var response = await Api.PostAsJsonAsync("api/service-requests", new { serviceRequest.Description, serviceRequest.CostInUsd, serviceRequest.ContractId });
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
        ViewBag.ContractId = serviceRequest.ContractId;
        return View(serviceRequest);
    }
}
