using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Areas.V1.Models
{
    [Table("WF_NGHIPHEP")]
    public class NghiphepModel
    {
        [Key]
        public int id { get; set; }

        public string user_id { get; set; }

        public string email { get; set; }

        public string title { get; set; }

        public string code { get; set; }

        public int? status_id { get; set; }
        public double? songaynghi { get; set; }

        public DateTime? tu_ngay { get; set; }
        public DateTime? den_ngay { get; set; }
        public string? ly_do { get; set; }
        public string? loaiphep { get; set; }

    }
}
