using ClosedXML.Excel;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class RoomController : Controller
    {
        private readonly HttpClient _httpClient;

        public RoomController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        // ================== ROOM LIST ==================
        public async Task<IActionResult> Index()
        {


            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }



            var response = await _httpClient.GetAsync("Room");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to fetch data from API.";
                return View(new List<RoomModel>());
            }

            var data = await response.Content.ReadAsStringAsync();

            var rooms = JsonConvert.DeserializeObject<List<RoomModel>>(data)
                        ?? new List<RoomModel>();

            return View(rooms);
        }

        // ================== ADD ROOM (GET) ==================
        public IActionResult Add()
        {
            return View(new RoomModel());
        }

        // ================== EDIT ROOM (GET) ==================
        public async Task<IActionResult> Edit(int Room_id)
        {
            var response = await _httpClient.GetAsync($"Room/{Room_id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var data = await response.Content.ReadAsStringAsync();

            var room = JsonConvert.DeserializeObject<RoomModel>(data);

            return View("Add", room);
        }

        // ================== ADD / UPDATE ROOM (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoomModel room)
        {
            if (!ModelState.IsValid)
                return View(room);

            var json = JsonConvert.SerializeObject(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (room.Room_id == 0)
                response = await _httpClient.PostAsync("Room", content);
            else
                response = await _httpClient.PutAsync($"Room/{room.Room_id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "API Error: " + error);
                return View(room);
            }
            TempData["Success"] = room.Room_id == 0
                           ? "Room added successfully!"
                        : "Room updated successfully!";

            

            return RedirectToAction("Index");
        }

        // ================== DELETE ROOM ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Room_id)
        {
            var response = await _httpClient.DeleteAsync($"Room/{Room_id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";

            return RedirectToAction("Index");
        }

        // GetRoomStatusCount

        public async Task<JsonResult> GetRoomStatusCount()
        {
            var response = await _httpClient.GetAsync("Room");
            var data = await response.Content.ReadAsStringAsync();

            var rooms = JsonConvert.DeserializeObject<List<RoomModel>>(data);

            var occupied = rooms.Count(r => r.Status.ToLower() == "occupied");
            var available = rooms.Count(r => r.Status.ToLower() == "available");
            var maintenance = rooms.Count(r => r.Status.ToLower() == "maintenance");

            return Json(new
            {
                occupied = occupied,
                available = available,
                maintenance = maintenance
            });
        }


        // ================== EXPORT ROOM TO EXCEL ==================
        public async Task<IActionResult> ExportToExcel()
        {
            var response = await _httpClient.GetAsync("Room");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error fetching Room data";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();

            var rooms = JsonConvert.DeserializeObject<List<RoomModel>>(data)
                        ?? new List<RoomModel>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Room List");

                // ===== HEADER =====

                worksheet.Cell(1, 1).Value = "Room ID";
                worksheet.Cell(1, 2).Value = "Student ID";
                worksheet.Cell(1, 3).Value = "Hostel ID";
                worksheet.Cell(1, 4).Value = "Room Number";
                worksheet.Cell(1, 5).Value = "Room Type";
                worksheet.Cell(1, 6).Value = "Capacity";
                worksheet.Cell(1, 7).Value = "Status";

                // ===== HEADER STYLE =====

                var header = worksheet.Range("A1:G1");

                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.Cyan;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // ===== INSERT DATA =====

                int row = 2;

                foreach (var room in rooms)
                {
                    worksheet.Cell(row, 1).Value = room.Room_id;
                    worksheet.Cell(row, 2).Value = room.Student_id;
                    worksheet.Cell(row, 3).Value = room.Hostel_id;
                    worksheet.Cell(row, 4).Value = room.Room_number;
                    worksheet.Cell(row, 5).Value = room.Room_type;
                    worksheet.Cell(row, 6).Value = room.Capacity;
                    worksheet.Cell(row, 7).Value = room.Status;

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
                        "RoomList.xlsx"
                    );
                }
            }
        }



    }
}


