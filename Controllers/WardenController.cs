using ClosedXML.Excel;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

public class WardenController : Controller
{
    private readonly HttpClient _httpClient;

    public WardenController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("api");
    }

    public async Task<IActionResult> Index()
    {

        if (HttpContext.Session.GetString("UserId") == null)
        {
            return RedirectToAction("Login", "Account");
        }


        var response = await _httpClient.GetAsync("Warden");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "API Error";
            return View(new List<WardenModel>());
        }

        var data = await response.Content.ReadAsStringAsync();
        var warden = JsonConvert.DeserializeObject<List<WardenModel>>(data) ?? new List<WardenModel>();

        return View(warden);
    }

    public IActionResult Add() => View(new WardenModel());

    public async Task<IActionResult> Edit(int Warden_id)
    {
        var response = await _httpClient.GetAsync($"Warden/{Warden_id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var data = await response.Content.ReadAsStringAsync();
        var warden = JsonConvert.DeserializeObject<WardenModel>(data);

        return View("Add", warden);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(WardenModel warden)
    {
        if (!ModelState.IsValid)
            return View(warden);

        var json = JsonConvert.SerializeObject(warden);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = warden.Warden_id == 0
            ? await _httpClient.PostAsync("Warden", content)
            : await _httpClient.PutAsync($"Warden/{warden.Warden_id}", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "API Error: " + error);
            return View(warden);
        }
        TempData["Success"] = warden.Warden_id == 0
? "Warden added successfully!"
: "Warden updated successfully!";

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int Warden_id)
    {
        var response = await _httpClient.DeleteAsync($"Warden/{Warden_id}");

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Delete failed!";

        return RedirectToAction("Index");
    }


    // 🔹 🔥 PUT THIS METHOD HERE
    public async Task<JsonResult> GetWardenCount()
    {
        var response = await _httpClient.GetAsync("Warden");
        var data = await response.Content.ReadAsStringAsync();

        var warden = JsonConvert.DeserializeObject<List<WardenModel>>(data);

        return Json(warden.Count);
    }


    // ================== EXPORT WARDEN TO EXCEL ==================
    public async Task<IActionResult> ExportToExcel()
    {
        var response = await _httpClient.GetAsync("Warden");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Error fetching Warden data";
            return RedirectToAction("Index");
        }

        var data = await response.Content.ReadAsStringAsync();

        var wardens = JsonConvert.DeserializeObject<List<WardenModel>>(data)
                      ?? new List<WardenModel>();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Warden List");

            // ===== HEADER =====

            worksheet.Cell(1, 1).Value = "Warden ID";
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

            // ===== HEADER STYLE =====

            var header = worksheet.Range("A1:K1");

            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ===== INSERT DATA =====

            int row = 2;

            foreach (var warden in wardens)
            {
                worksheet.Cell(row, 1).Value = warden.Warden_id;
                worksheet.Cell(row, 2).Value = warden.FirstName;
                worksheet.Cell(row, 3).Value = warden.LastName;
                worksheet.Cell(row, 4).Value = warden.Gender;
                worksheet.Cell(row, 5).Value = warden.Phone;
                worksheet.Cell(row, 6).Value = warden.Email;
                worksheet.Cell(row, 7).Value = warden.Address;

                worksheet.Cell(row, 8).Value = warden.JoiningDate.ToString("yyyy-MM-dd");

                worksheet.Cell(row, 9).Value = warden.ExperienceYears;

                worksheet.Cell(row, 10).Value = warden.Status ? "Active" : "Inactive";

                worksheet.Cell(row, 11).Value = warden.CreatedAt.ToString("yyyy-MM-dd");

                row++;
            }

            worksheet.Columns().AdjustToContents();

            // ===== DOWNLOAD FILE =====

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);

                var content = stream.ToArray();

                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "WardenList.xlsx"
                );
            }
        }
    }

}




