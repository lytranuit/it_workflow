using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
    [Table("execution")]
    public class ExecutionModel
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public string user_id { get; set; }
        public int status_id { get; set; }

        public string process_id { get; set; }

        [ForeignKey("process_id")]
        public ProcessModel process { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }

    enum ExecutionStatus
    {
        [Display(Name = "Nháp")]
        Draft = 1,
        [Display(Name = "Đang thực hiên")]
        Executing = 2,
        [Display(Name = "Hoàn thành")]
        Success = 3,
        [Display(Name = "Thất bại")]
        Fail = 4,
    }
}
