using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace it.Areas.Admin.Models
{

	[Table("AspNetUsers")]
	public class UserModel : IdentityUser
	{
		public string FullName { get; set; }
		public string? image_url { get; set; }

		public DateTime? created_at { get; set; }

		public DateTime? updated_at { get; set; }

		public DateTime? deleted_at { get; set; }


	}
}
