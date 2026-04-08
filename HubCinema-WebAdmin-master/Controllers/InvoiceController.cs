using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ClosedXML.Excel;

namespace HubCinemaAdmin.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public InvoiceController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: /Invoice?startDate=2025-07-01&endDate=2025-07-31
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = "http://api.dvxuanbac.com:2030/api/Invoice/all";

            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể lấy dữ liệu hóa đơn từ API.";
                return View(new List<InvoiceAdminViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var invoices = JsonConvert.DeserializeObject<List<InvoiceAdminViewModel>>(json);

            // Lọc theo ngày
            if (startDate.HasValue)
                invoices = invoices.Where(i => i.CreateAt.Date >= startDate.Value.Date).ToList();
            if (endDate.HasValue)
                invoices = invoices.Where(i => i.CreateAt.Date <= endDate.Value.Date).ToList();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(invoices);
        }

        // GET: /Invoice/ExportToExcel?startDate=2025-07-01&endDate=2025-07-31
        public async Task<IActionResult> ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = "http://api.dvxuanbac.com:2030/api/Invoice/all";

            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể lấy dữ liệu hóa đơn từ API.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var invoices = JsonConvert.DeserializeObject<List<InvoiceAdminViewModel>>(json);

            // Lọc theo ngày
            if (startDate.HasValue)
                invoices = invoices.Where(i => i.CreateAt.Date >= startDate.Value.Date).ToList();
            if (endDate.HasValue)
                invoices = invoices.Where(i => i.CreateAt.Date <= endDate.Value.Date).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Hóa Đơn");

            worksheet.Cell(1, 1).Value = "Mã HD";
            worksheet.Cell(1, 2).Value = "Ngày tạo";
            worksheet.Cell(1, 3).Value = "Phim";
            worksheet.Cell(1, 4).Value = "Suất chiếu";
            worksheet.Cell(1, 5).Value = "Phòng";
            worksheet.Cell(1, 6).Value = "Ghế";
            worksheet.Cell(1, 7).Value = "Combo";
            worksheet.Cell(1, 8).Value = "Tổng tiền";

            for (int i = 0; i < invoices.Count; i++)
            {
                var row = i + 2;
                var invoice = invoices[i];

                worksheet.Cell(row, 1).Value = invoice.IdInvoice;
                worksheet.Cell(row, 2).Value = invoice.CreateAt.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 3).Value = invoice.MovieName;
                worksheet.Cell(row, 4).Value = $"{invoice.NgayChieu:dd/MM} {invoice.GioChieu}";
                worksheet.Cell(row, 5).Value = invoice.Room;
                worksheet.Cell(row, 6).Value = string.Join(", ", invoice.Seats ?? new List<string>());
                worksheet.Cell(row, 7).Value = invoice.Foods != null && invoice.Foods.Any()
                    ? string.Join(", ", invoice.Foods.Select(f => $"{f.FoodName} x{f.Quantity}"))
                    : "Không có";
                worksheet.Cell(row, 8).Value = invoice.TotalPrice;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DanhSachHoaDon_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
