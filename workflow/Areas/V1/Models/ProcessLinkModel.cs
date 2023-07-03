using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workflow.Areas.V1.Models
{
    [Table("process_link")]
    public class ProcessLinkModel
    {
        [Key]
        public string id { get; set; }

        public string process_id { get; set; }
        public string source { get; set; }
        public int sourceAnchor { get; set; }
        public string target { get; set; }
        public int targetAnchor { get; set; }
        public string? label { get; set; }
        public bool reverse { get; set; }

        public string clazz { get; set; }

        [ForeignKey("process_id")]
        public virtual ProcessModel process { get; set; }
    }

}
