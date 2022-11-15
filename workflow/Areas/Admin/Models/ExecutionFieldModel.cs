
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace it.Areas.Admin.Models
{
	[Table("e_field")]
	public class ExecutionFieldModel
	{
		[Key]
		public string id { get; set; }
		public string activity_id { get; set; }
		public int execution_id { get; set; }
		public string type { get; set; }
		public string name { get; set; }
		public bool? is_require { get; set; }
		public int? stt { get; set; }


		[ForeignKey("activity_id")]
		public virtual ActivityModel activity { get; set; }

		[ForeignKey("execution_id")]
		public virtual ExecutionModel execution { get; set; }
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

		public string? inputValues { get; set; }
		[NotMapped]
		public virtual InputValues? values
		{
			get
			{
				//Console.WriteLine(settings);
				return JsonConvert.DeserializeObject<InputValues>(string.IsNullOrEmpty(inputValues) ? "{}" : inputValues);
			}
			set
			{
				inputValues = JsonConvert.SerializeObject(value);
			}
		}
	}
	public class InputValues
	{
		public string? value { get; set; }
		public List<string>? value_array { get; set; }

		public List<Dictionary<string, string>>? list_data { get; set; }
		public List<FileUp>? files { get; set; }
	}

	public class FileUp
	{
		public string? name { get; set; }
		public string? url { get; set; }

		public string? ext { get; set; }
		public string? mimeType { get; set; }
	}
}
