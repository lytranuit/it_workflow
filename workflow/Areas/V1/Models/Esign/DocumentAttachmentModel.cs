﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Vue.Models
{
    [Table("document_attachment")]
    public class DocumentAttachmentModel
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        [StringLength(255)]
        public string url { get; set; }
        [StringLength(50)]
        public string ext { get; set; }
        [StringLength(255)]
        public string mimeType { get; set; }

        public int document_id { get; set; }

        [JsonIgnore]
        [ForeignKey("document_id")]
        public virtual DocumentModel? document { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }


    }
}
