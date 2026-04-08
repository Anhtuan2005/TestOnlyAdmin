using System.ComponentModel.DataAnnotations;

namespace HubCinemaAdmin.Models
{
    public class FoodDTO
    {
        public int? IDFood { get; set; }
        [Display(Name = "Tên đồ ăn")]
        public string FoodName { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Giá")]
        public decimal Price { get; set; }
        [Display(Name = "Ảnh")]
        public string ImageURL { get; set; }
        public List<int> SelectedCinemaIds { get; set; } = new();
        public bool ApplyToAllCinemas { get; set; }

    }
}
