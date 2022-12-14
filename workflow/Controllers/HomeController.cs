
using it.Models;
using Microsoft.AspNetCore.Mvc;
using it.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net.Mime;
using System.Globalization;
using it.Services;
using it.Areas.Admin.Models;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Linq;
using elFinder.NetCore.Models;
//using Microsoft.
//using System.Runtime.InteropServices;
//using System.IO;
//using Microsoft.Office.Core;
namespace it.Controllers
{
	public class HomeController : Controller
	{

		protected readonly ItContext _context;

		private readonly IConfiguration _configuration;
		private readonly Workflow _workflow;
		public HomeController(IConfiguration configuration, ItContext context, Workflow workflow)
		{
			_configuration = configuration;
			_context = context;
			_workflow = workflow;
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

		public async Task<JsonResult> MailTask()
		{


			var blockings = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && d.clazz == "mailSystem").Take(10).ToList();
			foreach (var blocking in blockings)
			{
				var data_setting_block = blocking.data_setting;
				var mail_setting = data_setting_block.mail;
				mail_setting = _workflow.fillMail(mail_setting, blocking);

				data_setting_block.mail = mail_setting;

				blocking.data_setting = data_setting_block;
				blocking.blocking = false;
				blocking.created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
				blocking.created_at = DateTime.Now;

				var SuccesMail = SendMail(mail_setting.to, mail_setting.title, mail_setting.content, mail_setting.filecontent.Split(',').ToList());
				if (SuccesMail.success == 1)
				{
					blocking.executed = true;
				}
				else
				{
					blocking.failed = true;
				}
				_context.Update(blocking);
				await _context.SaveChangesAsync();

				_workflow.create_next(blocking);
			}

			return Json(new { success = true });
		}
		public async Task<JsonResult> PrintTask()
		{

			string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;

			var blockings = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && d.clazz == "printSystem").Take(10).ToList();
			foreach (var blocking in blockings)
			{
				await _workflow.PrintTask(blocking);
			}

			return Json(new { success = true });
		}
		private SuccesMail SendMail(string to, string subject, string body, List<string>? attachments = null)
		{
			try
			{

				string[] list_to = to.Split(",");
				list_to = list_to.Distinct().ToArray();
				System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
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

		public async Task<JsonResult> cronjob()
		{
			var emails = _context.EmailModel.Where(d => d.status == 1).Take(10).ToList();
			foreach (var email in emails)
			{
				var SuccesMail = SendMail(email.email_to, email.subject, email.body, email.data_attachments);
				if (SuccesMail.success == 1)
				{
					email.status = 2;
				}
				else
				{
					email.status = 3;
					email.error = SuccesMail.ex.ToString();
				}
				_context.Update(email);
			}
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}
	}

	class SuccesMail
	{
		public int success { get; set; }
		public Exception ex { get; set; }
	}
}
