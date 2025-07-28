using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vue.Models
{
    [Table("related_esign")]
    public class RelatedEsignModel
    {
        [Key]
        public int id { get; set; }
        public int esign_id { get; set; }
        public int related_id { get; set; }
        public string type { get; set; }

        public DateTime? created_at { get; set; }

    }

}
