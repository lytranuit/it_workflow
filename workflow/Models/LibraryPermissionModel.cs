using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Vue.Models
{
	[Table("Library_permission")]
	public class LibraryPermissionModel
	{
		[Key]
		public int id { get; set; }

		public string? path { get; set; }

		public string user_id { get; set; }

		public string permission { get; set; }

		[ForeignKey("user_id")]
		public virtual UserModel? user { get; set; }
		public int item_id { get; set; }

	}
}
