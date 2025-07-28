using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Vue.Models
{
    [Table("document_type")]
    public class DocumentTypeModel
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        public bool? is_self_check { get; set; }
        public bool? is_active { get; set; }
        public string? color { get; set; }
        public int? stt { get; set; }
        public string? symbol { get; set; }
        public int? min_sign { get; set; }
        public int? group_id { get; set; }


        public virtual List<DocumentTypeReceiveModel>? users_receive { get; set; }
        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }


    }
}
