using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class ProfileController : Controller
{
    private readonly HttpClient _httpClient;

    public ProfileController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("api");
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var response = await _httpClient.GetAsync("Student");

        if (!response.IsSuccessStatusCode)
            return View(new List<StudentModel>());

        var data = await response.Content.ReadAsStringAsync();
        var students = JsonConvert.DeserializeObject<List<StudentModel>>(data);

        return View(students);
    }
}