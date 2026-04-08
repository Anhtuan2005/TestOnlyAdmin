namespace HubCinemaAdmin.Models
{
    public class SeatTypeInRoomAdminViewModel
    {
        public int Id { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public string RowCode { get; set; }
        public string SeatType { get; set; }
        public long Price { get; set; }

        // Dùng để gửi đi (POST/PUT)
        public int CinemaId { get; set; }
        public int RoomId { get; set; }
    }


}
