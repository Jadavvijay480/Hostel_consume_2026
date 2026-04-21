using ClosedXML.Excel;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class FeesViewController : Controller
{
    private readonly HttpClient _httpClient;

    public FeesViewController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("api");
    }

    // 🔹 GET: FeesView
    public async Task<IActionResult> Index()
    {

        if (HttpContext.Session.GetString("UserId") == null)
        {
            return RedirectToAction("Login", "Account");
        }


        var response = await _httpClient.GetAsync("FeesView");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "API Error";
            return View(new List<FeesViewModel>());
        }

        var data = await response.Content.ReadAsStringAsync();
        var feesView = JsonConvert.DeserializeObject<List<FeesViewModel>>(data)
                       ?? new List<FeesViewModel>();

        return View(feesView);
    }


    //GetStudentFeesLineChart

    [HttpGet]
    public async Task<IActionResult> GetStudentFeesLineChart()
    {
        var response = await _httpClient.GetAsync("FeesView");
        var jsonData = await response.Content.ReadAsStringAsync();
        var feesView = JsonConvert.DeserializeObject<List<FeesViewModel>>(jsonData);

        var chartData = feesView.Select(x => new
        {
            student = x.Student_Name,
            amount = x.Amount
        }).ToList();

        return Json(chartData);
    }

    // ================== EXPORT FEES VIEW TO EXCEL ==================
    public async Task<IActionResult> ExportToExcel()
    {
        var response = await _httpClient.GetAsync("FeesView");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Error fetching Fees Report";
            return RedirectToAction("Index");
        }

        var data = await response.Content.ReadAsStringAsync();

        var fees = JsonConvert.DeserializeObject<List<FeesViewModel>>(data)
                   ?? new List<FeesViewModel>();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Fees Report");

            // ===== HEADER =====

            worksheet.Cell(1, 1).Value = "Fee ID";
            worksheet.Cell(1, 2).Value = "Student ID";
            worksheet.Cell(1, 3).Value = "Student Name";
            worksheet.Cell(1, 4).Value = "Phone";
            worksheet.Cell(1, 5).Value = "Email";
            worksheet.Cell(1, 6).Value = "Amount";
            worksheet.Cell(1, 7).Value = "Fee Month";
            worksheet.Cell(1, 8).Value = "Due Date";
            worksheet.Cell(1, 9).Value = "Status";
            worksheet.Cell(1, 10).Value = "Room ID";
            worksheet.Cell(1, 11).Value = "Room Number";
            worksheet.Cell(1, 12).Value = "Room Type";

            // ===== STYLE =====

            var header = worksheet.Range("A1:L1");

            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ===== DATA =====

            int row = 2;

            foreach (var item in fees)
            {
                worksheet.Cell(row, 1).Value = item.Fee_id;
                worksheet.Cell(row, 2).Value = item.Student_id;
                worksheet.Cell(row, 3).Value = item.Student_Name;
                worksheet.Cell(row, 4).Value = item.Phone;
                worksheet.Cell(row, 5).Value = item.Email;

                worksheet.Cell(row, 6).Value = item.Amount;
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "₹ #,##0";

                worksheet.Cell(row, 7).Value = item.Fee_month;
                worksheet.Cell(row, 8).Value = item.Due_date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 9).Value = item.Status;

                worksheet.Cell(row, 10).Value = item.Room_id;
                worksheet.Cell(row, 11).Value = item.Room_number;
                worksheet.Cell(row, 12).Value = item.Room_type;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            // ===== DOWNLOAD =====

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);

                var content = stream.ToArray();

                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "FeesReport.xlsx"
                );
            }
        }
    }





}