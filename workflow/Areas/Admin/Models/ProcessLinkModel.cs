using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
    [Table("process_link")]
    public class ProcessLinkModel
    {
        [Key]
        public string id { get; set; }

        public string process_id { get; set; }
        public string originID { get; set; }
        public int originSlot { get; set; }
        public string targetID { get; set; }
        public int targetSlot { get; set; }

        [ForeignKey("process_id")]
        public virtual ProcessModel process { get; set; }
    }

}
