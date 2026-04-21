using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class StudentAttendanceController : Controller
    {
        private readonly HttpClient _httpClient;

        public StudentAttendanceController(IHttpClientFactory factory)
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



            var response = await _httpClient.GetAsync("StudentAttendance");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to fetch data from API.";
                return View(new List<StudentAttendanceModel>());
            }

            var data = await response.Content.ReadAsStringAsync();

            var studentAttendance = JsonConvert.DeserializeObject<List<StudentAttendanceModel>>(data)
                        ?? new List<StudentAttendanceModel>();

            return View(studentAttendance);
        }

        // ================== ADD Admin (GET) ==================
        public IActionResult Add()
        {
            return View(new StudentAttendanceModel());
        }

        // ================== EDIT Admin (GET) ==================
        public async Task<IActionResult> Edit(int Attendance_id)
        {
            var response = await _httpClient.GetAsync($"StudentAttendance/{Attendance_id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var data = await response.Content.ReadAsStringAsync();

            var studentAttendance = JsonConvert.DeserializeObject<StudentAttendanceModel>(data);

            return View("Add", studentAttendance);
        }

        // ================== ADD / UPDATE Admin (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(StudentAttendanceModel studentAttendance)
        {
            if (!ModelState.IsValid)
                return View(studentAttendance);

            var json = JsonConvert.SerializeObject(studentAttendance);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (studentAttendance.Attendance_id == 0)
                response = await _httpClient.PostAsync("StudentAttendance", content);
            else
                response = await _httpClient.PutAsync($"StudentAttendance/{studentAttendance.Attendance_id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "API Error: " + error);
                return View(studentAttendance);
            }
            TempData["Success"] = studentAttendance.Attendance_id == 0
    ? "Attendance added successfully!"
    : "Attendance updated successfully!";

         

            return RedirectToAction("Index");
        }

        // ================== DELETE Admin ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Attendance_id)
        {
            var response = await _httpClient.DeleteAsync($"StudentAttendance/ {Attendance_id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";

            return RedirectToAction("Index");
        }

        //pr and ab
        public async Task<JsonResult> GetAttendanceCount()
        {
            var response =
                await _httpClient.GetAsync("StudentAttendance");

            var data =
                await response.Content.ReadAsStringAsync();

            var list =
                JsonConvert.DeserializeObject<List<StudentAttendanceModel>>(data);

            var present =
                list.Count(x => x.Status == "Present");

            var absent =
                list.Count(x => x.Status == "Absent");

            return Json(new
            {
                present,
                absent
            });
        }

        // Pie Chart Data for Attendance
        public async Task<JsonResult> GetAttendanceByStatus()
        {
            var response = await _httpClient.GetAsync("StudentAttendance");

            var data = await response.Content.ReadAsStringAsync();

            var list = JsonConvert.DeserializeObject<List<StudentAttendanceModel>>(data);

            var result = list
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    status = g.Key,
                    total = g.Count()
                });

            return Json(result);
        }

        // ================== EXPORT ATTENDANCE TO EXCEL ==================
        public async Task<IActionResult> ExportToExcel()
        {
            var response = await _httpClient.GetAsync("StudentAttendance");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error fetching Attendance data";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();

            var attendanceList = JsonConvert.DeserializeObject<List<StudentAttendanceModel>>(data)
                                 ?? new List<StudentAttendanceModel>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Student Attendance");

                // ===== HEADER =====

                worksheet.Cell(1, 1).Value = "Attendance ID";
                worksheet.Cell(1, 2).Value = "Student ID";
                worksheet.Cell(1, 3).Value = "Attendance Date";
                worksheet.Cell(1, 4).Value = "Status";
                worksheet.Cell(1, 5).Value = "Place";
                worksheet.Cell(1, 6).Value = "Remarks";
                worksheet.Cell(1, 7).Value = "Created At";

                // ===== HEADER STYLE =====

                var header = worksheet.Range("A1:G1");

                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightGray;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // ===== INSERT DATA =====

                int row = 2;

                foreach (var item in attendanceList)
                {
                    worksheet.Cell(row, 1).Value = item.Attendance_id;
                    worksheet.Cell(row, 2).Value = item.Student_id;
                    worksheet.Cell(row, 3).Value = item.AttendanceDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 4).Value = item.Status;
                    worksheet.Cell(row, 5).Value = item.Place;
                    worksheet.Cell(row, 6).Value = item.Remarks;
                    worksheet.Cell(row, 7).Value = item.CreatedAt.ToString("yyyy-MM-dd");

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
                        "StudentAttendanceList.xlsx"
                    );
                }
            }
        }





    }
}


