using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
    [Table("department")]
    public class DepartmentModel
    {
        [Key]
        public int id { get; set; }

        public string? color { get; set; }
        public string name { get; set; }
        public int? parent { get; set; } = 0;
        public int? count_child { get; set; } = 0;
        public int? stt { get; set; } = 0;
        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }
}
