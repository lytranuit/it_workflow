using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace it.Areas.Admin.Models
{
    [Table("e_comment_file")]
    public class CommentFileModel
    {
        public int id { get; set; }

        [StringLength(255)]
        public string name { get; set; }
        [StringLength(255)]
        public string url { get; set; }
        [StringLength(50)]
        public string ext { get; set; }
        [StringLength(255)]
        public string mimeType { get; set; }

        public int comment_id { get; set; }

        [ForeignKey("comment_id")]
        public virtual CommentModel? comment { get; set; }

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }




    }
}
