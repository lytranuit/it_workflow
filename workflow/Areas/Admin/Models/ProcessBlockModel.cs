using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
    [Table("process_block")]
    public class ProcessBlockModel
    {
        [Key]
        public string id { get; set; }
        public string process_id { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public int type_performer { get; set; }

        [ForeignKey("process_id")]
        public virtual ProcessModel process { get; set; }

        public virtual List<ProcessFieldModel> fields { get; set; }
        public string? settings { get; set; }
        [NotMapped]
        public virtual BlockSettings? data_setting
        {
            get
            {
                //Console.WriteLine(settings);
                return JsonConvert.DeserializeObject<BlockSettings>(string.IsNullOrEmpty(settings) ? "{}" : settings);
            }
            set
            {
                settings = JsonConvert.SerializeObject(value);
            }
        }
        public double? x { get; set; }

        public double? y { get; set; }
        public int? stt { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
    }
    public class BlockSettings
    {
    }
}
