using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using workflow.Models;

namespace Vue.Models
{

    [Table("AspNetUsers")]
    public class UserModel : IdentityUser
    {
        public string FullName { get; set; }
        public string? image_url { get; set; }
        public string? image_sign { get; set; }
        public string? signature { get; set; }
        public string? department_text { get; set; }
        public string? reportId { get; set; }
        public string? msnv { get; set; }

        public double? ngaynghi { get; set; }
        public DateTime? last_login { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }
        public List<UserDepartmentModel> departments { get; set; }


    }
}
