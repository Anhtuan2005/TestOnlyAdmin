namespace HubCinemaAdmin.Models
{
    public class SeatLayoutResponse
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string LayoutName { get; set; } = string.Empty;
        public List<List<string?>> Layout { get; set; } = new();
    }
}