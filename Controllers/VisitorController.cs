using ClosedXML.Excel;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

public class VisitorController : Controller
{
    private readonly HttpClient _httpClient;

    public VisitorController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("api");
    }

    // 🔹 GET: Visitor list
    public async Task<IActionResult> Index()
    {

        if (HttpContext.Session.GetString("UserId") == null)
        {
            return RedirectToAction("Login", "Account");
        }


        var response = await _httpClient.GetAsync("Visitor");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "API Error";
            return View(new List<VisitorModel>());
        }

        var data = await response.Content.ReadAsStringAsync();
        var visitors = JsonConvert.DeserializeObject<List<VisitorModel>>(data)
                       ?? new List<VisitorModel>();

        return View(visitors);
    }

    // 🔹 GET: Add
    public IActionResult Add()
    {
        return View(new VisitorModel
        {
            VisitDate = DateTime.Today
        });
    }

    // 🔹 GET: Edit
    public async Task<IActionResult> Edit(int Visitor_id)
    {
        var response = await _httpClient.GetAsync($"Visitor/{Visitor_id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var data = await response.Content.ReadAsStringAsync();
        var visitor = JsonConvert.DeserializeObject<VisitorModel>(data);

        return View("Add", visitor);
    }

    // 🔹 POST: Add / Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(VisitorModel visitor)
    {
        if (!ModelState.IsValid)
            return View(visitor);

        var json = JsonConvert.SerializeObject(visitor);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = visitor.Visitor_id == 0
            ? await _httpClient.PostAsync("Visitor", content)
            : await _httpClient.PutAsync($"Visitor/{visitor.Visitor_id}", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "API Error: " + error);
            return View(visitor);
        }
        TempData["Success"] = visitor.Visitor_id == 0
    ? "Visitor added successfully!"
    : "Visitor updated successfully!";

        

        return RedirectToAction("Index");
    }

    // 🔹 POST: Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int Visitor_id)
    {
        var response = await _httpClient.DeleteAsync($"Visitor/{Visitor_id}");

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Delete failed!";

        return RedirectToAction("Index");
    }


    // 🔹 🔥 PUT THIS METHOD HERE
    public async Task<JsonResult> GetVisitorCount()
    {
        var response = await _httpClient.GetAsync("Visitor");
        var data = await response.Content.ReadAsStringAsync();

        var visitor = JsonConvert.DeserializeObject<List<VisitorModel>>(data);

        return Json(visitor.Count);
    }

    // ================== EXPORT VISITOR TO EXCEL ==================
    public async Task<IActionResult> ExportToExcel()
    {
        var response = await _httpClient.GetAsync("Visitor");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Error fetching Visitor data";
            return RedirectToAction("Index");
        }

        var data = await response.Content.ReadAsStringAsync();

        var visitors = JsonConvert.DeserializeObject<List<VisitorModel>>(data)
                       ?? new List<VisitorModel>();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Visitor List");

            // ===== HEADER =====

            worksheet.Cell(1, 1).Value = "Visitor ID";
            worksheet.Cell(1, 2).Value = "Visitor Name";
            worksheet.Cell(1, 3).Value = "Gender";
            worksheet.Cell(1, 4).Value = "Phone";
            worksheet.Cell(1, 5).Value = "Email";
            worksheet.Cell(1, 6).Value = "Address";
            worksheet.Cell(1, 7).Value = "Visit Date";
            worksheet.Cell(1, 8).Value = "In Time";
            worksheet.Cell(1, 9).Value = "Out Time";
            worksheet.Cell(1, 10).Value = "Purpose";
            worksheet.Cell(1, 11).Value = "Student ID";
            worksheet.Cell(1, 12).Value = "Warden ID";
            worksheet.Cell(1, 13).Value = "Status";
            worksheet.Cell(1, 14).Value = "Created At";

            // ===== HEADER STYLE =====

            var header = worksheet.Range("A1:N1");

            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ===== INSERT DATA =====

            int row = 2;

            foreach (var visitor in visitors)
            {
                worksheet.Cell(row, 1).Value = visitor.Visitor_id;
                worksheet.Cell(row, 2).Value = visitor.VisitorName;
                worksheet.Cell(row, 3).Value = visitor.Gender;
                worksheet.Cell(row, 4).Value = visitor.Phone;
                worksheet.Cell(row, 5).Value = visitor.Email;
                worksheet.Cell(row, 6).Value = visitor.Address;

                worksheet.Cell(row, 7).Value = visitor.VisitDate.ToString("yyyy-MM-dd");

                worksheet.Cell(row, 8).Value = visitor.InTime.ToString(@"hh\:mm");

                worksheet.Cell(row, 9).Value = visitor.OutTime.HasValue
                    ? visitor.OutTime.Value.ToString(@"hh\:mm")
                    : "";

                worksheet.Cell(row, 10).Value = visitor.Purpose;
                worksheet.Cell(row, 11).Value = visitor.Student_id;
                worksheet.Cell(row, 12).Value = visitor.Warden_id;

                worksheet.Cell(row, 13).Value = visitor.Status ? "Approved" : "Pending";

                worksheet.Cell(row, 14).Value = visitor.CreatedAt.ToString("yyyy-MM-dd");

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
                    "VisitorList.xlsx"
                );
            }
        }
    }

    //GetVisitorStats
    public async Task<JsonResult> GetVisitorStats()
    {
        var response = await _httpClient.GetAsync("Visitor");
        var data = await response.Content.ReadAsStringAsync();

        var visitors = JsonConvert.DeserializeObject<List<VisitorModel>>(data);

        var totalVisitors = visitors.Count;

        var todayVisitors = visitors.Count(v => v.VisitDate.Date == DateTime.Today);

        var insideVisitors = visitors.Count(v => v.OutTime == null);

        var outsideVisitors = visitors.Count(v => v.OutTime != null);

        return Json(new
        {
            total = totalVisitors,
            today = todayVisitors,
            inside = insideVisitors,
            outside = outsideVisitors
        });
    }


}