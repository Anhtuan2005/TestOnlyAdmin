using System.ComponentModel.DataAnnotations;

namespace HubCinemaAdmin.Models
{
    public class MovieDTO
    {
        public int? IDMovie { get; set; }
        [Display(Name = "Tên phim")]
        public string MovieName { get; set; }
        [Display(Name = "Thể loại")]
        public string Genre { get; set; }
        [Display(Name = "Thời lượng")]
        public int Duration { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Đạo diễn")]
        public string Director { get; set; }
        [Display(Name = "Ngày khởi chiếu")]
        public DateTime ReleaseDate { get; set; }
        public string CoverURL { get; set; }
        public string TrailerURL { get; set; }
        [Display(Name = "Giới hạn tuổi")]
        public string AgeRestriction { get; set; }
        [Display(Name = "Nhà sản xuất")]
        public string Producer { get; set; }
        [Display(Name = "Diễn viên")]
        public string Actors { get; set; }
        [Display(Name = "Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        public int status { get; set; } // 0: Đang chiếu, 1: Sắp chiếu, 2: Ngừng chiếu
    }
}
