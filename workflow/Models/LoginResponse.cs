using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Vue.Models
{
	public class LoginResponse
	{
		[Key]
		public bool authed { get; set; }

		public string? error { get; set; }
		public string? parameter { get; set; }

		public string? session { get; set; }
		public string? user { get; set; }
		public string? token { get; set; }
	}
}
