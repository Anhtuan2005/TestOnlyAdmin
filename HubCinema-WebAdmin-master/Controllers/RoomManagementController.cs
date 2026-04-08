using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HubCinemaAdmin.Controllers
{
    public class RoomManagementController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRoomService _roomService;

        public RoomManagementController(IHttpClientFactory httpClientFactory, IRoomService roomService)
        {
            _httpClientFactory = httpClientFactory;
            _roomService = roomService;
        }
        public IActionResult CreateRoom(int idCinema)
        {
            var roomDTO = new RoomDTO
            {
                CinemaID = idCinema
            };
            Console.WriteLine(idCinema);
            return View(roomDTO);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomDTO roomDTO)
        {
            if (!ModelState.IsValid)
                return View(roomDTO);

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");
            Console.WriteLine("EditCinema: " + roomDTO.CinemaID);
            var success = await _roomService.CreateRoomAsync(roomDTO);

            if (success)
            {
                TempData["Success"] = "Tạo phòng chiếu thành công!";
                return RedirectToAction("EditCinema","CinemaManagement", new { idCinema = roomDTO.CinemaID });
            }

            TempData["Error"] = "Tạo rạp chiếu thất bại!";
            return View(roomDTO);
        }

        public async Task<List<RoomDTO>> GetRoomsByCinemaId(int cinemaId)
        {
            var client = CreateAuthorizedClient(_httpClientFactory);

            var response = await client.GetAsync($"{LinkHost.Url}/Admin/GetRoomsByCinemaId/{cinemaId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<RoomDTO>>(json);
                return rooms ?? new List<RoomDTO>();
            }

            return new List<RoomDTO>();
        }
    }
}
