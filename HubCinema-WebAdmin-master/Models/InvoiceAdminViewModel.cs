namespace HubCinemaAdmin.Models
{
    public class InvoiceAdminViewModel
    {
        public int IdInvoice { get; set; }
        public DateTime CreateAt { get; set; }
        public int TotalPrice { get; set; }
        public byte Status { get; set; }

        public string MovieName { get; set; }
        public DateTime NgayChieu { get; set; }
        public TimeSpan GioChieu { get; set; }
        public string Room { get; set; }

        public List<string> Seats { get; set; }
        public List<FoodItemDto> Foods { get; set; }
    }

    public class FoodItemDto
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }

}
