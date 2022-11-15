using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
    [Table("e_activity")]
    public class ActivityModel
    {
        [Key]
        public string id { get; set; }

        public int? execution_id { get; set; }
        public string? label { get; set; }
        public string? clazz { get; set; }
        public string block_id { get; set; }

        public int stt { get; set; }

        public List<ExecutionFieldModel>? fields { get; set; }

        public string? created_by { get; set; }


        [ForeignKey("created_by")]
        public virtual UserModel? user_created_by { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? deleted_at { get; set; }
    }

}
