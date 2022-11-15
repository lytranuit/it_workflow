using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace it.Areas.Admin.Models
{
    [Table("e_comment")]
    public class CommentModel
    {
        public int id { get; set; }
        public int execution_id { get; set; }
        public string? comment { get; set; }


        [ForeignKey("execution_id")]
        public virtual ExecutionModel? execution { get; set; }


        public string user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual UserModel? user { get; set; }
        public virtual List<CommentFileModel>? files { get; set; }

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [NotMapped]

        public bool is_read { get; set; } = false;


    }
}
