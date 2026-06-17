using Microsoft.AspNetCore.Mvc;
using TechMoveSystem.Models;

namespace TechMoveSystem.Controllers;

public class ClientsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ClientsController(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;
    private HttpClient Api => _httpClientFactory.CreateClient("TechMoveApi");

    public async Task<IActionResult> Index(string searchString, string regionFilter)
    {
        ViewBag.CurrentRegionFilter = regionFilter;
        ViewBag.CurrentSearch = searchString;
        var url = $"api/clients?searchString={Uri.EscapeDataString(searchString ?? "")}&regionFilter={Uri.EscapeDataString(regionFilter ?? "")}";
        var clients = await Api.GetFromJsonAsync<List<Client>>(url) ?? new();
        return View(clients);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,ContactDetails,Region")] Client client)
    {
        ModelState.Remove("Contracts");
        if (!ModelState.IsValid) return View(client);
        var response = await Api.PostAsJsonAsync("api/clients", new { client.Name, client.ContactDetails, client.Region });
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
        return View(client);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var client = await Api.GetFromJsonAsync<Client>($"api/clients/{id}");
        return client is null ? NotFound() : View(client);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactDetails,Region")] Client client)
    {
        if (id != client.Id) return NotFound();
        ModelState.Remove("Contracts");
        if (!ModelState.IsValid) return View(client);
        var response = await Api.PutAsJsonAsync($"api/clients/{id}", new { client.Name, client.ContactDetails, client.Region });
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
        return View(client);
    }
}
