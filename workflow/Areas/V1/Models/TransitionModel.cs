using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Areas.V1.Models
{
    [Table("e_transition")]
    public class TransitionModel
    {
        [Key]
        public string id { get; set; }


        public int execution_id { get; set; }
        public string? from_block_id { get; set; }
        public string? to_block_id { get; set; }
        public string? link_id { get; set; }
        public bool? reverse { get; set; }

        public string? from_activity_id { get; set; }
        public string? to_activity_id { get; set; }
        public int? stt { get; set; }


        public string? label { get; set; }

        [ForeignKey("execution_id")]
        public ExecutionModel? ExecutionModel { get; set; }
        public string? created_by { get; set; }
        public DateTime? created_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }

}
