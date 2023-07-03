using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Areas.V1.Models
{
    [Table("e_user_read")]
    public class UserReadModel
    {
        public int id { get; set; }

        [Required]
        public int execution_id { get; set; }

        [Required]
        public string user_id { get; set; }

        [Required]
        public DateTime time_read { get; set; }

    }
}
