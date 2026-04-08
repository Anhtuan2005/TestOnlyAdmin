namespace HubCinemaAdmin.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public DateTime? CreateAt { get; set; }
    }
    public class InvoiceDto
    {
        public string OrderId { get; set; }
        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime ShowTime { get; set; }
        public string Seats { get; set; }
        public decimal Price { get; set; }
        public decimal ComboTotal { get; set; }

        public List<FoodDto> Foods { get; set; }
    }

    public class FoodDto
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
