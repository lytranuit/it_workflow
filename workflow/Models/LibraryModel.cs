using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Vue.Models
{
	[Table("Library")]
	public class LibraryModel
	{
		[Key]
		public int ItemId { get; set; }

		public string Name { get; set; }

		public int ParentID { get; set; }

		public Int64? Size { get; set; }
		public string? MimeType { get; set; }
		public string? user_id { get; set; }

		[ForeignKey("user_id")]
		public virtual UserModel? user { get; set; }
		public string? Type { get; set; }
		public string? FilterPath { get; set; }
		public bool? HasChild { get; set; }
		public bool? IsFile { get; set; }
		public bool? IsRoot { get; set; }

		public byte[]? Content { get; set; }
		public string? url { get; set; }
		public DateTime? DateCreated { get; set; }
		public DateTime? DateModified { get; set; }
		public DateTime? DateDeleted { get; set; }

	}
}
