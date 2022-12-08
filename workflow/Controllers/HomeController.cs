
using it.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using it.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net.Mime;
using Spire.Presentation;
using System.Globalization;

namespace it.Controllers
{

	public class HomeController : Controller
	{

		protected readonly ItContext _context;

		private readonly IConfiguration _configuration;
		public HomeController(IConfiguration configuration, ItContext context)
		{
			_configuration = configuration;
			_context = context;
		}

		public IActionResult Index()
		{
			return Redirect("/Admin");
		}
		public IActionResult RemoveSignIn()
		{

			////Remove Cookie
			Response.Cookies.Delete(_configuration["JWT:NameCookieAuth"], new CookieOptions()
			{
				Domain = _configuration["JWT:Domain"]
			});
			return Redirect("/Admin");
		}
		public async Task<JsonResult> cronjob()
		{

			string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;

			var blockings = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && d.clazz == "mailSystem").Take(10).ToList();
			foreach (var blocking in blockings)
			{

				var ExecutionModel = _context.ExecutionModel.Where(d => d.id == blocking.execution_id && d.deleted_at == null)
					.Include(d => d.user)
					.Include(d => d.activities)
					.ThenInclude(d => d.user_created_by)
					.Include(d => d.activities)
					.ThenInclude(d => d.fields).FirstOrDefault();
				if (ExecutionModel == null)
					continue;
				Dictionary<string, string> replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
																  { "id", ExecutionModel.id.ToString()},
																  { "created_at", ExecutionModel.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss")},
																  { "created_by_name", ExecutionModel.user.FullName},
																  { "title", ExecutionModel.title},
															};

				Dictionary<string, string> replacements_email = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
																   { "created_by_name", ExecutionModel.user.Email},
															};

				Dictionary<string, string> replacements_file = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { };
				foreach (var activity in ExecutionModel.activities)
				{
					if (activity.user_created_by != null)
						replacements.Add("created_by_name_" + activity.block_id, activity.user_created_by.FullName);
					if (activity.created_at != null)
						replacements.Add("created_at_" + activity.block_id, activity.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss"));

					foreach (var field in activity.fields)
					{

						var data_setting = field.data_setting;
						var values = field.values;
						var text = values.value;
						if (field.type == "select")
						{
							var options = data_setting.options;
							var option = options.Where(d => d.id == values.value).FirstOrDefault();
							text = option.name;
							replacements.Add(field.variable, text);
						}
						else if (field.type == "department")
						{
							var department = _context.DepartmentModel.Where(d => d.id == Int32.Parse(values.value)).FirstOrDefault();
							text = department.name;
							replacements.Add(field.variable, text);
						}
						else if (field.type == "employee")
						{
							var employee = _context.UserModel.Where(d => d.Id == values.value).FirstOrDefault();
							text = employee.FullName;
							replacements.Add(field.variable, text);
							replacements_email.Add(field.variable, employee.Email);
						}
						else if (field.type == "date")
						{
							if (values.value != null)
							{
								var datetime = DateTime.Parse(values.value);
								text = datetime.ToString("yyyy-MM-dd");
								replacements.Add(field.variable, text);
							}

						}
						else if (field.type == "date_month")
						{
							if (values.value != null)
							{
								var datetime = DateTime.Parse(values.value);

								text = datetime.ToString("yyyy-MM");
								replacements.Add(field.variable, text);
							}
						}
						else if (field.type == "date_time")
						{
							if (values.value != null)
							{
								var datetime = DateTime.Parse(values.value);
								text = datetime.ToString("yyyy-MM-dd HH:mm:ss");
								replacements.Add(field.variable, text);
							}
						}
						else if (field.type == "file" || field.type == "file_multiple")
						{
							text = "";
							var list_file = new List<string>();
							foreach (var file in values.files)
							{
								text += "<a href='" + Domain + file.url + "'>" + file.name + "</a>";
								list_file.Add(file.url);
							}
							replacements.Add(field.variable, text);
							replacements_file.Add(field.variable, String.Join(",", list_file));
						}
						else if (field.type == "select_multiple")
						{
							var options = data_setting.options;
							var option = options.Where(d => values.value_array.Contains(d.id)).Select(d => d.name).ToList();
							text = String.Join(", ", option);
							replacements.Add(field.variable, text);
						}
						else if (field.type == "select_department")
						{
							var options = data_setting.options;
							var option = _context.DepartmentModel.Where(d => values.value_array.Contains(d.id.ToString())).Select(d => d.name).ToList();
							text = String.Join(", ", option);
							replacements.Add(field.variable, text);
						}
						else if (field.type == "select_employee")
						{
							var options = data_setting.options;
							var option = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.FullName).ToList();
							text = String.Join(", ", option);
							replacements.Add(field.variable, text);


							var option_email = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.Email).ToList();
							var text_email = String.Join(", ", option_email);
							replacements_email.Add(field.variable, text_email);
						}
						else if (field.type == "currency")
						{
							if (text == null)
								continue;
							CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
							text = double.Parse(text).ToString("#,###", cul.NumberFormat);
							replacements.Add(field.variable, text);

						}
						else if (field.type == "table")
						{
							var columns = data_setting.columns;
							var list_data = values.list_data;
							text = $"<table style='width:100%;border:1px solid #eaf0f7;border-collapse:collapse;'><thead style='background:aliceblue;'><tr>";
							foreach (var column in columns)
							{
								text += $"<td style='padding: 10px;border:1px solid white;'>{column.name}</td>";
							}
							text += "</thead></tr>";
							text += "<tbody>";
							foreach (var data in list_data)
							{
								text += "<tr>";
								foreach (var column in columns)
								{
									var value_column = data[column.id] ?? "";


									if (column.type == "currency")
									{
										CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
										value_column = double.Parse(value_column).ToString("#,###", cul.NumberFormat);
									}
									text += $"<td style='padding: 10px;border:1px solid #eaf0f7;'>{value_column}</td>";

								}
								text += "</tr>";
							}
							text += "</tbody>";
							text += "</table>";
							replacements.Add(field.variable, text);
						}
						else
						{
							replacements.Add(field.variable, text);
						}

					}
				}


				var data_setting_block = blocking.data_setting;
				var mail_setting = data_setting_block.mail;
				var firstChar = "!#";
				var lastChar = "#";
				var to = replacements_email.Aggregate(mail_setting.to, (current, value) =>
						current.Replace(firstChar + value.Key + lastChar, value.Value));
				var title = replacements.Aggregate(mail_setting.title, (current, value) =>
						current.Replace(firstChar + value.Key + lastChar, value.Value));
				var content = replacements.Aggregate(mail_setting.content, (current, value) =>
						current.Replace(firstChar + value.Key + lastChar, value.Value));
				var filecontent = mail_setting.filecontent != null ? replacements_file.Aggregate(mail_setting.filecontent, (current, value) =>
						current.Replace(firstChar + value.Key + lastChar, value.Value)) : null;

				data_setting_block.mail.to = to;
				data_setting_block.mail.title = title;
				data_setting_block.mail.content = content;
				data_setting_block.mail.filecontent = filecontent;

				blocking.data_setting = data_setting_block;
				blocking.blocking = false;
				blocking.created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
				blocking.created_at = DateTime.Now;

				var SuccesMail = SendMail(to, title, content, filecontent.Split(',').ToList());
				if (SuccesMail.success == 1)
				{
					blocking.executed = true;

				}
				else
				{
					blocking.failed = true;
				}
				_context.Update(blocking);
			}
			await _context.SaveChangesAsync();

			return Json(new { success = true });
		}

		private SuccesMail SendMail(string to, string subject, string body, List<string>? attachments = null)
		{
			try
			{

				string[] list_to = to.Split(",");
				list_to = list_to.Distinct().ToArray();
				MailMessage message = new MailMessage();
				message.From = new MailAddress("pymepharco.mail@gmail.com", "Pymepharco System");
				//message.From = new MailAddress("daolytran@pymepharco.com", "Pymepharco System");
				foreach (string str in list_to)
				{
					if (str == "")
						continue;
					message.To.Add(new MailAddress(str.Trim()));
				}

				message.Subject = subject;
				message.Body = body;
				message.BodyEncoding = System.Text.Encoding.UTF8;
				message.SubjectEncoding = System.Text.Encoding.UTF8;
				message.IsBodyHtml = true;
				if (attachments != null)
				{
					foreach (var attach in attachments)
					{
						if (System.IO.File.Exists("." + attach) == false)
							continue;
						// Create  the file attachment for this email message.
						Attachment data = new Attachment("." + attach);
						// Add time stamp information for the file.
						ContentDisposition disposition = data.ContentDisposition;
						disposition.CreationDate = System.IO.File.GetCreationTime(attach);
						disposition.ModificationDate = System.IO.File.GetLastWriteTime(attach);
						disposition.ReadDate = System.IO.File.GetLastAccessTime(attach);
						// Add the file attachment to this email message.

						message.Attachments.Add(data);
					}
				}

				SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
				//SmtpClient client = new SmtpClient("mail.pymepharco.com", 993);
				client.EnableSsl = true;
				client.UseDefaultCredentials = false;
				client.Credentials = new System.Net.NetworkCredential("pymepharco.mail@gmail.com", "xenrezrhmvueqmvw");
				//client.Credentials = new System.Net.NetworkCredential("daolytran@pymepharco.com", "Asd12345");
				client.Send(message);
			}
			catch (Exception ex)
			{
				return new SuccesMail { ex = ex, success = 0 };
			}
			return new SuccesMail { success = 1 };
		}

	}

	class SuccesMail
	{
		public int success { get; set; }
		public Exception ex { get; set; }
	}
}
