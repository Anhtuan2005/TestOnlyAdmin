using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class CinemaManagementController : BaseController
    {
        private readonly CinemaService _cinemaService;
        private readonly IRoomService _roomService;

        public CinemaManagementController(IHttpClientFactory httpClientFactory, IRoomService roomService, IHttpContextAccessor contextAccessor)
        {
            _cinemaService = new CinemaService(httpClientFactory, contextAccessor);
            _roomService = roomService;
        }

        public IActionResult CreateCinema()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCinema(CinemaDTO cinemaDTO)
        {
            if (!ModelState.IsValid)
                return View(cinemaDTO);

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var success = await _cinemaService.CreateCinemaAsync(cinemaDTO);

            if (success)
            {
                TempData["Success"] = "Tạo rạp chiếu thành công!";
                return RedirectToAction("LoadListCinema");
            }

            TempData["Error"] = "Tạo rạp chiếu thất bại! Vui lòng kiểm tra lại thông tin.";
            return View(cinemaDTO);
        }

        public async Task<IActionResult> LoadListCinema()
        {
            try
            {
                var cinemas = await _cinemaService.GetCinemasAsync();
                return View(cinemas);
            }
            catch (Exception)
            {
                TempData["Error"] = "Không thể tải danh sách rạp chiếu!";
                return View(new List<CinemaDTO>());
            }
        }

        public async Task<IActionResult> EditCinema(int idCinema)
        {
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var cinema = await _cinemaService.GetCinemaByIdAsync(idCinema);
            if (cinema == null)
                return RedirectToAction("LoadListCinema");

            var rooms = await _roomService.GetRoomsByCinemaIdAsync(cinema.IDCinema);
            ViewBag.Rooms = rooms;

            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> EditCinema(CinemaDTO cinemaDTO)
        {
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                var rooms = await _roomService.GetRoomsByCinemaIdAsync(cinemaDTO.IDCinema);
                ViewBag.Rooms = rooms;
                return View(cinemaDTO);
            }

            var success = await _cinemaService.UpdateCinemaAsync(cinemaDTO);

            if (success)
            {
                TempData["Success"] = "Cập nhật rạp chiếu thành công!";
                return RedirectToAction("LoadListCinema");
            }

            var fallbackRooms = await _roomService.GetRoomsByCinemaIdAsync(cinemaDTO.IDCinema);
            ViewBag.Rooms = fallbackRooms;

            TempData["Error"] = "Cập nhật rạp chiếu thất bại!";
            return View(cinemaDTO);
        }
        [HttpDelete("CinemaManagement/DeleteCinema/{idCinema}")]
        public async Task<IActionResult> DeleteCinema(int idCinema)
        {
            if (!IsAuthenticated)
                return Unauthorized();

            try
            {
                var success = await _cinemaService.DeleteCinemaAsync(idCinema);
                if (success)
                    return Ok("Xóa thành công!");
                else
                    return BadRequest("Xóa rạp chiếu thất bại!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa rạp: {ex.Message}");
            }
        }

    }
}
