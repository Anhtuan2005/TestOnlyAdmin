namespace HubCinemaAdmin.Models
{
    public class SetSeatTypes
    {
        public int MaPhong { get; set; }
        public int MaRap { get; set; }
        public List<SeatTypeDto> DanhSachGhe { get; set; } = new();
    }
}
