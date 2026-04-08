using System.ComponentModel.DataAnnotations;

namespace HubCinemaAdmin.Models
{
    public class CinemaDTO
    {
        public int IDCinema { get; set; }

        [Display(Name = "Tên rạp")]
        public string CinemaName { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Thành phố")]
        public string City { get; set; }
    }
}
