using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using ClosedXML.Excel;

public class FeesController : Controller
{
    private readonly HttpClient _httpClient;

    public FeesController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("api");
    }




    // 🔹 GET: Fees/ViewModel (Student + Room + Fees)
    //public async Task<IActionResult> FeesView()
    //{
    //    var response = await _httpClient.GetAsync("api/Fees/fees-view");

    //    if (!response.IsSuccessStatusCode)
    //    {
    //        ViewBag.Error = "API Error";
    //        return View(new List<FeesViewModel>());
    //    }

    //    var data = await response.Content.ReadAsStringAsync();

    //    var feesView = JsonConvert.DeserializeObject<List<FeesViewModel>>(data)
    //                   ?? new List<FeesViewModel>();

    //    return View(feesView);
    //}






    // 🔹 GET: Fees
    public async Task<IActionResult> Index()
    {

        if (HttpContext.Session.GetString("UserId") == null)
        {
            return RedirectToAction("Login", "Account");
        }


        var response = await _httpClient.GetAsync("Fees");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "API Error";
            return View(new List<FeesModel>());
        }

        var data = await response.Content.ReadAsStringAsync();
        var fees = JsonConvert.DeserializeObject<List<FeesModel>>(data)
                   ?? new List<FeesModel>();

        return View(fees);
    }

    // 🔹 GET: Fees/Add
    public IActionResult Add() => View(new FeesModel());

    // 🔹 GET: Fees/Edit/5
    public async Task<IActionResult> Edit(int Fee_id)
    {
        var response = await _httpClient.GetAsync($"Fees/{Fee_id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var data = await response.Content.ReadAsStringAsync();
        var fees = JsonConvert.DeserializeObject<FeesModel>(data);

        return View("Add", fees);
    }

    // ================== ADD / UPDATE FEES (POST) ==================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(FeesModel fees)
    {
        if (!ModelState.IsValid)
            return View(fees);

        var json = JsonConvert.SerializeObject(fees);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;

        if (fees.Fee_id == 0)
            response = await _httpClient.PostAsync("Fees", content);
        else
            response = await _httpClient.PutAsync($"Fees/{fees.Fee_id}", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "API Error: " + error);
            return View(fees);
        }
        TempData["Success"] = fees.Fee_id == 0
         ? "Fee added successfully!"
         : "Fee updated successfully!";

        

        return RedirectToAction("Index");
    }


    // 🔹 POST: Fees/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int Fee_id)
    {
        var response = await _httpClient.DeleteAsync($"Fees/{Fee_id}");

        if (!response.IsSuccessStatusCode)
            TempData["Error"] = "Delete failed!";

        return RedirectToAction("Index");
    }







    //==================  TotalFeesAmount ==================
    public async Task<JsonResult> GetTotalFeesAmount()
    {
        var response = await _httpClient.GetAsync("Fees");
        var data = await response.Content.ReadAsStringAsync();

        var fees = JsonConvert.DeserializeObject<List<FeesModel>>(data);

        var totalAmount = fees.Sum(f => f.Amount); // 👈 Make sure property name is correct

        return Json(totalAmount);
    }

  
     //================== EXPORT FEES TO EXCEL ==================
    public async Task<IActionResult> ExportToExcel()
    {
        var response = await _httpClient.GetAsync("Fees");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Error fetching Fees data";
            return RedirectToAction("Index");
        }

        var data = await response.Content.ReadAsStringAsync();

        var feesList = JsonConvert.DeserializeObject<List<FeesModel>>(data)
                       ?? new List<FeesModel>();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Fees List");

            // Header
            worksheet.Cell(1, 1).Value = "Fee ID";
            worksheet.Cell(1, 2).Value = "Student ID";
            worksheet.Cell(1, 3).Value = "Amount";
            worksheet.Cell(1, 4).Value = "Fee Month";
            worksheet.Cell(1, 5).Value = "Due Date";
            worksheet.Cell(1, 6).Value = "Status";

            // Header Style
            var header = worksheet.Range("A1:F1");

            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightBlue;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Insert Data
            int row = 2;

            foreach (var fee in feesList)
            {
                worksheet.Cell(row, 1).Value = fee.Fee_id;
                worksheet.Cell(row, 2).Value = fee.Student_id;
                worksheet.Cell(row, 3).Value = fee.Amount;
                worksheet.Cell(row, 4).Value = fee.Fee_month;
                worksheet.Cell(row, 5).Value = fee.Due_date.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 6).Value = fee.Status;

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
                    "FeesList.xlsx"
                );
            }
        }
    }

    //line charts

    public async Task<JsonResult> GetFeesChartData()
    {
        var response = await _httpClient.GetAsync("Fees");

        var data = await response.Content.ReadAsStringAsync();

        var fees = JsonConvert.DeserializeObject<List<FeesModel>>(data);


        var result = fees
            .GroupBy(f => f.Fee_month)
            .Select(g => new
            {
                month = g.Key,
                total = g.Sum(x => x.Amount)
            })
            .OrderBy(x => x.month)
            .ToList();


        var labels = result.Select(x => x.month).ToList();

        var values = result.Select(x => x.total).ToList();


        return Json(new { labels, values });

    }

    //barcharts
    public async Task<JsonResult> GetFeesAmountByStatus()
    {
        var response = await _httpClient.GetAsync("Fees");
        var data = await response.Content.ReadAsStringAsync();

        var fees = JsonConvert.DeserializeObject<List<FeesModel>>(data);

        var result = fees
            .GroupBy(f => f.Status)
            .Select(g => new
            {
                status = g.Key,
                totalAmount = g.Sum(x => x.Amount)
            })
            .ToList();

        return Json(result);
    }




}