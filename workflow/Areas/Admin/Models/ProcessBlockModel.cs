using it.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace it.Areas.Admin.Models
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

		[ForeignKey("process_id")]
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
		public FileUp file_template { get; set; }
		public MailSetting mail { get; set; }
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
	public class MailSetting
	{
		public string to { get; set; }
		public string title { get; set; }
		public string content { get; set; }
		public string filecontent { get; set; }
	}
}
