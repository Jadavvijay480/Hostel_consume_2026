using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        // ================== Admin LIST ==================
        public async Task<IActionResult> Index()
        {


            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }



            var response = await _httpClient.GetAsync("Admin");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to fetch data from API.";
                return View(new List<AdminModel>());
            }

            var data = await response.Content.ReadAsStringAsync();

            var admin = JsonConvert.DeserializeObject<List<AdminModel>>(data)
                        ?? new List<AdminModel>();

            return View(admin);
        }

        // ================== ADD Admin (GET) ==================
        public IActionResult Add()
        {
            return View(new AdminModel());
        }

        // ================== EDIT Admin (GET) ==================
        public async Task<IActionResult> Edit(int Admin_id)
        {
            var response = await _httpClient.GetAsync($"Admin/{Admin_id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var data = await response.Content.ReadAsStringAsync();

            var Admin = JsonConvert.DeserializeObject<AdminModel>(data);

            return View("Add", Admin);
        }

        // ================== ADD / UPDATE Admin (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AdminModel admin)
        {
            if (!ModelState.IsValid)
                return View(admin);

            var json = JsonConvert.SerializeObject(admin);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (admin.Admin_id == 0)
                response = await _httpClient.PostAsync("Admin", content);
            else
                response = await _httpClient.PutAsync($"Admin/{admin.Admin_id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "API Error: " + error);
                return View(admin);
            }

            // ✅ ADD THIS PART HERE
            TempData["Success"] = admin.Admin_id == 0
                ? "Admin added successfully!"
                : "Admin updated successfully!";

            return RedirectToAction("Index");
        }

        // ================== DELETE Admin ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Admin_id)
        {
            var response = await _httpClient.DeleteAsync($"Admin/ {Admin_id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";

            return RedirectToAction("Index");
        }

        // ================== EXPORT ADMIN TO EXCEL ==================
        public async Task<IActionResult> ExportToExcel()
        {
            var response = await _httpClient.GetAsync("Admin");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error fetching Admin data";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();

            var admins = JsonConvert.DeserializeObject<List<AdminModel>>(data)
                         ?? new List<AdminModel>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Admin List");

                // Header
                worksheet.Cell(1, 1).Value = "Admin ID";
                worksheet.Cell(1, 2).Value = "First Name";
                worksheet.Cell(1, 3).Value = "Last Name";
                worksheet.Cell(1, 4).Value = "Gender";
                worksheet.Cell(1, 5).Value = "Phone";
                worksheet.Cell(1, 6).Value = "Email";
                worksheet.Cell(1, 7).Value = "Address";
                worksheet.Cell(1, 8).Value = "Joining Date";
                worksheet.Cell(1, 9).Value = "Experience (Years)";
                worksheet.Cell(1, 10).Value = "Status";
                worksheet.Cell(1, 11).Value = "Created At";

                // Header Style
                var header = worksheet.Range("A1:K1");

                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightGreen;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Insert Data
                int row = 2;

                foreach (var admin in admins)
                {
                    worksheet.Cell(row, 1).Value = admin.Admin_id;
                    worksheet.Cell(row, 2).Value = admin.FirstName;
                    worksheet.Cell(row, 3).Value = admin.LastName;
                    worksheet.Cell(row, 4).Value = admin.Gender;
                    worksheet.Cell(row, 5).Value = admin.Phone;
                    worksheet.Cell(row, 6).Value = admin.Email;
                    worksheet.Cell(row, 7).Value = admin.Address;
                    worksheet.Cell(row, 8).Value = admin.JoiningDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 9).Value = admin.ExperienceYears;
                    worksheet.Cell(row, 10).Value = admin.Status ? "Active" : "Inactive";
                    worksheet.Cell(row, 11).Value = admin.CreatedAt.ToString("yyyy-MM-dd");

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "AdminList.xlsx"
                    );
                }
            }
        }




    }
}


