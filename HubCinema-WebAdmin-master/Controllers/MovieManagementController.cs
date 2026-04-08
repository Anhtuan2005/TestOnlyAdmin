using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class MovieManagementController : BaseController
    {
        private readonly IMovieService _movieService;

        public MovieManagementController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Check authentication
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieDTO movie)
        {
            // Check authentication
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            // Validate request
            if (!ModelState.IsValid)
                return View(movie);

            try
            {
                // Call service
                var success = await _movieService.CreateMovieAsync(movie);

                // Handle response
                if (success)
                {
                    TempData["Success"] = "Tạo phim thành công!";
                    return RedirectToAction("LoadListMovie");
                }

                TempData["Error"] = "Tạo phim thất bại!";
                return View(movie);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return View(movie);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadListMovie(int pageNumber = 1)
        {
            // Check authentication
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            const int pageSize = 10;

            try
            {
                // Call service
                var movies = await _movieService.GetAllMoviesAsync();

                // ✅ Kiểm tra null và tạo empty list nếu cần
                if (movies == null)
                {
                    movies = new List<MovieDTO>();
                }

                // Prepare pagination
                var totalMovies = movies.Count;
                var moviesPaged = movies
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Set ViewBag for pagination
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling((double)totalMovies / pageSize));

                // ✅ Đảm bảo luôn trả về list, không bao giờ null
                return View(moviesPaged ?? new List<MovieDTO>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải danh sách phim: " + ex.Message;
                
                // ✅ Set ViewBag để tránh lỗi pagination
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 1;
                
                // ✅ Trả về empty list thay vì null
                return View(new List<MovieDTO>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditMovie(int id)
        {
            // Check authentication
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            try
            {
                // Call service
                var movies = await _movieService.GetAllMoviesAsync();
                
                if (movies != null)
                {
                    var movie = movies.FirstOrDefault(m => m.IDMovie == id);
                    if (movie != null)
                    {
                        return View(movie);
                    }
                }

                TempData["Error"] = "Không tìm thấy phim!";
                return RedirectToAction("LoadListMovie");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin phim: {ex.Message}";
                return RedirectToAction("LoadListMovie");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditMovie(MovieDTO movie)
        {
            // Check authentication
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            // Validate request
            if (!ModelState.IsValid)
                return View(movie);

            try
            {
                // Call service
                var success = await _movieService.UpdateMovieAsync(movie);

                // Handle response
                if (success)
                {
                    TempData["Success"] = "Cập nhật phim thành công!";
                    return RedirectToAction("LoadListMovie");
                }

                TempData["Error"] = "Cập nhật phim thất bại!";
                return View(movie);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return View(movie);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            // Check authentication
            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            try
            {
                // Call service
                var success = await _movieService.DeleteMovieAsync(id);

                // Handle response
                if (success)
                {
                    TempData["Success"] = "Xóa phim thành công!";
                }
                else
                {
                    TempData["Error"] = "Xóa phim thất bại!";
                }

                return RedirectToAction("LoadListMovie");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa phim: {ex.Message}";
                return RedirectToAction("LoadListMovie");
            }
        }
    }
}
