using ClosedXML.Excel;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly HttpClient _httpClient;

        public ComplaintController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        // ================== Complaint LIST ==================
        public async Task<IActionResult> Index()
        {


            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }



            var response = await _httpClient.GetAsync("Complaint");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to fetch data from API.";
                return View(new List<ComplaintModel>());
            }

            var data = await response.Content.ReadAsStringAsync();

            var complaint = JsonConvert.DeserializeObject<List<ComplaintModel>>(data)
                        ?? new List<ComplaintModel>();

            return View(complaint);
        }

        // ================== ADD Complaint (GET) ==================
        public IActionResult Add()
        {
            return View(new ComplaintModel());
        }

        // ================== EDIT Complaint (GET) ==================
        public async Task<IActionResult> Edit(int Complaint_id)
        {
            var response = await _httpClient.GetAsync($"Complaint/{Complaint_id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var data = await response.Content.ReadAsStringAsync();

            var complaint = JsonConvert.DeserializeObject<ComplaintModel>(data);

            return View("Add", complaint);
        }

        // ================== ADD / UPDATE Complaint (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ComplaintModel complaint)
        {
            if (!ModelState.IsValid)
                return View(complaint);

            var json = JsonConvert.SerializeObject(complaint);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (complaint.Complaint_id == 0)
                response = await _httpClient.PostAsync("Complaint", content);
            else
                response = await _httpClient.PutAsync($"Complaint/{complaint.Complaint_id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "API Error: " + error);
                return View(complaint);
            }

            TempData["Success"] = complaint.Complaint_id == 0
              ? "Complaint added successfully!"
              : "Complaint updated successfully!";

            

            return RedirectToAction("Index");
        }

        // ================== DELETE Complaint ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Complaint_id)
        {
            var response = await _httpClient.DeleteAsync($"Complaint/ {Complaint_id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";

            return RedirectToAction("Index");
        }

        //complaint count
        public async Task<JsonResult> GetComplaintStatusCount()
        {
            var response =
                await _httpClient.GetAsync("Complaint");

            var data =
                await response.Content.ReadAsStringAsync();

            var list =
                JsonConvert.DeserializeObject<List<ComplaintModel>>(data);

            var pending =
                list.Count(x => x.Status == "Pending");

            var inprogress =
                list.Count(x => x.Status == "In Progress");

            var resolved =
                list.Count(x => x.Status == "Resolved");

            return Json(new
            {
                pending,
                inprogress,
                resolved
            });
        }
        // ================== EXPORT COMPLAINT TO EXCEL ==================
        public async Task<IActionResult> ExportToExcel()
        {
            var response = await _httpClient.GetAsync("Complaint");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error fetching Complaint data";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();

            var complaints = JsonConvert.DeserializeObject<List<ComplaintModel>>(data)
                             ?? new List<ComplaintModel>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Complaint List");

                // Header
                worksheet.Cell(1, 1).Value = "Complaint ID";
                worksheet.Cell(1, 2).Value = "Student ID";
                worksheet.Cell(1, 3).Value = "Complaint Type";
                worksheet.Cell(1, 4).Value = "Complaint Details";
                worksheet.Cell(1, 5).Value = "Complaint Date";
                worksheet.Cell(1, 6).Value = "Status";
                worksheet.Cell(1, 7).Value = "Action Taken";
                worksheet.Cell(1, 8).Value = "Resolved Date";
                worksheet.Cell(1, 9).Value = "Created At";

                // Header Style
                var header = worksheet.Range("A1:I1");

                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.Orange;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Insert Data
                int row = 2;

                foreach (var complaint in complaints)
                {
                    worksheet.Cell(row, 1).Value = complaint.Complaint_id;
                    worksheet.Cell(row, 2).Value = complaint.Student_id;
                    worksheet.Cell(row, 3).Value = complaint.ComplaintType;
                    worksheet.Cell(row, 4).Value = complaint.ComplaintDetails;
                    worksheet.Cell(row, 5).Value = complaint.ComplaintDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 6).Value = complaint.Status;
                    worksheet.Cell(row, 7).Value = complaint.ActionTaken;

                    worksheet.Cell(row, 8).Value = complaint.ResolvedDate.HasValue
                        ? complaint.ResolvedDate.Value.ToString("yyyy-MM-dd")
                        : "";

                    worksheet.Cell(row, 9).Value = complaint.CreatedAt.ToString("yyyy-MM-dd");

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
                        "ComplaintList.xlsx"
                    );
                }
            }
        }



    }
}


