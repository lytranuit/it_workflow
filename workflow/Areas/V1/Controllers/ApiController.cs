
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;



using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System.Reflection;
using it.Services;
using workflow.Areas.V1.Models;
using Vue.Data;
using Vue.Models;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace workflow.Areas.V1.Controllers
{
    public class ApiController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private RoleManager<IdentityRole> RoleManager;

        private readonly IConfiguration _configuration;
        private readonly Workflow _workflow;
        private readonly ViewRender _view;


        public ApiController(ItContext context, IConfiguration configuration, UserManager<UserModel> UserMgr, RoleManager<IdentityRole> RoleMgr, Workflow workflow, ViewRender view) : base(context)
        {
            _configuration = configuration;
            UserManager = UserMgr;
            RoleManager = RoleMgr;
            _workflow = workflow;
            _view = view;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<JsonResult> Employee()
        {
            var UserModel = _context.UserModel.Where(x => x.deleted_at == null).ToList();
            var list = new List<SelectResponse>();
            foreach (var user in UserModel)
            {
                var DepartmentResponse = new SelectResponse
                {

                    id = user.Id,
                    label = user.FullName + "(" + user.Email + ")",
                    name = user.FullName
                };
                list.Add(DepartmentResponse);
            }
            //var jsonData = new { data = ProcessModel };
            return Json(list, new System.Text.Json.JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }

        public async Task<JsonResult> Roles()
        {
            var Model = RoleManager.Roles.Select(a => new
            {
                id = a.Name,
                label = a.Name
            }).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(Model, new System.Text.Json.JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        public async Task<JsonResult> ProcessGroupWithProcess()
        {
            var ProcessGroupModel = _context.ProcessGroupModel.Where(x => x.deleted_at == null).Include(x => x.list_process_version.Where(d => d.active == true)).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(ProcessGroupModel);
        }

        public async Task<JsonResult> Process(string id)
        {
            var ProcessModel = _context.ProcessModel.Where(x => x.id == id).Include(x => x.blocks).ThenInclude(d => d.fields.OrderBy(x => x.stt)).Include(x => x.links).FirstOrDefault();
            //var jsonData = new { data = ProcessModel };
            return Json(ProcessModel);
        }
        public async Task<JsonResult> ProcessVersion(string id)
        {
            var ProcessVersionModel = _context.ProcessVersionModel.Where(x => x.id == id).FirstOrDefault();
            //var jsonData = new { data = ProcessModel };
            return Json(ProcessVersionModel);
        }

        public async Task<JsonResult> Execution(int id)
        {
            var ExecutionModel = _context.ExecutionModel.Where(x => x.id == id).Include(d => d.user).FirstOrDefault();
            //var jsonData = new { data = ProcessModel };
            return Json(ExecutionModel);
        }

        public async Task<JsonResult> TransitionByExecution(int execution_id)
        {
            var TransitionModel = _context.TransitionModel.Where(x => x.execution_id == execution_id && x.deleted_at == null).OrderBy(d => d.stt).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(TransitionModel);
        }

        public async Task<JsonResult> ActivityByExecution(int execution_id)
        {
            var ActivityModel = _context.ActivityModel.Where(x => x.execution_id == execution_id && x.deleted_at == null).Include(d => d.fields.OrderBy(d => d.stt)).Include(d => d.user_created_by).OrderBy(d => d.stt).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(ActivityModel);
        }

        public async Task<JsonResult> CustomBlockByExecution(int execution_id)
        {
            var CustomBlockModel = _context.CustomBlockModel.Where(x => x.execution_id == execution_id).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(CustomBlockModel);
        }
        public async Task<JsonResult> HomeBadge(string? process_id = null)
        {
            var c = _context.ProcessVersionModel.Where(d => d.deleted_at == null);
            if (process_id != null)
            {
                c = c.Where(d => d.process_id == process_id);
            }
            var list_process = c.Select(d => d.id).ToList();
            var count = _context.ExecutionModel.Where(d => d.deleted_at == null && list_process.Contains(d.process_version_id)).Count();
            var wait_count = _context.ExecutionModel.Where(d => d.deleted_at == null && list_process.Contains(d.process_version_id) && d.status_id == (int)ExecutionStatus.Executing).Count();
            var done_count = _context.ExecutionModel.Where(d => d.deleted_at == null && list_process.Contains(d.process_version_id) && d.status_id == (int)ExecutionStatus.Success).Count();
            var cancle_count = _context.ExecutionModel.Where(d => d.deleted_at == null && list_process.Contains(d.process_version_id) && d.status_id == (int)ExecutionStatus.Fail).Count();

            return Json(new { execution_success = done_count, execution_fail = cancle_count, execution_amount = count, execution_wait = wait_count });
        }
        [HttpPost]
        public async Task<JsonResult> tableUser()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var process_id = Request.Form["process_id"].FirstOrDefault();
            //System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            //string user_id = UserManager.GetUserId(currentUser); // Get user id:
            //var user_current = await UserManager.GetUserAsync(currentUser);
            //var subsql = "";
            //var sql = $"select type_id,COUNT(1) as num from document where deleted_at is null and status_id IN(4,5,6,7) {subsql} GROUP BY type_id";

            //var data = _context.ChartType
            //	 .FromSqlRaw(sql);
            //var d = data.Include(d => d.type).OrderByDescending(d => d.num).ToList();
            var c = _context.ProcessVersionModel.Where(d => d.deleted_at == null);
            if (process_id != null)
            {
                c = c.Where(d => d.process_id == process_id);
            }
            var list_process = c.Select(d => d.id).ToList();
            var blocking_activity = _context.ActivityModel.Include(d => d.execution).Where(d => d.execution.deleted_at == null && d.deleted_at == null && list_process.Contains(d.execution.process_version_id) && d.blocking == true).Select(d => d.block_id + d.execution_id).ToList();
            var custom_block = _context.CustomBlockModel.Where(d => blocking_activity.Contains(d.block_id + d.execution_id)).ToList();
            var list = new List<string>();
            foreach (var block in custom_block)
            {
                var data_setting = block.data_setting;
                if (data_setting.type_performer == 4 && data_setting.listuser != null)
                {
                    list.AddRange(data_setting.listuser);
                }
            }
            var groupedCustomerList = list
                .GroupBy(u => u)
                .Select(grp => new
                {
                    count = grp.Count(),
                    user = _context.UserModel.Find(grp.Key)
                })
                .Where(d => d.user != null);

            int recordsTotal = groupedCustomerList.Count();
            int recordsFiltered = groupedCustomerList.Count();
            var records = groupedCustomerList.OrderByDescending(d => d.count).Skip(skip)
                .ToList();
            var data = new ArrayList();

            foreach (var record in records)
            {
                var data1 = new
                {
                    user = record.user,
                    count = record.count,
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
            //return Json(new { labels = labels, datasets = datasets });
        }
        [HttpPost]
        public async Task<JsonResult> tableProcess()
        {

            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var process_id = Request.Form["process_id"].FirstOrDefault();
            //System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            //string user_id = UserManager.GetUserId(currentUser); // Get user id:
            //var user_current = await UserManager.GetUserAsync(currentUser);
            //var subsql = "";
            //var sql = $"select type_id,COUNT(1) as num from document where deleted_at is null and status_id IN(4,5,6,7) {subsql} GROUP BY type_id";

            //var data = _context.ChartType
            //	 .FromSqlRaw(sql);
            //var d = data.Include(d => d.type).OrderByDescending(d => d.num).ToList();
            var c = _context.ProcessVersionModel.Where(d => d.deleted_at == null);
            if (process_id != null)
            {
                c = c.Where(d => d.process_id == process_id);
            }
            var list_process = c.Select(d => d.id).ToList();
            var list = _context.ExecutionModel.Where(d => d.deleted_at == null && list_process.Contains(d.process_version_id));
            var groupedCustomerList = list
                .GroupBy(u => u.process_version_id)
                .Select(grp => new
                {
                    count = grp.Count(),
                    process_version_id = grp.Key,
                    process_version = _context.ProcessVersionModel.Where(d => d.id == grp.Key).FirstOrDefault(),
                }).ToList();



            var records = groupedCustomerList.OrderByDescending(d => d.count).Skip(skip)
                .ToList();

            int recordsTotal = groupedCustomerList.Count();
            int recordsFiltered = groupedCustomerList.Count();
            var data = new ArrayList();

            foreach (var record in records)
            {
                var process_version = record.process_version;
                var process = process_version.process;
                var data1 = new
                {
                    name = $"<a href='#'>{process.name}</a>",
                    version = process_version.version,
                    count = record.count,
                    id = process_version.id,
                    excel = $"<a href='/v1/process/exportVersion?process_version_id={process_version.id}' class='export'><i class=\"fas fa-download\"></i></a>",
                };
                data.Add(data1);
            }

            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
            //return Json(new { labels = labels, datasets = datasets });
        }

        [HttpPost]
        public async Task<JsonResult> tableExecution()
        {

            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var process_id = Request.Form["process_id"].FirstOrDefault();
            //System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            //string user_id = UserManager.GetUserId(currentUser); // Get user id:
            //var user_current = await UserManager.GetUserAsync(currentUser);
            //var subsql = "";
            //var sql = $"select type_id,COUNT(1) as num from document where deleted_at is null and status_id IN(4,5,6,7) {subsql} GROUP BY type_id";

            //var data = _context.ChartType
            //	 .FromSqlRaw(sql);
            //var d = data.Include(d => d.type).OrderByDescending(d => d.num).ToList();
            var c = _context.ProcessVersionModel.Where(d => d.deleted_at == null);
            if (process_id != null)
            {
                c = c.Where(d => d.process_id == process_id);
            }
            var list_process = c.Select(d => d.id).ToList();
            var list = _context.ExecutionModel.Where(d => d.deleted_at == null && list_process.Contains(d.process_version_id));

            var records = list.Include(d => d.user).OrderByDescending(d => d.created_at).Skip(skip)
                .ToList();

            int recordsTotal = list.Count();
            int recordsFiltered = list.Count();
            var data = new ArrayList();

            //foreach (var record in records)
            //{
            //    //var data1 = new
            //    //{
            //    //    name = $"<a href='#'>{process.name}</a>",
            //    //    version = process_version.version,
            //    //    count = record.count,
            //    //    id = process_version.id,
            //    //    excel = $"<a href='/v1/process/exportVersion?process_version_id={process_version.id}' class='export'><i class=\"fas fa-download\"></i></a>",
            //    //};
            //    //data.Add(data1);
            //}

            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = records };
            return Json(jsonData);
            //return Json(new { labels = labels, datasets = datasets });
        }
        public async Task<JsonResult> datachartDepartment(string? process_id = null)
        {
            var c = _context.ProcessVersionModel.Where(d => d.deleted_at == null);
            if (process_id != null)
            {
                c = c.Where(d => d.process_id == process_id);
            }
            var list_process = c.Select(d => d.id).ToList();
            var blocking_activity = _context.ActivityModel.Include(d => d.execution).Where(d => d.execution.deleted_at == null && d.deleted_at == null && list_process.Contains(d.execution.process_version_id) && d.blocking == true).Select(d => d.block_id + d.execution_id).ToList();
            var custom_block = _context.CustomBlockModel.Where(d => blocking_activity.Contains(d.block_id + d.execution_id)).ToList();
            var list = new List<int>();
            foreach (var block in custom_block)
            {
                var data_setting = block.data_setting;
                if (data_setting.type_performer == 3)
                {
                    list.AddRange(data_setting.listdepartment);
                }
            }
            var groupedCustomerList = list
                .GroupBy(u => u)
                .Select(grp => new
                {
                    count = grp.Count(),
                    department = _context.DepartmentModel.Find(grp.Key)
                })
                .Where(d => d.department != null)
                .OrderByDescending(d => d.count)
                .ToList();

            var labels = new List<string>() { };
            var datasets = new List<ChartDataSet>();
            var backgroundColor = new List<string>();
            var data1 = new List<int?>();
            foreach (var record in groupedCustomerList)
            {
                labels.Add(record.department.name);
                backgroundColor.Add(record.department.color);
                data1.Add(record.count);
            }
            datasets.Add(new ChartDataSet
            {
                backgroundColor = backgroundColor,
                data = data1
            });

            //var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            //return Json(jsonData);
            return Json(new { labels = labels, datasets = datasets });
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(CommentModel CommentModel)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            string user_id = UserManager.GetUserId(currentUser); // Get user id:
            var user = await UserManager.GetUserAsync(currentUser); // Get user id:
            CommentModel.user_id = user_id;
            CommentModel.created_at = DateTime.Now;
            _context.Add(CommentModel);
            _context.SaveChanges();
            var files = Request.Form.Files;

            if (files != null && files.Count > 0)
            {
                var pathroot = _configuration["Source:Path_Private"] + "\\executions\\" + CommentModel.execution_id + "\\";
                bool exists = Directory.Exists(pathroot);

                if (!exists)
                    Directory.CreateDirectory(pathroot);

                var items_comment = new List<CommentFileModel>();
                foreach (var file in files)
                {
                    var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    string name = file.FileName;
                    string ext = Path.GetExtension(name);
                    string mimeType = file.ContentType;

                    //var fileName = Path.GetFileName(name);
                    var newName = timeStamp + " - " + name;

                    newName = newName.Replace("+", "_");
                    newName = newName.Replace("%", "_");
                    newName = newName.Replace(",", "_");
                    var filePath = _configuration["Source:Path_Private"] + "\\executions\\" + CommentModel.execution_id + "\\" + newName;
                    string url = "/private/executions/" + CommentModel.execution_id + "/" + newName;
                    items_comment.Add(new CommentFileModel
                    {
                        ext = ext,
                        url = url,
                        name = name,
                        mimeType = mimeType,
                        comment_id = CommentModel.id,
                        created_at = DateTime.Now
                    });

                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileSrteam);
                    }
                }
                _context.AddRange(items_comment);
                _context.SaveChanges();
            }
            ////
            ///
            UserReadModel user_read = _context.UserReadModel.Where(d => d.execution_id == CommentModel.execution_id && d.user_id == CommentModel.user_id).FirstOrDefault();
            if (user_read == null)
            {
                user_read = new UserReadModel
                {
                    execution_id = CommentModel.execution_id,
                    user_id = CommentModel.user_id,
                    time_read = DateTime.Now,
                };
                _context.Add(user_read);
            }
            else
            {
                user_read.time_read = DateTime.Now;
                _context.Update(user_read);
            }

            ///create unread

            //var DocumentModel = _context.DocumentModel
            //            .Where(d => d.id == DocumentCommentModel.document_id)
            //            .Include(d => d.users_follow)
            //            .Include(d => d.users_signature)
            //            .Include(d => d.users_receive)
            //            .FirstOrDefault();

            //var users_follow = DocumentModel.users_follow.Select(a => a.user_id).ToList();
            //var users_signature = DocumentModel.users_signature.Select(a => a.user_id).ToList();
            //var users_representative = DocumentModel.users_signature.Where(a => a.representative_id != null).Select(a => a.representative_id).ToList();
            //var users_receive = DocumentModel.users_receive.Select(a => a.user_id).ToList();
            //List<string> users_related = new List<string>();
            //users_related.AddRange(users_follow);
            //users_related.AddRange(users_signature);
            //users_related.AddRange(users_representative);
            //users_related.AddRange(users_receive);
            //users_related = users_related.Distinct().ToList();
            //var itemToRemove = users_related.SingleOrDefault(r => r == user_id);
            //users_related.Remove(itemToRemove);
            //var items = new List<DocumentUserUnreadModel>();
            //foreach (string u in users_related)
            //{
            //    items.Add(new DocumentUserUnreadModel
            //    {
            //        user_id = u,
            //        document_id = DocumentModel.id,
            //        time = DateTime.Now,
            //    });
            //}
            //_context.AddRange(items);
            ////SEND MAIL
            //if (users_related != null)
            //{
            //    var users_related_obj = _context.UserModel.Where(d => users_related.Contains(d.Id)).Select(d => d.Email).ToList();
            //    var mail_string = string.Join(",", users_related_obj.ToArray());
            //    string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;
            //    var body = _view.Render("Emails/NewComment", new { link_logo = Domain + "/images/PMP_Stada_Group.png", link = Domain + "/admin/document/details/" + DocumentModel.id });
            //    var email = new EmailModel
            //    {
            //        email_to = mail_string,
            //        subject = "[Tin nhắn mới] " + DocumentModel.name_vi,
            //        body = body,
            //        email_type = "new_comment_document",
            //        status = 1
            //    };
            //    _context.Add(email);
            //}
            ////await _context.SaveChangesAsync();

            ///// Audittrail
            //var audit = new AuditTrailsModel();
            //audit.UserId = user.Id;
            //audit.Type = AuditType.Update.ToString();
            //audit.DateTime = DateTime.Now;
            //audit.description = $"Tài khoản {user.FullName} đã thêm bình luận.";
            //_context.Add(audit);
            //await _context.SaveChangesAsync();

            CommentModel.user = user;
            CommentModel.is_read = true;

            return Json(new
            {
                success = 1,
                comment = CommentModel
            });
        }
        public async Task<IActionResult> Events(int execution_id)
        {
            var events = _context.EventModel.Where(d => d.execution_id == execution_id).ToList();

            return Json(new { success = 1, events });
        }

        public async Task<IActionResult> MoreComment(int execution_id, int? from_id)
        {
            int limit = 10;
            var comments_ctx = _context.CommentModel
                .Where(d => d.execution_id == execution_id);
            if (from_id != null)
            {
                comments_ctx = comments_ctx.Where(d => d.id < from_id);
            }
            List<CommentModel> comments = comments_ctx.OrderByDescending(d => d.id)
                .Take(limit).Include(d => d.files).Include(d => d.user).ToList();
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            string current_user_id = UserManager.GetUserId(currentUser); // Get user id:
            var user_read = _context.UserReadModel.Where(d => d.user_id == current_user_id && d.execution_id == execution_id).FirstOrDefault();
            DateTime? time_read = null;
            if (user_read != null)
                time_read = user_read.time_read;

            foreach (var comment in comments)
            {
                if (comment.user_id == current_user_id)
                {
                    comment.is_read = true;
                    continue;
                }
                if (time_read != null && comment.created_at <= time_read)
                    comment.is_read = true;
            }
            return Json(new { success = 1, comments });
        }
        public async Task<JsonResult> Department()
        {
            var All = GetChild(0);
            //var jsonData = new { data = ProcessModel };
            return Json(All, new System.Text.Json.JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        public async Task<JsonResult> UserDepartments()
        {
            var All = GetChildUserDepartments(0);
            //var jsonData = new { data = ProcessModel };
            return Json(All, new System.Text.Json.JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        public async Task<JsonResult> UserInfo()
        {

            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string user_id = UserManager.GetUserId(currentUser); // Get user id:
            if (user_id == null)
                user_id = "1c8daf01-36a5-4be2-a9a3-47608e6c2096";
            UserModel User = _context.UserModel.Where(d => d.Id == user_id).Include(d => d.departments).FirstOrDefault();
            return Json(User);
        }
        public async Task<JsonResult> ProcessGroup()
        {
            var ProcessGroupModel = _context.ProcessGroupModel.Where(x => x.deleted_at == null).ToList();
            var list = new List<SelectResponse>();
            foreach (var item in ProcessGroupModel)
            {
                var Response = new SelectResponse
                {

                    id = item.id.ToString(),
                    label = item.name
                };
                list.Add(Response);
            }
            //var jsonData = new { data = ProcessModel };
            return Json(list);
        }
        private List<SelectResponse> GetChild(int parent)
        {
            var DepartmentModel = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == parent).OrderBy(d => d.stt).ToList();
            var list = new List<SelectResponse>();
            if (DepartmentModel.Count() > 0)
            {
                foreach (var department in DepartmentModel)
                {
                    var DepartmentResponse = new SelectResponse
                    {

                        id = department.id.ToString(),
                        label = department.name,
                        name = department.name,
                    };
                    var count_child = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == department.id).Count();
                    if (count_child > 0)
                    {
                        var child = GetChild(department.id);
                        DepartmentResponse.children = child;
                    }
                    list.Add(DepartmentResponse);
                }
            }
            return list;
        }
        private List<SelectResponse> GetChildUserDepartments(int parent)
        {
            var DepartmentModel = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == parent).OrderBy(d => d.stt).ToList();
            var list = new List<SelectResponse>();
            if (DepartmentModel.Count() > 0)
            {
                foreach (var department in DepartmentModel)
                {
                    //if (users.Count == 0)
                    //    continue;
                    var DepartmentResponse = new SelectResponse
                    {

                        id = department.id.ToString(),
                        label = "Bộ phận: " + department.name,
                        name = department.name,
                    };
                    //var count_child = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == department.id).Count();
                    //if (count_child > 0)
                    //{
                    var child = GetChildUserDepartments(department.id);
                    var users = _context.UserDepartmentModel.Where(d => d.department_id == department.id).Include(d => d.user).ToList();
                    if (users.Count() == 0 && child.Count() == 0)
                        continue;
                    foreach (var item in users)
                    {
                        var user = item.user;
                        child.Add(new SelectResponse
                        {

                            id = user.Id.ToString(),
                            label = user.FullName + "<" + user.Email + ">",
                            name = user.FullName,
                        });
                    }
                    if (child.Count() > 0)
                        DepartmentResponse.children = child;
                    //}
                    list.Add(DepartmentResponse);

                }
            }
            if (parent == 0)
            {
                var user_department = _context.UserDepartmentModel.GroupBy(d => d.user_id).Select(d => d.Key).ToList();
                var list_user_notin_department = _context.UserModel.Where(d => !user_department.Contains(d.Id) && d.deleted_at == null).ToList();
                foreach (var user in list_user_notin_department)
                {
                    //var user = item.user;
                    list.Add(new SelectResponse
                    {

                        id = user.Id.ToString(),
                        label = user.FullName + "<" + user.Email + ">",
                        name = user.FullName,
                    });
                }
            }
            return list;
        }
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<JsonResult> Saveprocess(string id, ProcessModel ProcessModel, List<ProcessBlockModel> blocks, List<ProcessLinkModel> links)
        {
            var fields = new List<ProcessFieldModel>();
            //if (id != ProcessModel.id)
            //{
            //    return Json(new { success = 0 });
            //}
            try
            {
                ProcessModel.blocks = null;
                ProcessModel.fields = null;
                ProcessModel.links = null;
                var ProcessModel_old = await _context.ProcessModel.FindAsync(id);
                if (ProcessModel_old == null)
                {
                    System.Security.Claims.ClaimsPrincipal currentUser = User;
                    string user_id = UserManager.GetUserId(currentUser); // Get user id:
                    ProcessModel.created_at = DateTime.Now;
                    ProcessModel.user_id = user_id;
                    ProcessModel.status_id = (int)ProcessStatus.Draft;
                    _context.Add(ProcessModel);
                }
                else
                {
                    CopyValues(ProcessModel_old, ProcessModel);
                    ProcessModel_old.updated_at = DateTime.Now;
                    ProcessModel_old.status_id = (int)ProcessStatus.Draft;
                    _context.Update(ProcessModel_old);
                }
                _context.SaveChanges();

                ///Block
                ///

                var blocks_old = _context.ProcessBlockModel.Where(d => d.process_id == ProcessModel.id).Select(a => a.id).ToList();
                var list_blocks = blocks.Select(block => block.id).ToList();
                IEnumerable<string> list_delete_block = blocks_old.Except(list_blocks);
                if (list_delete_block != null)
                {
                    var removeBlocks = _context.ProcessBlockModel.Where(d => list_delete_block.Contains(d.id)).ToList();
                    _context.RemoveRange(removeBlocks);
                }
                if (blocks.Count() > 0)
                {
                    foreach (ProcessBlockModel block in blocks)
                    {
                        if (block.fields != null)
                            fields.AddRange(block.fields);
                        block.fields = null;
                        block.process_id = ProcessModel.id;
                        var existing = _context.ProcessBlockModel.Where(d => d.id == block.id).FirstOrDefault();
                        if (existing == null)
                        {
                            block.created_at = DateTime.Now;
                            _context.Add(block);
                        }
                        else
                        {
                            CopyValues(existing, block);
                            _context.Update(existing);
                        }
                    }
                    //_context.SaveChanges();
                }




                ///Link
                ///
                var links_old = _context.ProcessLinkModel.Where(d => d.process_id == ProcessModel.id).Select(a => a.id).ToList();
                var list_links = links.Select(d => d.id).ToList();
                IEnumerable<string> list_delete = links_old.Except(list_links);
                if (list_delete != null)
                {
                    var removeLink = _context.ProcessLinkModel.Where(d => list_delete.Contains(d.id)).ToList();
                    _context.RemoveRange(removeLink);
                }
                if (links.Count() > 0)
                {
                    foreach (ProcessLinkModel link in links)
                    {
                        var existing = _context.ProcessLinkModel.Find(link.id);
                        if (existing == null)
                        {
                            link.process_id = ProcessModel.id;
                            _context.ProcessLinkModel.Add(link);
                        }
                        else
                        {
                            CopyValues(existing, link);
                            _context.ProcessLinkModel.Update(existing);
                        }
                    }
                }


                ///Fields
                var fields_old = _context.ProcessFieldModel.Where(d => d.process_id == ProcessModel.id).Select(a => a.id).ToList();
                var list_fields = fields.Select(d => d.id).ToList();
                IEnumerable<string> list_delete_fields = fields_old.Except(list_fields);
                if (list_delete_fields != null)
                {
                    var remove = _context.ProcessFieldModel.Where(d => list_delete_fields.Contains(d.id)).ToList();
                    _context.RemoveRange(remove);
                }
                if (fields.Count() > 0)
                {
                    int index = 0;
                    foreach (ProcessFieldModel field in fields)
                    {
                        field.block = null;
                        field.stt = index++;
                        field.settings = JsonConvert.SerializeObject(field.data_setting);
                        var existing = _context.ProcessFieldModel.Find(field.id);
                        if (existing == null)
                        {
                            field.process_id = ProcessModel.id;
                            _context.ProcessFieldModel.Add(field);
                        }
                        else
                        {
                            CopyValues(existing, field);
                            _context.ProcessFieldModel.Update(existing);
                        }
                    }
                }
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1 });

        }
        [HttpPost]
        public async Task<JsonResult> CreateExecution(ExecutionModel ExecutionModel)
        {
            try
            {
                var process_version_id = ExecutionModel.process_version_id;

                var process_version = _context.ProcessVersionModel.Where(d => d.id == process_version_id).FirstOrDefault();
                if (process_version != null)
                {
                    var process_id = process_version.process_id;
                    var process = process_version.process;
                    var list_version = _context.ProcessVersionModel.Where(d => d.process_id == process_id).Select(d => d.id).ToList();
                    var count = _context.ExecutionModel.Where(d => list_version.Contains(d.process_version_id)).Count();

                    System.Security.Claims.ClaimsPrincipal currentUser = User;
                    string user_id = UserManager.GetUserId(currentUser); // Get user id:
                    ExecutionModel.code = process.code + "-" + (count + 1);
                    ExecutionModel.user = null;
                    ExecutionModel.created_at = DateTime.Now;
                    ExecutionModel.user_id = user_id;
                    ExecutionModel.status_id = (int)ExecutionStatus.Executing;
                    _context.Add(ExecutionModel);
                    _context.SaveChanges();


                    /////create event
                    var user = _context.UserModel.Find(user_id);
                    EventModel EventModel = new EventModel
                    {
                        execution_id = ExecutionModel.id,
                        event_content = "<b>" + user.FullName + "</b> tạo mới",
                        created_at = DateTime.Now,
                    };
                    _context.Add(EventModel);
                    await _context.SaveChangesAsync();
                    return Json(new { success = 1, data = ExecutionModel });
                }
                else
                {
                    return Json(new { success = 0, message = "Không tìm thấy Qui trình" });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = 0, message = "" });
            }


        }
        [HttpPost]
        public async Task<JsonResult> UpdateExecution(int id, ExecutionModel ExecutionModel)
        {
            var ExecutionModel_old = await _context.ExecutionModel.FindAsync(id);
            if (ExecutionModel_old != null)
            {
                CopyValues(ExecutionModel_old, ExecutionModel);
                ExecutionModel_old.updated_at = DateTime.Now;
                _context.Update(ExecutionModel_old);
                _context.SaveChanges();
            }
            return Json(new { success = 1 });

        }

        [HttpPost]
        public async Task<JsonResult> CreateCustomBlock(CustomBlockModel CustomBlockModel, string event_type)
        {
            try
            {

                System.Security.Claims.ClaimsPrincipal currentUser = User;
                string user_id = UserManager.GetUserId(currentUser); // Get user id:
                _context.Add(CustomBlockModel);
                _context.SaveChanges();

                if (event_type == "reassignment")
                {
                    /////create event
                    var user = _context.UserModel.Find(user_id);
                    var event_content = "";
                    if (CustomBlockModel.data_setting.type_performer == 4)
                    {
                        var listuser = CustomBlockModel.data_setting.listuser;
                        var list = _context.UserModel.Where(d => listuser.Contains(d.Id)).Select(d => d.FullName).ToList();
                        event_content = "<b>" + user.FullName + "</b> đã phân công lại cho <b>" + string.Join(",", list) + "</b>";
                    }
                    else if (CustomBlockModel.data_setting.type_performer == 3)
                    {
                        var listdepartment = CustomBlockModel.data_setting.listdepartment;
                        var list = _context.DepartmentModel.Where(d => listdepartment.Contains(d.id)).Select(d => d.name).ToList();
                        event_content = "<b>" + user.FullName + "</b> đã phân công lại cho bộ phận <b>" + string.Join(",", list) + "</b>";

                    }
                    EventModel EventModel = new EventModel
                    {
                        execution_id = CustomBlockModel.execution_id,
                        event_content = event_content,
                        created_at = DateTime.Now,
                    };
                    _context.Add(EventModel);
                    await _context.SaveChangesAsync();




                    var block_id = CustomBlockModel.block_id;
                    var ActivityModel = _context.ActivityModel.Where(d => d.deleted_at == null && d.block_id == block_id && d.execution_id == CustomBlockModel.execution_id).FirstOrDefault();
                    if (ActivityModel == null)
                    {
                        goto end;
                    }
                    var data_setting = ActivityModel.data_setting;
                    var mail = data_setting.mail;
                    if (mail == null)
                    {
                        goto end;
                    }
                    mail = _workflow.fillMail(mail, ActivityModel);
                    //data_setting.mail = mail;
                    //ActivityModel.data_setting = data_setting;
                    //_context.Update(ActivityModel);

                    var email = new EmailModel
                    {
                        email_to = mail.to,
                        subject = mail.title,
                        body = mail.content,
                        data_attachments = mail.filecontent.Split(",").ToList(),
                        email_type = "forward_step",
                        status = 1
                    };
                    _context.Add(email);
                    await _context.SaveChangesAsync();

                }

end:
                Console.WriteLine("End");
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1, data = CustomBlockModel });

        }
        [HttpPost]
        public async Task<JsonResult> UpdateCustomBlock(int id, CustomBlockModel CustomBlockModel, string event_type)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            string user_id = UserManager.GetUserId(currentUser); // Get user id:
            var CustomBlockModel_old = await _context.CustomBlockModel.FindAsync(id);
            if (CustomBlockModel_old != null)
            {
                CopyValues(CustomBlockModel_old, CustomBlockModel);
                _context.Update(CustomBlockModel_old);
                _context.SaveChanges();
                if (event_type == "reassignment")
                {
                    /////create event
                    var user = _context.UserModel.Find(user_id);
                    var event_content = "";
                    if (CustomBlockModel_old.data_setting.type_performer == 4)
                    {
                        var listuser = CustomBlockModel_old.data_setting.listuser;
                        var list = _context.UserModel.Where(d => listuser.Contains(d.Id)).Select(d => d.FullName).ToList();
                        event_content = "<b>" + user.FullName + "</b> đã phân công lại cho <b>" + string.Join(",", list) + "</b>";
                    }
                    else if (CustomBlockModel_old.data_setting.type_performer == 3)
                    {
                        var listdepartment = CustomBlockModel_old.data_setting.listdepartment;
                        var list = _context.DepartmentModel.Where(d => listdepartment.Contains(d.id)).Select(d => d.name).ToList();
                        event_content = "<b>" + user.FullName + "</b> đã phân công lại cho bộ phận <b>" + string.Join(",", list) + "</b>";
                    }
                    EventModel EventModel = new EventModel
                    {
                        execution_id = CustomBlockModel_old.execution_id,
                        event_content = event_content,
                        created_at = DateTime.Now,
                    };
                    _context.Add(EventModel);
                    await _context.SaveChangesAsync();



                    var block_id = CustomBlockModel_old.block_id;
                    var ActivityModel = _context.ActivityModel.Where(d => d.deleted_at == null && d.block_id == block_id && d.execution_id == CustomBlockModel_old.execution_id).FirstOrDefault();
                    if (ActivityModel == null)
                    {
                        goto end;
                    }
                    var data_setting = ActivityModel.data_setting;
                    var mail = data_setting.mail;
                    if (mail == null)
                    {
                        goto end;
                    }
                    mail = _workflow.fillMail(mail, ActivityModel);
                    //data_setting.mail = mail;
                    //ActivityModel.data_setting = data_setting;
                    //_context.Update(ActivityModel);

                    var email = new EmailModel
                    {
                        email_to = mail.to,
                        subject = mail.title,
                        body = mail.content,
                        data_attachments = mail.filecontent.Split(",").ToList(),
                        email_type = "forward_step",
                        status = 1
                    };
                    _context.Add(email);

                    await _context.SaveChangesAsync();
                }
            }
end:
            Console.WriteLine("End");
            return Json(new { success = 1 });

        }

        [HttpPost]
        public async Task<JsonResult> CreateTransition(TransitionModel TransitionModel)
        {
            try
            {

                System.Security.Claims.ClaimsPrincipal currentUser = User;
                string user_id = UserManager.GetUserId(currentUser); // Get user id:
                TransitionModel.created_at = DateTime.Now;
                TransitionModel.created_by = user_id;
                _context.Add(TransitionModel);
                _context.SaveChanges();

                if (TransitionModel.reverse == true)
                {
                    var execution = _context.ExecutionModel.Where(d => d.id == TransitionModel.execution_id).Include(d => d.process_version).FirstOrDefault();
                    var process_version = execution.process_version;
                    var process = process_version.process;
                    var blocks = process.blocks;
                    var links = process.links;

                    var to_block_id = TransitionModel.to_block_id;
                    var to_activity_id = TransitionModel.to_activity_id;
                    var to_block = blocks.Where(d => d.id == to_block_id).FirstOrDefault();
                    var stt_to_block = to_block.stt;
                    var next_all_block = blocks.Where(d => d.stt >= stt_to_block).ToList();

                    foreach (var block in next_all_block)
                    {
                        var activities = _context.ActivityModel.Where(d => d.block_id == block.id && d.id != to_activity_id && d.execution_id == TransitionModel.execution_id).ToList();
                        if (activities != null)
                        {
                            foreach (var ac in activities)
                            {
                                ac.deleted_at = DateTime.Now;
                            }
                            _context.UpdateRange(activities);
                        }
                        var transitions = _context.TransitionModel.Where(d => d.from_block_id == block.id && d.execution_id == TransitionModel.execution_id).ToList();
                        if (transitions != null)
                        {
                            foreach (var ac in transitions)
                            {
                                ac.deleted_at = DateTime.Now;
                            }
                            _context.UpdateRange(transitions);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1, data = TransitionModel });

        }


        [HttpPost]
        public async Task<JsonResult> CreateActivity(ActivityModel ActivityModel, string event_type)
        {
            try
            {
                System.Security.Claims.ClaimsPrincipal currentUser = User;
                string user_id = UserManager.GetUserId(currentUser); // Get user id:
                string user_id_current = user_id;

                //if (ActivityModel.clazz == "parallelGateway" || ActivityModel.clazz == "inclusiveGateway" || ActivityModel.clazz == "success" || ActivityModel.clazz == "fail")
                //{
                //    user_id = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
                //}
                ActivityModel.created_at = DateTime.Now;
                ActivityModel.started_at = DateTime.Now;
                ActivityModel.created_by = user_id;
                if (ActivityModel.blocking == true)
                {
                    ActivityModel.created_at = null;
                    ActivityModel.created_by = null;
                }
                if (ActivityModel.failed == true)
                {
                    ActivityModel.fields = null;
                }
                if (ActivityModel.clazz == "printSystem")
                {
                    ActivityModel.created_by = user_id;
                }
                _context.Add(ActivityModel);
                _context.SaveChanges();


                if (ActivityModel.clazz == "fail")
                {
                    var ExecutionModel = _context.ExecutionModel.Find(ActivityModel.execution_id);
                    ExecutionModel.status_id = (int)ExecutionStatus.Fail;
                    _context.Update(ExecutionModel);
                    _context.SaveChanges();
                    /////create event
                    //var user = _context.UserModel.Find(user_id);
                    EventModel EventModel = new EventModel
                    {
                        execution_id = ActivityModel.execution_id,
                        event_content = "Đã thất bại",
                        type = 2,
                        created_at = DateTime.Now,
                    };
                    _context.Add(EventModel);

                    ////Gửi mail quy trình thất bại
                    var user_id_create = ExecutionModel.user_id;
                    var user_create = _context.UserModel.Find(user_id_create);
                    string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;
                    var body = _view.Render("Emails/Cancle", new
                    {
                        link_logo = Domain + "/images/clientlogo_astahealthcare.com_f1800.png",
                        link = Domain + "/execution/details/" + ExecutionModel.process_version_id + "?execution_id=" + ExecutionModel.id,
                        reason = ActivityModel.note
                    });

                    var email = new EmailModel
                    {
                        email_to = user_create.Email,
                        subject = "[Thất bại] " + ExecutionModel.title,
                        body = body,
                        email_type = "failed_execution",
                        status = 1
                    };
                    _context.Add(email);


                    //////Nếu có thông báo gửi mail
                    var data_setting = ActivityModel.data_setting;
                    if (data_setting.has_notification == true)
                    {
                        var mail = data_setting.mail;
                        if (mail == null)
                        {
                            goto end;
                        }
                        mail = _workflow.fillMail(mail, ActivityModel);
                        //data_setting.mail = mail;
                        //ActivityModel.data_setting = data_setting;
                        //_context.Update(ActivityModel);

                        var email1 = new EmailModel
                        {
                            email_to = mail.to,
                            subject = mail.title,
                            body = mail.content,
                            data_attachments = mail.filecontent.Split(",").ToList(),
                            email_type = "forward_step",
                            status = 1
                        };
                        _context.Add(email1);
                    }

                    await _context.SaveChangesAsync();
                }
                else if (ActivityModel.clazz == "success")
                {
                    var ExecutionModel = _context.ExecutionModel.Find(ActivityModel.execution_id);
                    ExecutionModel.status_id = (int)ExecutionStatus.Success;
                    _context.Update(ExecutionModel);
                    _context.SaveChanges();
                    /////create event
                    //var user = _context.UserModel.Find(user_id);
                    EventModel EventModel = new EventModel
                    {
                        execution_id = ActivityModel.execution_id,
                        event_content = "Đã hoàn thành",
                        created_at = DateTime.Now,
                    };
                    _context.Add(EventModel);

                    ////Gửi mail quy trình thành công đến người tạo quy trình
                    var user_id_create = ExecutionModel.user_id;
                    var user_create = _context.UserModel.Find(user_id_create);
                    string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;
                    var body = _view.Render("Emails/Success", new
                    {
                        link_logo = Domain + "/images/clientlogo_astahealthcare.com_f1800.png",
                        link = Domain + "/execution/details/" + ExecutionModel.process_version_id + "?execution_id=" + ExecutionModel.id,
                    });

                    var email = new EmailModel
                    {
                        email_to = user_create.Email,
                        subject = "[Hoàn thành] " + ExecutionModel.title,
                        body = body,
                        email_type = "success_execution",
                        status = 1
                    };
                    _context.Add(email);

                    //////Nếu có thông báo gửi mail
                    var data_setting = ActivityModel.data_setting;
                    if (data_setting.has_notification == true)
                    {
                        var mail = data_setting.mail;
                        if (mail == null)
                        {
                            goto end;
                        }
                        mail = _workflow.fillMail(mail, ActivityModel);
                        //data_setting.mail = mail;
                        //ActivityModel.data_setting = data_setting;
                        //_context.Update(ActivityModel);

                        var email1 = new EmailModel
                        {
                            email_to = mail.to,
                            subject = mail.title,
                            body = mail.content,
                            data_attachments = mail.filecontent.Split(",").ToList(),
                            email_type = "forward_step",
                            status = 1
                        };
                        _context.Add(email1);
                    }

                    await _context.SaveChangesAsync();
                }
                else if (ActivityModel.clazz == "printSystem")
                {
                    await _workflow.PrintTask(ActivityModel);
                }
                else if (ActivityModel.clazz == "outputSystem")
                {
                    await _workflow.OutputTask(ActivityModel);
                }
                else if (ActivityModel.blocking == false)
                {
                    /////create event
                    var user = _context.UserModel.Find(user_id);
                    EventModel EventModel = new EventModel
                    {
                        execution_id = ActivityModel.execution_id,
                        event_content = "<b>" + user.FullName + "</b> đã thực hiện <b>" + ActivityModel.label + "</b>",
                        created_at = DateTime.Now,
                    };
                    _context.Add(EventModel);
                    await _context.SaveChangesAsync();
                }


/////GỬI MAIL THEO MẪU
//if (ActivityModel.blocking == true)
//{
//    var data_setting = ActivityModel.data_setting;
//    var mail = data_setting.mail;
//    if (mail == null)
//    {
//        goto end;
//    }
//    mail = _workflow.fillMail(mail, ActivityModel);
//    //data_setting.mail = mail;
//    //ActivityModel.data_setting = data_setting;
//    //_context.Update(ActivityModel);

//    var email = new EmailModel
//    {
//        email_to = mail.to,
//        subject = mail.title,
//        body = mail.content,
//        data_attachments = mail.filecontent.Split(",").ToList(),
//        email_type = "forward_step",
//        status = 1
//    };
//    _context.Add(email);
//    await _context.SaveChangesAsync();
//}

/////

end:
                Console.WriteLine("End");
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1, data = ActivityModel });

        }

        [HttpPost]
        public async Task<JsonResult> UpdateActivity(string id, ActivityModel ActivityModel, string event_type, List<ExecutionFieldModel>? fields)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            string user_id = UserManager.GetUserId(currentUser); // Get user id:
            ActivityModel.user_created_by = null;
            ActivityModel.fields = null;
            var ActivityModel_old = await _context.ActivityModel.FindAsync(id);
            if (ActivityModel_old == null)
                return Json(new { success = 0 });
            var is_change_blocking = ActivityModel_old.blocking != ActivityModel.blocking;
            if (ActivityModel_old != null)
            {
                CopyValues(ActivityModel_old, ActivityModel);
                if (is_change_blocking)
                {
                    //if (ActivityModel_old.clazz == "parallelGateway" || ActivityModel_old.clazz == "inclusiveGateway" || ActivityModel_old.clazz == "success" || ActivityModel_old.clazz == "fail")
                    //{
                    //    user_id = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
                    //}
                    ActivityModel_old.created_at = DateTime.Now;
                    ActivityModel_old.created_by = user_id;

                    if (ActivityModel_old.blocking == true)
                    {
                        ActivityModel_old.created_at = null;
                        ActivityModel_old.created_by = null;
                    }

                }
                _context.Update(ActivityModel_old);
                _context.SaveChanges();

                if (fields.Count() > 0 && ActivityModel_old.failed == false)
                {
                    var fields_old = _context.ExecutionFieldModel.Where(d => d.activity_id == ActivityModel_old.id).ToList();
                    _context.RemoveRange(fields_old);
                    _context.SaveChanges();


                    _context.AddRange(fields);
                    _context.SaveChanges();
                }
                if (is_change_blocking && ActivityModel.blocking == false)
                {
                    /////create event
                    if (ActivityModel.failed == true)
                    {
                        var user = _context.UserModel.Find(user_id);
                        EventModel EventModel = new EventModel
                        {
                            execution_id = ActivityModel.execution_id,
                            event_content = "<b>" + user.FullName + "</b> đã thực hiện <b>" + ActivityModel.label + "</b> <div><span>Lý do: " + ActivityModel.note + "</span></div>",
                            type = 2,
                            created_at = DateTime.Now,
                        };
                        _context.Add(EventModel);
                        await _context.SaveChangesAsync();

                        //// delete all activities from next
                        //var next = 
                    }
                    else
                    {
                        ////ADD EVENT
                        var user = _context.UserModel.Find(user_id);
                        EventModel EventModel = new EventModel
                        {
                            execution_id = ActivityModel.execution_id,
                            event_content = "<b>" + user.FullName + "</b> đã thực hiện <b>" + ActivityModel.label + "</b>",
                            created_at = DateTime.Now,
                        };
                        _context.Add(EventModel);
                        await _context.SaveChangesAsync();
                    }


                    //// CHECK PREVIOUS
                    var prev_transitions = _context.TransitionModel.Where(d => d.to_activity_id == ActivityModel.id).ToList();
                    foreach (var transition in prev_transitions)
                    {

                        var prev_activity = _context.ActivityModel.Where(d => d.id == transition.from_activity_id).FirstOrDefault();
                        if (prev_activity != null && prev_activity.clazz == "exclusiveGateway")
                        {
                            var transitions = _context.TransitionModel.Where(d => d.from_activity_id == prev_activity.id).ToList();
                            foreach (var transition1 in transitions)
                            {
                                var activity = _context.ActivityModel.Where(d => d.id == transition1.to_activity_id).FirstOrDefault();
                                if (activity.blocking == true)
                                {
                                    _context.Remove(activity);
                                    _context.Remove(transition1);
                                    await _context.SaveChangesAsync();
                                }
                            }

                        }
                    }
                }


            }

            if (event_type == "require_sign")
            {
                /////create event
                var user = _context.UserModel.Find(user_id);
                var data_setting = ActivityModel_old.data_setting;

                var event_content = "";
                var listuser = data_setting.listusersign.Select(d => d.user_sign).ToList();
                var list = _context.UserModel.Where(d => listuser.Contains(d.Id)).Select(d => d.FullName).ToList();
                event_content = "<b>" + user.FullName + "</b> đã yêu cầu ký nháy <b>" + string.Join(",", list) + "</b>";

                EventModel EventModel = new EventModel
                {
                    execution_id = ActivityModel_old.execution_id,
                    event_content = event_content,
                    created_at = DateTime.Now,
                };
                _context.Add(EventModel);
                await _context.SaveChangesAsync();


                var mail = data_setting.mail;
                if (mail == null)
                {
                    goto end;
                }

                mail = _workflow.fillMail(mail, ActivityModel_old);

                var list1 = _context.UserModel.Where(d => listuser.Contains(d.Id)).Select(d => d.Email).ToList();
                var email = new EmailModel
                {
                    email_to = string.Join(",", list1),
                    subject = "[Yêu cầu ký nháy]" + mail.title,
                    body = mail.content,
                    data_attachments = mail.filecontent != null || mail.filecontent != "" ? mail.filecontent.Split(",").ToList() : null,
                    email_type = "forward_step",
                    status = 1
                };
                _context.Add(email);

                await _context.SaveChangesAsync();
            }
end:
            Console.WriteLine("End");
            return Json(new { success = 1 });

        }
        [HttpPost]
        public async Task<JsonResult> uploadFile(int execution_id)
        {

            var files = Request.Form.Files;

            var items = new List<FileUp>();
            if (files != null && files.Count > 0)
            {
                var pathroot = _configuration["Source:Path_Private"] + "\\executions\\" + execution_id + "\\";
                bool exists = Directory.Exists(pathroot);

                if (!exists)
                    Directory.CreateDirectory(pathroot);

                foreach (var file in files)
                {
                    var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    string name = file.FileName;
                    string ext = Path.GetExtension(name);
                    string mimeType = file.ContentType;

                    //var fileName = Path.GetFileName(name);
                    var newName = timeStamp + " - " + name;

                    newName = newName.Replace("+", "_");
                    newName = newName.Replace("%", "_");
                    newName = newName.Replace(",", "_");
                    var filePath = _configuration["Source:Path_Private"] + "\\executions\\" + execution_id + "\\" + newName;
                    string url = "/private/executions/" + execution_id + "/" + newName;
                    items.Add(new FileUp
                    {
                        ext = ext,
                        url = url,
                        name = name,
                        mimeType = mimeType
                    });

                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileSrteam);
                    }
                }
            }
            return Json(new { success = 1, list = items });
        }
        [HttpPost]
        public async Task<JsonResult> uploadFileTemplate()
        {

            var files = Request.Form.Files;
            var file = files[0];
            var pathroot = _configuration["Source:Path_Private"] + "\\templates\\";
            bool exists = Directory.Exists(pathroot);

            if (!exists)
                Directory.CreateDirectory(pathroot);

            var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string name = file.FileName;
            string ext = Path.GetExtension(name);
            string mimeType = file.ContentType;

            //var fileName = Path.GetFileName(name);
            var newName = timeStamp + " - " + name;

            newName = newName.Replace("+", "_");
            newName = newName.Replace("%", "_");
            newName = newName.Replace(",", "_");
            var filePath = _configuration["Source:Path_Private"] + "\\templates\\" + newName;
            string url = "/private/templates/" + newName;
            var item = new FileUp
            {
                ext = ext,
                url = url,
                name = name,
                mimeType = mimeType
            };

            using (var fileSrteam = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileSrteam);
            }


            return Json(new { success = 1, item });
        }
        [HttpPost]
        public async Task<JsonResult> WithDraw(string transition_id)
        {
            var transition = _context.TransitionModel.Where(d => d.id == transition_id).FirstOrDefault();
            transition.deleted_at = DateTime.Now;
            _context.Update(transition);
            var to_activity_id = transition.to_activity_id;
            var to_activity = _context.ActivityModel.Find(to_activity_id);
            if (to_activity != null)
            {
                to_activity.deleted_at = DateTime.Now;
                _context.Update(to_activity);
            }
            var from_activity_id = transition.from_activity_id;
            var from_activity = _context.ActivityModel.Where(d => d.id == from_activity_id).Include(d => d.fields).FirstOrDefault();
            if (from_activity != null)
            {
                if (from_activity.clazz == "approveTask" || from_activity.clazz == "formTask" || from_activity.clazz == "suggestTask")
                {
                    from_activity.created_by = null;
                    from_activity.created_at = null;
                    from_activity.executed = false;
                    from_activity.failed = false;
                    from_activity.blocking = true;
                    from_activity.fields = null;
                }
                _context.Update(from_activity);
            }
            _context.SaveChanges();

            /////create event
            System.Security.Claims.ClaimsPrincipal currentUser = User;
            string user_id = UserManager.GetUserId(currentUser); // Get user id:

            var user = _context.UserModel.Find(user_id);
            EventModel EventModel = new EventModel
            {
                execution_id = transition.execution_id,
                event_content = "<b>" + user.FullName + "</b> đã rút lại lượt thực hiện <b>" + transition.label + "</b> cho <b>" + from_activity.label + "</b>",
                created_at = DateTime.Now,
            };
            _context.Add(EventModel);
            await _context.SaveChangesAsync();
            return Json(new { success = 1 });
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
        [HttpPost]
        public async Task<JsonResult> SaveSign(Signature sign, string activity_esign_id)
        {
            var user_local = _context.UserModel.Where(d => d.Id == sign.user_sign).FirstOrDefault();
            if (user_local == null)
                return Json(new { success = 0 });
            ///// Request get user info esign
            var client = new HttpClient();

            //var url = _configuration["JWT:ValidIssuer"] + "/api/UserInfo?email=" + user_local.Email;
            //var response = await client.GetAsync(url);
            //EsignResponse user = await response.Content.ReadFromJsonAsync<EsignResponse>();

            /////
            var dir = _configuration["Source:Path_Private"].Replace("\\private", "").Replace("\\", "/");
            var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string fileName = Path.GetFileNameWithoutExtension(sign.url);
            string forlder = Path.GetDirectoryName(sign.url);
            string ext = Path.GetExtension(user_local.image_sign);
            string save = "\\" + forlder.Substring(1) + "\\" + timeStamp + ".pdf";
            string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value + "/";
            //Draw the image
            var file_image = dir + user_local.image_sign;
            //PdfImage pdfImage = PdfImage.FromFile("." + user.image_sign);
            ImageData da = ImageDataFactory.Create(file_image);
            int image_size_width = (int)Math.Round((float)sign.image_size_width);
            int image_size_height = (int)Math.Round((float)sign.image_size_height);
            if (ext.ToLower() == ".png")
            {
                using (System.Drawing.Image src = System.Drawing.Image.FromFile(dir + user_local.image_sign))
                using (Bitmap dst = new Bitmap(image_size_width, image_size_height))
                using (Graphics g = Graphics.FromImage(dst))
                {
                    //g.Clear(Color.White);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(src, 0, 0, dst.Width, dst.Height);
                    MemoryStream ms = new MemoryStream();
                    dst.Save(ms, ImageFormat.Png);
                    da = ImageDataFactory.CreatePng(ms.ToArray());
                }
                //pdfImage = PdfImage.FromFile("wwwroot\\temp\\" + timeStamp + "png");
            }
            // os = new FileStream(dest, FileMode.Create, FileAccess.Write);

            //Activate MultiSignatures
            //To disable Multi signatures uncomment this line : every new signature will invalidate older ones !
            //stamper = PdfStamper.CreateSignature(reader, os, '\0');
            var dest = new PdfWriter(_configuration["Source:Path_Private"].Replace("\\private", "") + save);
            var reader = new PdfReader(dir + sign.url);

            PdfSignerNoObjectStream signer = new PdfSignerNoObjectStream(reader, dest, new StampingProperties().UseAppendMode());
            // Creating the appearance
            FontProgram fontProgram = FontProgramFactory.CreateFont("wwwroot/assets/fonts/vuArial.ttf");
            PdfFont font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H);
            var width = (int)sign.image_size_width;
            var heigth = (int)sign.image_size_height;

            if (width < 180)
                width = 180;
            heigth += 40;
            if (sign.reason != null)
            {
                heigth += 30;
            }

            iText.Kernel.Geom.Rectangle rect = new iText.Kernel.Geom.Rectangle((int)sign.position_x, (int)sign.position_y, width, heigth);
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance()
                .SetReuseAppearance(false)
                .SetPageRect(rect)
                .SetPageNumber(sign.page);


            if (sign.reason != null)
            {
                appearance = appearance.SetReason(sign.reason);
            }
            PdfFormXObject layer2 = appearance.GetLayer2();
            PdfDocument doc = signer.GetDocument();
            PdfCanvas canvas = new PdfCanvas(layer2, doc);

            int p_y = 0;
            p_y += 40;
            var text = user_local.FullName + "\n" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (sign.reason != null)
            {
                text += "\nÝ kiến: " + sign.reason;
                p_y += 30;
            }
            iText.Kernel.Geom.Rectangle signatureRect = new iText.Kernel.Geom.Rectangle(0, 0, 180, p_y);
            Canvas signLayoutCanvas = new Canvas(canvas, signatureRect);
            Paragraph paragraph = new Paragraph(text).SetFont(font).SetMargin(0).SetMultipliedLeading(1.2f).SetFontSize(10);
            Div div = new Div();
            div.SetHeight(signatureRect.GetHeight());
            div.SetWidth(signatureRect.GetWidth());
            div.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.TOP);
            div.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
            div.Add(paragraph);
            signLayoutCanvas.Add(div);




            iText.Kernel.Geom.Rectangle dataRect = new iText.Kernel.Geom.Rectangle(0, p_y, (float)sign.image_size_width, rect.GetHeight() - p_y);
            Canvas dataLayoutCanvas = new Canvas(canvas, dataRect);
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(da);
            image.SetAutoScale(true);
            Div dataDiv = new Div();
            dataDiv.SetHeight(dataRect.GetHeight());
            dataDiv.SetWidth(dataRect.GetWidth());
            dataDiv.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
            dataDiv.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
            dataDiv.Add(image);
            dataLayoutCanvas.Add(dataDiv);




            signer.SetFieldName(timeStamp.ToString());
            // Creating the signature
            string KEYSTORE = dir + user_local.signature;
            char[] PASSWORD = "!PMP_it123456".ToCharArray();

            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(KEYSTORE,
            FileMode.Open, FileAccess.Read), PASSWORD);
            string alias = null;
            foreach (object a in pk12.Aliases)
            {
                alias = (string)a;
                if (pk12.IsKeyEntry(alias))
                {
                    break;
                }
            }
            ICipherParameters pk = pk12.GetKey(alias).Key;
            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            X509Certificate[] chain = new X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
            {
                chain[k] = ce[k].Certificate;
            }
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);

            signer.SignDetached(pks, chain, null, null, null, 0,
                    PdfSigner.CryptoStandard.CMS);

            dest.Close();
            sign.date = DateTime.Now;
            sign.status = 2;

            ///Save DB
            ///// Cap nhat user_sign
            var activity_esign = _context.ActivityModel.Where(d => d.id == activity_esign_id).FirstOrDefault();
            var data_setting = activity_esign.data_setting;
            var files = data_setting.esign.files;
            var signatures = data_setting.esign.signatures;
            if (signatures == null)
            {
                signatures = new List<Signature>();
            }
            files.Add(new FileUp()
            {
                name = files[0].name,
                url = save.Replace("\\", "/"),
                ext = ".pdf",
                mimeType = "application/pdf"
            });
            signatures.Add(sign);
            data_setting.esign.files = files;
            data_setting.esign.signatures = signatures;
            activity_esign.data_setting = data_setting;
            _context.Update(activity_esign);
            _context.SaveChanges();
            /////Cập nhật activity
            //if (activity_id != null)
            //{
            //	var activity = _context.ActivityModel.Where(d => d.id == activity_id).FirstOrDefault();
            //	var data_setting1 = activity.data_setting;
            //	var listusersign = data_setting1.listusersign;
            //	if (listusersign != null)
            //	{
            //		var findIndex = listusersign.FindIndex(d => d.user_sign == user_local.Id);
            //		if (findIndex != -1)
            //		{
            //			listusersign[findIndex] = sign;
            //			data_setting1.listusersign = listusersign;
            //			activity.data_setting = data_setting1;
            //			_context.Update(activity);
            //			_context.SaveChanges();
            //		}
            //	}

            //}

            return Json(new { success = 1, sign });
        }

        public async Task<JsonResult> process_data(string process_id)
        {
            //var Process = _context.ProcessModel.Where(d => d.id == process_id).Include(d => d.versions).ThenInclude(d => d.executions).ThenInclude(d => d.user).FirstOrDefault();
            //if (Process == null)
            //	return Json("Không tìm thấy Qui trình");
            var ProcessVersion = _context.ProcessVersionModel.Where(d => d.process_id == process_id).Select(d => d.id).ToList();

            var ExecutionModel = _context.ExecutionModel.Where(d => d.deleted_at == null && ProcessVersion.Contains(d.process_version_id)).Include(d => d.fields).ToList();

            //if (ProcessVersion.Count > 0)
            //{
            //	foreach (var version in ProcessVersion)
            //	{
            //		var executions = version.executions.Where(d => d.deleted_at == null).ToList();
            //		if (executions.Count() == 0)
            //			continue;
            //	}
            //}

            return Json(ExecutionModel);

        }
    }
    public class SelectResponse
    {
        public string id { get; set; }
        public string label { get; set; }

        public string name { get; set; }
        public virtual List<SelectResponse> children { get; set; }
    }
    public class EsignResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string image_url { get; set; }
        public string image_sign { get; set; }
        public bool is_sign { get; set; }
        public string position { get; set; }
        public string FullName { get; set; }
    }
    public class ChartDataSet
    {
        [Key]
        public string label { get; set; }

        public List<string> backgroundColor { get; set; }
        public List<int?> data { get; set; }

    }
    public class PdfSignerNoObjectStream : PdfSigner
    {
        public PdfSignerNoObjectStream(PdfReader reader, Stream outputStream, StampingProperties properties) : base(reader, outputStream, properties)
        {
        }

        protected override PdfDocument InitDocument(PdfReader reader, PdfWriter writer, StampingProperties properties)
        {
            try
            {
                return base.InitDocument(reader, writer, properties);
            }
            finally
            {
                if (reader.HasHybridXref())
                {
                    FieldInfo propertiesField = typeof(PdfWriter).GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    WriterProperties writerProperties = (WriterProperties)propertiesField.GetValue(writer);
                    writerProperties.SetFullCompressionMode(false);
                }
            }
        }
    }

}
