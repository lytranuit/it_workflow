using it.Data;
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
        public bool? has_deadline { get; set; }
        public string? guide { get; set; }

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
        public int days { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public List<string> listuser { get; set; }

        public string block_id { get; set; }

        public List<string> blocks_approve_id { get; set; }

        public virtual List<ProcessBlockModel> blocks_approve
        {
            get;
            set;
        }

        public List<int> listdepartment { get; set; }


    }
}
