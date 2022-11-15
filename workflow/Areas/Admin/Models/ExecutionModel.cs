using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.ComponentModel;

namespace it.Areas.Admin.Models
{
    [Table("execution")]
    public class ExecutionModel
    {
        [Key]
        public int id { get; set; }

        public string title { get; set; }
        public string user_id { get; set; }
        public int? status_id { get; set; }
        public string? status
        {
            get
            {
                if (status_id == null)
                    return "";
                ExecutionStatus ExecutionStatus = (ExecutionStatus)status_id;
                var DisplayName = ExecutionStatus.GetType()
                        .GetMember(ExecutionStatus.ToString())
                        .First()
                        .GetCustomAttributes<DisplayAttribute>().First();
                //Console.WriteLine(settings);
                return DisplayName.Name;
            }
        }
        public string process_version_id { get; set; }


        [ForeignKey("process_version_id")]
        public ProcessVersionModel process_version { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }

        [ForeignKey("user_id")]
        public UserModel user { get; set; }
    }

    enum ExecutionStatus
    {
        [Display(Name = "Nháp")]
        Draft = 1,
        [Display(Name = "Đang thực hiên")]
        Executing = 2,
        [Display(Name = "Hoàn thành")]
        Success = 3,
        [Display(Name = "Thất bại")]
        Fail = 4,
    }
}
