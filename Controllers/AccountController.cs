using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        // ================= LOGIN GET =================
        public IActionResult Login()
        {
            return View();
        }

        // ================= LOGIN POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonConvert.SerializeObject(model);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("User/Login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid Email or Password";
                return View(model);
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            //dynamic result = JsonConvert.DeserializeObject(resultJson);
            var result = JsonConvert.DeserializeObject<ApiResponse>(resultJson);

            if (result.success)
            {
                HttpContext.Session.SetString("UserId", result.data.UserId.ToString());
                HttpContext.Session.SetString("FullName", result.data.FullName);
                HttpContext.Session.SetString("Role", result.data.Role);

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Login Failed";
            return View(model);
        }

        // ================= REGISTER GET =================
        public IActionResult Register()
        {
            return View();
        }

        // ================= REGISTER POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("User/Register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration Successful. Please Login.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Registration Failed";
            return View(model);
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }





    }
}