using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Areas.V1.Models
{
    [Table("process")]
    public class ProcessModel
    {
        [Key]
        public string id { get; set; }

        public string name { get; set; }
        public string? description { get; set; }
        public string user_id { get; set; }
        public int status_id { get; set; }

        public int group_id { get; set; }

        [ForeignKey("group_id")]
        public ProcessGroupModel group { get; set; }

        public virtual List<ProcessBlockModel>? blocks { get; set; }
        public virtual List<ProcessLinkModel>? links { get; set; }
        public virtual List<ProcessFieldModel>? fields { get; set; }
        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }

    enum ProcessStatus
    {
        [Display(Name = "Nháp")]
        Draft = 1,
        [Display(Name = "Đang phát hành")]
        Release = 2
    }
}
