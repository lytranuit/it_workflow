using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace it.Areas.Admin.Models
{
	[Table("e_event")]
	public class EventModel
	{
		public int id { get; set; }

		[Required]
		[StringLength(255)]
		public string event_content { get; set; }


		public int execution_id { get; set; }

		[ForeignKey("execution_id")]
		public virtual ExecutionModel? execution { get; set; }

		public int? type { get; set; }
		public DateTime? created_at { get; set; }

		public DateTime? updated_at { get; set; }

		public DateTime? deleted_at { get; set; }


	}
}
