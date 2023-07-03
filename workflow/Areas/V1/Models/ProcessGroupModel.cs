using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Areas.V1.Models
{
    [Table("process_group")]
    public class ProcessGroupModel
    {
        [Key]
        public int id { get; set; }

        public string? color { get; set; }
        public string name { get; set; }

        public List<ProcessModel> list_process { get; set; }

        public List<ProcessVersionModel> list_process_version { get; set; }
        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }
}
