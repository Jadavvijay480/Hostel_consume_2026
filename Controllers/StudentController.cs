using ClosedXML.Excel;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;

        public StudentController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        // GET: /Student
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login", "Account");

            var response = await _httpClient.GetAsync("Student");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "API Error";
                return View(new List<StudentModel>());
            }

            var data = await response.Content.ReadAsStringAsync();
            var students = JsonConvert.DeserializeObject<List<StudentModel>>(data) ?? new List<StudentModel>();

            return View(students);
        }

        // GET: Add student
        public IActionResult Add() => View(new StudentModel());

        // GET: Edit student
        public async Task<IActionResult> Edit(int Student_id)
        {
            var response = await _httpClient.GetAsync($"Student/{Student_id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var data = await response.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<StudentModel>(data);

            return View("Add", student);
        }

        // POST: Add/Edit student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(StudentModel student)
        {
            // ✅ ModelState validation works with FluentValidation automatically
            if (!ModelState.IsValid)
                return View(student);

            var json = JsonConvert.SerializeObject(student);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = student.Student_id == 0
                ? await _httpClient.PostAsync("Student", content)
                : await _httpClient.PutAsync($"Student/{student.Student_id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "API Error: " + error);
                return View(student);
            }
            TempData["Success"] = student.Student_id == 0
    ? "Student added successfully!"
    : "Student updated successfully!";

           

            return RedirectToAction("Index");
        }

        // POST: Delete student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Student_id)
        {
            var response = await _httpClient.DeleteAsync($"Student/{Student_id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";

            return RedirectToAction("Index");
        }

        // GET: Total student count (JSON)
        public async Task<JsonResult> GetStudentCount()
        {
            var response = await _httpClient.GetAsync("Student");
            var data = await response.Content.ReadAsStringAsync();
            var students = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            return Json(students.Count);
        }

        // GET: Gender count (JSON)
        public async Task<JsonResult> GetGenderCount()
        {
            var response = await _httpClient.GetAsync("Student");
            var data = await response.Content.ReadAsStringAsync();
            var students = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            var maleCount = students.Count(s => s.Gender == "Male");
            var femaleCount = students.Count(s => s.Gender == "Female");

            return Json(new { male = maleCount, female = femaleCount });
        }

        // GET: Export student list to Excel
        public async Task<IActionResult> ExportToExcel()
        {
            var response = await _httpClient.GetAsync("Student");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error fetching data";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();
            var students = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Students");

                // Header
                worksheet.Cell(1, 1).Value = "Student ID";
                worksheet.Cell(1, 2).Value = "First Name";
                worksheet.Cell(1, 3).Value = "Last Name";
                worksheet.Cell(1, 4).Value = "Gender";
                worksheet.Cell(1, 5).Value = "Date of Birth";
                worksheet.Cell(1, 6).Value = "Phone Number";
                worksheet.Cell(1, 7).Value = "Email Address";
                worksheet.Cell(1, 8).Value = "Address";

                // Header style
                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

                // Data
                int row = 2;
                foreach (var student in students)
                {
                    worksheet.Cell(row, 1).Value = student.Student_id;
                    worksheet.Cell(row, 2).Value = student.First_name;
                    worksheet.Cell(row, 3).Value = student.Last_name;
                    worksheet.Cell(row, 4).Value = student.Gender;
                    worksheet.Cell(row, 5).Value = student.Dob.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 6).Value = student.Phone;
                    worksheet.Cell(row, 7).Value = student.Email;
                    worksheet.Cell(row, 8).Value = student.Address;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "StudentList.xlsx");
                }
            }
        }
    }
}


