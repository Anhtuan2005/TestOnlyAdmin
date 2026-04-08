namespace HubCinemaAdmin.Models
{
    public class RoomDTO
    {
        public int? IDRoom { get; set; }
        public int CinemaID { get; set; }
        public string RoomName { get; set; }
        public int RoomType { get; set; }
        public string RoomImageURL { get; set; }
        public bool Status { get; set; }
        public string? id_layout { get; set; }
    }
}
