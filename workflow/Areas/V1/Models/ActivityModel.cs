﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vue.Models;

namespace workflow.Areas.V1.Models
{
    [Table("e_activity")]
    public class ActivityModel
    {
        [Key]
        public string id { get; set; }

        public int execution_id { get; set; }
        public string? label { get; set; }
        public string? clazz { get; set; }
        public string block_id { get; set; }
        public string? variable { get; set; }

        public bool? executed { get; set; }

        public bool? failed { get; set; }

        public bool? blocking { get; set; }
        public int? status_notification { get; set; }
        public string? error_notification { get; set; }

        public int stt { get; set; }
        public string? note { get; set; }

        public List<ExecutionFieldModel>? fields { get; set; }

        public string? created_by { get; set; }
        public string? settings { get; set; }
        public int? esign_id { get; set; }
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

        [ForeignKey("execution_id")]
        public virtual ExecutionModel execution { get; set; }

        [ForeignKey("created_by")]
        public virtual UserModel? user_created_by { get; set; }
        public DateTime? started_at { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? deleted_at { get; set; }


    }

}
