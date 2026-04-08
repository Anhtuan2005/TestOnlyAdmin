namespace HubCinemaAdmin.Models
{
    public class ShowtimeDTO
    {
        public int MaSuatChieu { get; set; }
        public int PhongChieu { get; set; }
        public int MaPhim { get; set; }
        public DateTime NgayChieu { get; set; }
        public TimeSpan GioChieu { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public int ChiPhi { get; set; }
        public int TypeSuatChieu { get; set; }
        public int MaRap { get; set; }
    }
}
