using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace HubCinemaAdmin.Controllers
{
    public class SeatTypeInRoomAdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string apiUrl = "http://api.dvxuanbac.com:2030/api/SeatTypeInRoom";

        public SeatTypeInRoomAdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        //GET: Danh sách
        public async Task<IActionResult> Index(string cinema = "", string room = "")
        {
            var response = await _httpClient.GetAsync("http://api.dvxuanbac.com:2030/api/SeatTypeInRoom");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể lấy dữ liệu.";
                return View(new List<SeatTypeInRoomAdminViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<SeatTypeInRoomAdminViewModel>>(json);

            // Tạo danh sách distinct để render dropdown
            ViewBag.CinemaNames = list.Select(x => x.CinemaName).Distinct().OrderBy(x => x).ToList();
            ViewBag.RoomNames = list.Select(x => x.RoomName).Distinct().OrderBy(x => x).ToList();

            // Lọc dữ liệu nếu có chọn
            if (!string.IsNullOrEmpty(cinema))
                list = list.Where(x => x.CinemaName == cinema).ToList();
            if (!string.IsNullOrEmpty(room))
                list = list.Where(x => x.RoomName == room).ToList();

            ViewBag.SelectedCinema = cinema;
            ViewBag.SelectedRoom = room;

            return View(list);
        }



        //GET: Thêm mới
        public IActionResult Create()
        {
            return View();
        }

        //POST: Thêm mới
        [HttpPost]
        public async Task<IActionResult> Create(SeatTypeInRoomAdminViewModel model)
        {
            var requestObj = new
            {
                cinemaId = model.CinemaId,
                roomId = model.RoomId,
                rowCode = model.RowCode,
                seatType = model.SeatType,
                price = model.Price
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestObj), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Thêm thất bại.");
            return View(model);
        }

        //GET: Sửa
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync("http://api.dvxuanbac.com:2030/api/SeatTypeInRoom");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<SeatTypeInRoomAdminViewModel>>(json);
            var model = list.FirstOrDefault(x => x.Id == id);
            if (model == null) return NotFound();

            ViewBag.Cinemas = new SelectList(
                list.GroupBy(x => new { x.CinemaId, x.CinemaName })
                    .Select(g => g.Key)
                    .OrderBy(x => x.CinemaName),
                "CinemaId",
                "CinemaName",
                model.CinemaId
            );

            ViewBag.Rooms = new SelectList(
                list.GroupBy(x => new { x.RoomId, x.RoomName })
                    .Select(g => g.Key)
                    .OrderBy(x => x.RoomName),
                "RoomId",
                "RoomName",
                model.RoomId
            );



            return View(model);
        }


        //POST: Sửa
        [HttpPost]
        public async Task<IActionResult> Edit(int id, SeatTypeInRoomAdminViewModel model)
        {
            var requestObj = new
            {
                cinemaId = model.CinemaId,
                roomId = model.RoomId,
                rowCode = model.RowCode,
                seatType = model.SeatType,
                price = model.Price
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestObj), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{apiUrl}/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại.");
            return View(model);
        }

        //GET: Xoá xác nhận
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<SeatTypeInRoomAdminViewModel>>(json);
            var model = list.FirstOrDefault(x => x.Id == id);
            if (model == null) return NotFound();

            return View(model);
        }

        //POST: Xoá
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{apiUrl}/{id}");
            return RedirectToAction("Index");
        }
    }
}
