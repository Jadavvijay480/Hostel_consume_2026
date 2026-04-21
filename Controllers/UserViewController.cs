using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class UserViewController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserViewController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }


        // ================== User LIST ==================
        public async Task<IActionResult> Index()
        {


            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }



            var response = await _httpClient.GetAsync("UserView");
            

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to fetch data from API.";
                return View(new List<UserModel>());
            }

            var data = await response.Content.ReadAsStringAsync();

            var userView = JsonConvert.DeserializeObject<List<UserModel>>(data)
                        ?? new List<UserModel>();

            return View(userView);
        }

        // ================== ADD User (GET) ==================
        public IActionResult Add()
        {
            return View(new UserModel());
        }

        // ================== EDIT User (GET) ==================
        public async Task<IActionResult> Edit(int UserId)
        {
            var response = await _httpClient.GetAsync($"UserView/{UserId}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var data = await response.Content.ReadAsStringAsync();

            var userView = JsonConvert.DeserializeObject<UserModel>(data);

            return View("Add", userView);
        }

        // ================== ADD / UPDATE User (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserModel userView)
        {
            if (!ModelState.IsValid)
                return View(userView);

            var json = JsonConvert.SerializeObject(userView);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (userView.UserId == 0)
                response = await _httpClient.PostAsync("UserView", content);
            else
                response = await _httpClient.PutAsync($"UserView/{userView.UserId}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "API Error: " + error);
                return View(userView);
            }
            TempData["Success"] = userView.UserId == 0
    ? "User added successfully!"
    : "User updated successfully!";


            return RedirectToAction("Index");
        }

        // ================== DELETE User ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int UserId)
        {
            var response = await _httpClient.DeleteAsync($"UserView/ {UserId}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";

            return RedirectToAction("Index");
        }

        //user count 
        public async Task<JsonResult> GetUserCount()
        {
            var response =
                await _httpClient.GetAsync("UserView");

            var data =
                await response.Content.ReadAsStringAsync();

            var list =
                JsonConvert.DeserializeObject<List<UserModel>>(data);

            var developer =
                list.Count(x => x.Role == "Developer");

            var student =
                list.Count(x => x.Role == "Student");

            var warden =
                list.Count(x => x.Role == "Warden");

            var active =
                list.Count(x => x.IsActive == true);

            var inactive =
                list.Count(x => x.IsActive == false);

            return Json(new
            {
                developer,
                student,
                warden,
                active,
                inactive
            });
        }


       
        // ✅ User Role + Active / Inactive Pie Chart
        public async Task<JsonResult> GetUserRoleStatusPieChart()
        {
            var response = await _httpClient.GetAsync("UserView");

            var data = await response.Content.ReadAsStringAsync();

            var list = JsonConvert.DeserializeObject<List<UserModel>>(data);

            var result = new
            {
                activeDeveloper = list.Count(x => x.Role == "Developer" && x.IsActive),
                inactiveDeveloper = list.Count(x => x.Role == "Developer" && !x.IsActive),

                activeStudent = list.Count(x => x.Role == "Student" && x.IsActive),
                inactiveStudent = list.Count(x => x.Role == "Student" && !x.IsActive),

                activeWarden = list.Count(x => x.Role == "Warden" && x.IsActive),
                inactiveWarden = list.Count(x => x.Role == "Warden" && !x.IsActive)
            };

            return Json(result);
        }

        // ================== EXPORT USER TO EXCEL ==================
        public async Task<IActionResult> ExportToExcel()
        {
            var response = await _httpClient.GetAsync("UserView");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error fetching User data";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<List<UserModel>>(data)
                        ?? new List<UserModel>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("User List");

                // ===== HEADER =====

                worksheet.Cell(1, 1).Value = "User ID";
                worksheet.Cell(1, 2).Value = "Full Name";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Password";
                worksheet.Cell(1, 5).Value = "Mobile No";
                worksheet.Cell(1, 6).Value = "Role";
                worksheet.Cell(1, 7).Value = "Status";
                worksheet.Cell(1, 8).Value = "Created At";
                worksheet.Cell(1, 9).Value = "Updated At";

                // ===== HEADER STYLE =====

                var header = worksheet.Range("A1:I1");

                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // ===== INSERT DATA =====

                int row = 2;

                foreach (var user in users)
                {
                    worksheet.Cell(row, 1).Value = user.UserId;
                    worksheet.Cell(row, 2).Value = user.FullName;
                    worksheet.Cell(row, 3).Value = user.Email;
                    worksheet.Cell(row, 4).Value = user.Password;
                    worksheet.Cell(row, 5).Value = user.MobileNo;
                    worksheet.Cell(row, 6).Value = user.Role;

                    worksheet.Cell(row, 7).Value = user.IsActive ? "Active" : "Inactive";

                    worksheet.Cell(row, 8).Value = user.CreatedAt.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 9).Value = user.UpdatedAt.ToString("yyyy-MM-dd");

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
                        "UserList.xlsx"
                    );
                }
            }
        }



    }
}


