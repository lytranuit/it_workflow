﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vue.Models
{
    [Table("document_type_receive")]
    public class DocumentTypeReceiveModel
    {
        public int id { get; set; }

        [Required]
        public int type_id { get; set; }

        [Required]
        public string user_id { get; set; }

        [JsonIgnore]
        [ForeignKey("type_id")]
        public DocumentTypeModel? type { get; set; }

    }
}
