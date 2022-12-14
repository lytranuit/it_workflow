using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace it.Areas.Admin.Models
{
	[Table("process_version")]
	public class ProcessVersionModel
	{
		[Key]
		public string id { get; set; }
		public string? process_id { get; set; }
		public int version { get; set; }
		public bool? active { get; set; }
		public int? group_id { get; set; }

		public virtual List<ExecutionModel> executions { get; set; }
		[ForeignKey("group_id")]
		public ProcessGroupModel? group { get; set; }
		public string? json { get; set; }
		[NotMapped]
		public virtual ProcessModel? process
		{
			get
			{
				//Console.WriteLine(settings);
				return JsonSerializer.Deserialize<ProcessModel>(string.IsNullOrEmpty(json) ? "{}" : json);
			}
			set
			{
				json = JsonSerializer.Serialize(value, new JsonSerializerOptions()
				{
					ReferenceHandler = ReferenceHandler.IgnoreCycles
				});
			}
		}
		public DateTime? created_at { get; set; }

	}
}
