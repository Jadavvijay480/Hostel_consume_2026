using ClosedXML.Excel;
using Hostel_Consume_2026.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Hostel_Consume_2026.Controllers
{
    public class RoomBookingController : Controller
    {
        private readonly HttpClient _httpClient;

        public RoomBookingController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        // ================== BOOKING LIST ==================
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _httpClient.GetAsync("RoomBooking");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to fetch data from API.";
                return View(new List<RoomBookingModel>());
            }

            var data = await response.Content.ReadAsStringAsync();

            var bookings = JsonConvert.DeserializeObject<List<RoomBookingModel>>(data)
                          ?? new List<RoomBookingModel>();

            return View(bookings);
        }

        // ================== ADD BOOKING (GET) ==================
        public IActionResult Add()
        {
            return View(new RoomBookingModel());
        }

        // ================== EDIT BOOKING (GET) ==================
        public async Task<IActionResult> Edit(int Booking_Id)
        {
            var response = await _httpClient.GetAsync($"RoomBooking/{Booking_Id}");

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var data = await response.Content.ReadAsStringAsync();

            var booking = JsonConvert.DeserializeObject<RoomBookingModel>(data);

            return View("Add", booking);
        }

        // ================== ADD / UPDATE BOOKING (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoomBookingModel booking)
        {
            if (!ModelState.IsValid)
                return View(booking);

            var json = JsonConvert.SerializeObject(booking);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (booking.Booking_Id == 0)
                response = await _httpClient.PostAsync("RoomBooking", content);
            else
                response = await _httpClient.PutAsync($"RoomBooking/{booking.Booking_Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "API Error: " + error);
                return View(booking);
            }
            TempData["Success"] = booking.Booking_Id == 0
              ? "Booking added successfully!"
                    : "Booking updated successfully!";

            

            return RedirectToAction("Index");
        }

        // ================== DELETE BOOKING ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Booking_Id)
        {
            var response = await _httpClient.DeleteAsync($"RoomBooking/{Booking_Id}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Delete failed!";

            return RedirectToAction("Index");
        }

        // ================== BOOKING STATUS COUNT ==================
        public async Task<JsonResult> GetBookingStatusCount()
        {
            var response = await _httpClient.GetAsync("RoomBooking");
            var data = await response.Content.ReadAsStringAsync();

            var bookings = JsonConvert.DeserializeObject<List<RoomBookingModel>>(data);

            var booked = bookings.Count(b => b.Booking_Status.ToLower() == "booked");
            var checkedIn = bookings.Count(b => b.Booking_Status.ToLower() == "checkedin");
            var checkedOut = bookings.Count(b => b.Booking_Status.ToLower() == "checkedout");
            var cancelled = bookings.Count(b => b.Booking_Status.ToLower() == "cancelled");

            return Json(new
            {
                booked = booked,
                checkedIn = checkedIn,
                checkedOut = checkedOut,
                cancelled = cancelled
            });
        }

        // ================== EXPORT BOOKING TO EXCEL ==================
        public async Task<IActionResult> ExportToExcel()
        {
            var response = await _httpClient.GetAsync("RoomBooking");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error fetching booking data";
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();

            var bookings = JsonConvert.DeserializeObject<List<RoomBookingModel>>(data)
                          ?? new List<RoomBookingModel>();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Room Booking List");

                // ===== HEADER =====
                worksheet.Cell(1, 1).Value = "Booking ID";
                worksheet.Cell(1, 2).Value = "Guest Name";
                worksheet.Cell(1, 3).Value = "Mobile";
                worksheet.Cell(1, 4).Value = "Room Number";
                worksheet.Cell(1, 5).Value = "Address";
                worksheet.Cell(1, 6).Value = "Booking Date";
                worksheet.Cell(1, 7).Value = "Status";

                // ===== STYLE =====
                var header = worksheet.Range("A1:G1");
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.Cyan;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // ===== DATA =====
                int row = 2;

                foreach (var item in bookings)
                {
                    worksheet.Cell(row, 1).Value = item.Booking_Id;
                    worksheet.Cell(row, 2).Value = item.Guest_Name;
                    worksheet.Cell(row, 3).Value = item.Mobile_No;
                    worksheet.Cell(row, 4).Value = item.Room_Number;
                    worksheet.Cell(row, 5).Value = item.Address;
                    worksheet.Cell(row, 6).Value = item.Booking_Date;
                    worksheet.Cell(row, 7).Value = item.Booking_Status;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "RoomBookingList.xlsx"
                    );
                }
            }
        }
    }
}