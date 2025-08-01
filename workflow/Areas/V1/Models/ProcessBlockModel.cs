﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vue.Models;

namespace workflow.Areas.V1.Models
{
    [Table("process_block")]
    public class ProcessBlockModel
    {
        [Key]
        public string id { get; set; }
        public string? process_id { get; set; }
        public string? label { get; set; }
        public string clazz { get; set; }
        public int? stt { get; set; }
        public int? type_performer { get; set; }
        public bool? has_deadline { get; set; }
        public string? guide { get; set; }
        public string? variable { get; set; }

        [ForeignKey("process_id")]
        [JsonIgnore]
        public ProcessModel process { get; set; }

        public virtual List<ProcessFieldModel>? fields { get; set; }
        public string? settings { get; set; }
        [NotMapped]
        public virtual BlockSettings? data_setting
        {
            get
            {
                //Console.WriteLine(settings);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<BlockSettings>(string.IsNullOrEmpty(settings) ? "{}" : settings);
            }
            set
            {
                settings = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }
        public double? x { get; set; }

        public double? y { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }

        [NotMapped]
        public int? start_c { get; set; }
    }
    public class BlockSettings
    {
        public int days { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public string type_template { get; set; }

        public string type_template_html { get; set; }
        public FileUp file_template { get; set; }

        public string type_output { get; set; }
        public string field_output { get; set; }
        /// <summary>
        /// (type_output == eisgn)
        /// </summary>
        public int esign_type_id { get; set; }
        /// <summary>
        /// (printTask)
        /// </summary>
        public Esign esign { get; set; }
        /// <summary>
        /// Gợi ý vị trí ký (suggestTask)
        /// </summary>
        public Dictionary<string, Signature> suggests { get; set; }

        /// <summary>
        /// Ký nháy (approveTask)
        /// </summary>
        public List<Signature> listusersign { get; set; }
        public bool has_notification { get; set; }
        /// <summary>
        /// (mailTask)
        /// </summary>
        public MailSetting mail { get; set; }
        public List<string> listuser { get; set; }

        public string block_id { get; set; }

        public List<string> blocks_approve_id { get; set; }
        public string blocks_esign_id { get; set; }

        public virtual List<ProcessBlockModel> blocks_approve { get; set; }
        public List<int> listdepartment { get; set; }
    }
    public class MailSetting
    {
        public string to { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string filecontent { get; set; }
    }
    public class Esign
    {
        public List<FileUp> files { get; set; }
        public List<Signature> signatures { get; set; }
    }
    public class Signature
    {

        public string block_id { get; set; }
        public string url { get; set; }
        public string user_sign { get; set; }
        public string user_esign { get; set; }
        public double position_x { get; set; }
        public double position_y { get; set; }
        public int page { get; set; }
        public DateTime date { get; set; }
        public string reason { get; set; }
        //public string image_sign { get; set; }
        public int status { get; set; } = 1;


        public double position_image_x { get; set; }
        public double position_image_y { get; set; }
        public double image_size_width { get; set; }
        public double image_size_height { get; set; }




    }

    public class Nghiphep
    {
        public string user_id { get; set; }
        public UserModel user { get; set; }
        public DateTime tu_ngay { get; set; }
        public DateTime den_ngay { get; set; }

        public double tongngaynghi { get; set; } = 0;

        public double tongngayphepdasudung { get; set; } = 0;

        public double tongngayphepconlai { get; set; } = 12;

        public List<string> loaiphep { get; set; }

        public string ly_do { get; set; }

    }
}
