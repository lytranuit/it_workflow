
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace it.Areas.Admin.Models
{
    [Table("process_field")]
    public class ProcessFieldModel
    {
        [Key]
        public string id { get; set; }
        public string process_id { get; set; }
        public string process_block_id { get; set; }
        public string? description { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string? guide { get; set; }
        public bool? is_require { get; set; }
        public bool? has_default { get; set; }
        public int? stt { get; set; }

        [ForeignKey("process_id")]
        public virtual ProcessModel process { get; set; }
        [ForeignKey("process_block_id")]
        public virtual ProcessBlockModel block { get; set; }

        public string? settings { get; set; }
        [NotMapped]
        public virtual FieldSettings? data_setting
        {
            get
            {
                //Console.WriteLine(settings);
                return JsonConvert.DeserializeObject<FieldSettings>(string.IsNullOrEmpty(settings) ? "{}" : settings);
            }
            set
            {
                settings = JsonConvert.SerializeObject(value);
            }
        }
        //public DateTime? created_at { get; set; }

        //public DateTime? updated_at { get; set; }

        //public DateTime? deleted_at { get; set; }
    }
    public class FieldSettings
    {
        public string? default_value { get; set; }
        public List<string>? default_value_array { get; set; }
        public List<Option>? options { get; set; }
        public List<Column>? columns { get; set; }
    }
    public class Option
    {
        public string id { get; set; }
        public string name { get; set; }
        public int? stt { get; set; }
    }
    public class Column
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int? stt { get; set; }
        public bool is_require { get; set; }
    }
}
