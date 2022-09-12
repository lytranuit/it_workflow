using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
    [Table("process")]
    public class ProcessModel
    {
        [Key]
        public int id { get; set; }

        public string code { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public string user_id { get; set; }
        public int status_id { get; set; }




        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }

    enum ProcessStatus
    {
        [Display(Name = "Đang thực hiện")]
        Excuting = 1,
        [Display(Name = "Đã hoàn thành")]
        Success = 2,
        [Display(Name = "Đang hủy")]
        Cancle = 3,
        [Display(Name = "Nháp")]
        Draft = 4,
    }
}
