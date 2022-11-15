using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
    [Table("e_user_unread")]
    public class UserUnreadModel
    {
        public int id { get; set; }

        [Required]
        public int execution_id { get; set; }


        [Required]
        public string user_id { get; set; }

        [Required]
        public DateTime time { get; set; }

    }
}
