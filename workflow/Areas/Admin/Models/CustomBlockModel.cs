using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace it.Areas.Admin.Models
{
	[Table("e_custom_block")]
	public class CustomBlockModel
	{
		[Key]
		public int id { get; set; }

		public int execution_id { get; set; }
		public string block_id { get; set; }

		public string? settings { get; set; }
		[NotMapped]
		public virtual CustomBlockSettings? data_setting
		{
			get
			{
				//Console.WriteLine(settings);
				return Newtonsoft.Json.JsonConvert.DeserializeObject<CustomBlockSettings>(string.IsNullOrEmpty(settings) ? "{}" : settings);
			}
			set
			{
				settings = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			}
		}
	}
	public class CustomBlockSettings
	{
		public int? type_performer { get; set; }
		public List<string> listuser { get; set; }
		public List<int> listdepartment { get; set; }
		public int days { get; set; }
		public int hours { get; set; }
		public int minutes { get; set; }
	}
}
