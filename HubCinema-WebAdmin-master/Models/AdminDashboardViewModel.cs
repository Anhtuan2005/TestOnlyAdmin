namespace HubCinemaAdmin.Models
{
    public class AdminDashboardViewModel
    {
        public DashboardSummaryDto Summary { get; set; }  // ✅ Dữ liệu thống kê
        public List<ChartDataPoint> DailySales { get; set; }  // ✅ Dữ liệu biểu đồ ngày
        public List<ChartDataPoint> CinemaSales { get; set; } // ✅ Dữ liệu biểu đồ rạp
    }

    public class DashboardSummaryDto
    {
        public int TicketsToday { get; set; }
        public int TicketsThisMonth { get; set; }
        public int TicketsThisYear { get; set; }
        public int CurrentMovies { get; set; }
        public int UpcomingMovies { get; set; }
        public int ActiveCinemas { get; set; }
        public int TotalUsers { get; set; }
    }

    public class ChartDataPoint
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
}
