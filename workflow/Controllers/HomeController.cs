
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using it.Services;
using Vue.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static Vue.Data.ItContext;
using System.Diagnostics;
using Spire.Doc;
using workflow.Areas.V1.Models;
using Vue.Models;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            var listener = _context.GetService<DiagnosticSource>();
            (listener as DiagnosticListener).SubscribeWithAdapter(new CommandInterceptor());
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
        public async Task<JsonResult> tinhngaynghi()
        {
            var process_id = "2qax2NMI4V9lBCnijIhwkYzblva0Wxlo";
            //var Process = _context.ProcessModel.Where(d => d.id == process_id).Include(d => d.versions).ThenInclude(d => d.executions).ThenInclude(d => d.user).FirstOrDefault();
            //if (Process == null)
            //	return Json("Không tìm thấy Qui trình");
            var ProcessVersion = _context.ProcessVersionModel.Where(d => d.process_id == process_id).Select(d => d.id).ToList();

            var ExecutionModel = _context.ExecutionModel.Where(d => d.deleted_at == null && ProcessVersion.Contains(d.process_version_id)).Include(d => d.fields).Include(d => d.user).ToList();
            var list_NghiPhep = new List<NghiphepModel>();
            if (ExecutionModel.Count > 0)
            {

                foreach (var execution in ExecutionModel)
                {
                    var NghiPhepModel = new NghiphepModel()
                    {
                        id = execution.id,
                        title = execution.title,
                        code = execution.code,
                        user_id = execution.user_id,
                        email = execution.user.Email,
                        status_id = execution.status_id
                    };
                    var fields = execution.fields;
                    var songaynghi = fields.Where(d => d.variable == "thoi_gian").FirstOrDefault();
                    if (songaynghi != null)
                    {
                        double value = double.Parse(songaynghi.values.value != null ? songaynghi.values.value : "0", CultureInfo.InvariantCulture.NumberFormat);
                        NghiPhepModel.songaynghi = value;
                    }
                    var tu_ngay = fields.Where(d => d.variable == "tu_ngay").FirstOrDefault();
                    if (tu_ngay != null)
                    {
                        DateTime value = DateTime.Parse(tu_ngay.values.value);
                        NghiPhepModel.tu_ngay = value;
                    }
                    var den_ngay = fields.Where(d => d.variable == "den_ngay").FirstOrDefault();
                    if (den_ngay != null)
                    {
                        DateTime value = DateTime.Parse(den_ngay.values.value);
                        NghiPhepModel.den_ngay = value;
                    }
                    var ly_do = fields.Where(d => d.variable == "ly_do").FirstOrDefault();
                    if (ly_do != null)
                    {
                        NghiPhepModel.ly_do = ly_do.values.value;
                    }

                    var loanghiphep = fields.Where(d => d.variable == "loaiphep").FirstOrDefault();
                    if (loanghiphep != null)
                    {
                        var options = loanghiphep.data_setting.options;
                        var list_value = loanghiphep.values.value_array;
                        var value = loanghiphep.values.value;
                        if (value == null)
                        {
                            value = list_value[0];
                        }
                        var value_option = options.Where(d => d.id == value).FirstOrDefault();
                        if (value_option != null)
                        {
                            var name = value_option.name;
                            var list_name = name.Split(" – ");
                            if (list_name.Length > 0)
                            {
                                NghiPhepModel.loaiphep = list_name[0];
                            }

                        }

                    }

                    var NghiPhepModel_old = _context.NghiphepModel.Where(d => d.id == NghiPhepModel.id).FirstOrDefault();
                    if (NghiPhepModel_old != null)
                    {
                        CopyValues(NghiPhepModel_old, NghiPhepModel);
                    }
                    else
                    {
                        _context.Add(NghiPhepModel);
                    }
                    _context.SaveChanges();

                    list_NghiPhep.Add(NghiPhepModel);


                }

                //foreach

            }

            return Json(new { success = true });

        }
        public void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }
        //public async Task<JsonResult> MailTask()
        //{


        //    var blockings = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && d.clazz == "mailSystem").Take(10).ToList();
        //    foreach (var blocking in blockings)
        //    {
        //        var data_setting_block = blocking.data_setting;
        //        var mail_setting = data_setting_block.mail;
        //        mail_setting = _workflow.fillMail(mail_setting, blocking);

        //        data_setting_block.mail = mail_setting;

        //        blocking.data_setting = data_setting_block;
        //        blocking.blocking = false;
        //        blocking.created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
        //        blocking.created_at = DateTime.Now;

        //        var SuccesMail = SendMail(mail_setting.to, mail_setting.title, mail_setting.content, mail_setting.filecontent.Split(',').ToList());
        //        if (SuccesMail.success == 1)
        //        {
        //            blocking.executed = true;
        //        }
        //        else
        //        {
        //            blocking.failed = true;
        //        }
        //        _context.Update(blocking);
        //        await _context.SaveChangesAsync();

        //        await _workflow.create_next(blocking);
        //    }

        //    return Json(new { success = true });
        //}

        public async Task<JsonResult> NotificationTask()
        {
            var blockings = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && d.status_notification == null).ToList();
            blockings = blockings.Where(d => d.data_setting.has_notification == true).ToList();
            foreach (var blocking in blockings)
            {
                var data_setting_block = blocking.data_setting;
                var mail_setting = data_setting_block.mail;
                mail_setting = _workflow.fillMail(mail_setting, blocking);
                var email = new EmailModel
                {
                    email_to = mail_setting.to,
                    subject = mail_setting.title,
                    body = mail_setting.content,
                    email_type = "NotificationTask",
                    data_attachments = mail_setting.filecontent != null ? mail_setting.filecontent.Split(',').ToList() : null,
                    status = 1
                };
                _context.Add(email);
                blocking.status_notification = 2;
                _context.Update(blocking);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, blockings = blockings });
        }
        public async Task<JsonResult> SyncEsign()
        {
            //var blockings = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && d.status_notification == null).ToList();
            //blockings = blockings.Where(d => d.data_setting.has_notification == true).ToList();
            //foreach (var blocking in blockings)
            //{

            //}
            var esigns = _context.DocumentSignatureModel.Where(d => d.date_workflow == null && d.block_id != null && d.status != 1).OrderByDescending(d => d.date).ToList();
            foreach (var esign in esigns)
            {
                ///Tim activity
                var ActivityModel = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && d.esign_id == esign.document_id && d.block_id == esign.block_id).FirstOrDefault();
                if (ActivityModel != null)
                {
                    if (esign.status == 2) /// Ký hoàn tất
                    {
                        ActivityModel.blocking = false;
                        ActivityModel.executed = true;
                        ActivityModel.failed = false;
                        ActivityModel.started_at = DateTime.Now;
                        ActivityModel.created_at = DateTime.Now;
                        ActivityModel.created_by = esign.user_sign;
                        _context.Update(ActivityModel);
                    }
                    else if (esign.status == 3)  /// Không ký
                    {
                        ActivityModel.blocking = false;
                        ActivityModel.executed = true;
                        ActivityModel.failed = true;

                        ActivityModel.note = esign.reason;
                        ActivityModel.created_at = DateTime.Now;
                        ActivityModel.created_by = esign.user_id;
                        ActivityModel.started_at = DateTime.Now;
                        _context.Update(ActivityModel);
                    }

                    /////Event
                    var user = _context.UserModel.Find(ActivityModel.created_by);
                    EventModel EventModel = new EventModel
                    {
                        execution_id = ActivityModel.execution_id,
                        event_content = "<b>" + user.FullName + "</b> đã thực hiện <b>" + ActivityModel.label + "</b>",
                        created_at = DateTime.Now,
                    };
                    _context.Add(EventModel);
                    await _context.SaveChangesAsync();

                    await _workflow.create_next(ActivityModel);
                }
                esign.date_workflow = DateTime.Now;
                _context.Update(esign);
                await _context.SaveChangesAsync();


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
                message.From = new MailAddress(_configuration["Mail:User"], _configuration["Mail:Name"]);
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
                var dir = _configuration["Source:Path_Private"].Replace("\\private", "").Replace("\\", "/");
                if (attachments != null)
                {
                    foreach (var attach in attachments)
                    {
                        if (System.IO.File.Exists(dir + attach) == false)
                            continue;
                        // Create  the file attachment for this email message.
                        Attachment data = new Attachment(dir + attach);
                        // Add time stamp information for the file.
                        System.Net.Mime.ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(attach);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attach);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(attach);
                        // Add the file attachment to this email message.

                        message.Attachments.Add(data);
                    }
                }

                SmtpClient client = new SmtpClient(_configuration["Mail:SMTP"], 587);
                //SmtpClient client = new SmtpClient("mail.pymepharco.com", 993);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_configuration["Mail:User"], _configuration["Mail:Pass"]);
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
            //var emails = _context.EmailModel.Where(d => d.status == 1).Take(10).ToList();
            //foreach (var email in emails)
            //{
            //    var SuccesMail = SendMail(email.email_to, email.subject, email.body, email.data_attachments);
            //    if (SuccesMail.success == 1)
            //    {
            //        email.status = 2;
            //    }
            //    else
            //    {
            //        email.status = 3;
            //        email.error = SuccesMail.ex.ToString();
            //    }
            //    _context.Update(email);
            //}
            //await _context.SaveChangesAsync();
            await SyncEsign();
            await NotificationTask();
            return Json(new { success = true });
        }
        //public IActionResult PrintEmployee()
        //{
        //    string authority = $"{(Request.IsHttps ? "https" : "http")}://{Request.Host}";
        //    string headerHtml = $"{authority}/template/nghiphep/ExportHeader.html";
        //    string footerHtml = $"{authority}/template/nghiphep/ExportFooter.html";

        //    return new ViewAsPdf("Employee")
        //    {
        //        //Model = viewModel,
        //        PageMargins = new Margins { Left = 10, Right = 10, Top = 25, Bottom = 20 },
        //        PageSize = Size.A4,
        //        PageOrientation = Orientation.Portrait,
        //        CustomSwitches = $"--header-html \"{headerHtml}\" --footer-html \"{footerHtml}\" --header-spacing 5 --footer-spacing 5"
        //    };
        //}
        public IActionResult Employee()
        {
            return View();
        }
    }

    class SuccesMail
    {
        public int success { get; set; }
        public Exception ex { get; set; }
    }
}
