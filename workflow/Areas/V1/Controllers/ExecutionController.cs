using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.AspNetCore.Identity;
using workflow.Services;
using Newtonsoft.Json;
using workflow.Areas.V1.Models;
using Vue.Models;
using Vue.Data;

namespace workflow.Areas.V1.Controllers
{
    public class ExecutionController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private string _type = "Execution";
        public ExecutionController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        {
            ViewData["controller"] = _type;
            UserManager = UserMgr;
        }

        // GET: Admin/Execution
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UserDone()
        {
            return View();
        }
        public IActionResult Wait(string user_id)
        {
            ViewBag.user_id = user_id;
            return View();
        }


        // GET: Admin/Execution/Create
        public IActionResult Create(string? id)
        {
            var ProcessVersionModel = _context.ProcessVersionModel.Where(d => d.id == id).FirstOrDefault();
            if (ProcessVersionModel == null)
                return NotFound();
            ViewBag.id = id;
            return View();
        }

        // GET: Admin/Execution/Edit/5
        public IActionResult Details(string? id, int? execution_id)
        {
            var ExecutionModel = _context.ExecutionModel.Where(d => d.id == execution_id).FirstOrDefault();
            if (ExecutionModel == null)
                return NotFound();
            var process_version_id = ExecutionModel.process_version_id;
            if (process_version_id != id)
                return NotFound();
            ViewBag.execution_id = execution_id;
            ViewBag.process_version_id = process_version_id;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Table()
        {
            var type = Request.Form["type"].FirstOrDefault();
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var user_id = Request.Form["user_id"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var status_id_text = Request.Form["filters[status_id]"].FirstOrDefault();
            var id_text = Request.Form["filters[id]"].FirstOrDefault();
            var title = Request.Form["filters[title]"].FirstOrDefault();

            int status_id = status_id_text != null ? Convert.ToInt32(status_id_text) : 0;
            int id = id_text != null ? Convert.ToInt32(id_text) : 0;
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            if (user_id == null || user_id == "")
                user_id = UserManager.GetUserId(currentUser); // Get user id:
            var user_current = await UserManager.GetUserAsync(currentUser); // Get user id:
            var customerData = _context.ExecutionModel.Where(m => m.deleted_at == null);
            //
            if (type == "wait")
            {
                var user_departments = _context.UserDepartmentModel.Where(d => d.user_id == user_id).Select(d => d.department_id).ToList();
                var list_exe = _context.CustomBlockModel.ToList();
                var list = list_exe.Where(d => (d.data_setting.type_performer == 4 && d.data_setting.listuser != null && d.data_setting.listuser.Contains(user_id))
                || (d.data_setting.type_performer == 3 && check_department(d.data_setting.listdepartment, user_departments))).Select(d => d.block_id + d.execution_id).ToList();
                var execution = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && list.Contains(d.block_id + d.execution_id)).Select(d => d.execution_id).ToList();
                customerData = customerData.Where(d => execution.Contains(d.id));
            }
            if (type == "user_done")
            {
                var execution = _context.ActivityModel.Where(d => d.executed == true && d.created_by == user_id && d.deleted_at == null).Select(d => d.execution_id).ToList();
                customerData = customerData.Where(d => execution.Contains(d.id));
            }
            int recordsTotal = customerData.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                customerData = customerData.Where(m => m.title.Contains(searchValue));
            }
            if (id > 0)
            {
                customerData = customerData.Where(d => d.id == id);
            }
            if (title != null && title != "")
            {
                customerData = customerData.Where(d => d.title.Contains(title));
            }

            if (status_id > 0)
            {
                customerData = customerData.Where(d => d.status_id == status_id);
            }
            int recordsFiltered = customerData.Count();
            //customerData = customerData.Include(m => m.group);
            var datapost = customerData.Include(d => d.process_version).Include(d => d.activities).Include(d => d.user).OrderByDescending(d => d.id).Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();

            foreach (var record in datapost)
            {

                var activities = record.activities.Where(d => d.deleted_at == null).OrderBy(d => d.stt).Select(d => new ActivityModel { id = d.id, label = d.label, blocking = d.blocking, failed = d.failed, }).ToList();
                var list_blocking = activities.Where(d => d.blocking == true).ToList();

                //var group = record.group;
                var html_process = record.process_version.process.name;
                //var html_status = $"<div class='status status_{record.status_id}'>{record.status}</div>";
                var html_user = record.user.FullName;
                var html_date = ((DateTime)record.created_at).ToString("dd/MM/yyyy");
                //var html_progress = "<div style='position: relative; margin:0 auto;' id='conta_" + record.id + "'>";
                //if (activities.Count > 0)
                //{
                //    foreach (var activity in activities)
                //    {
                //        var Iclass = "e_activity";
                //        if (activity.blocking == true)
                //        {
                //            Iclass += " e_activity_blocking";
                //        }
                //        else if (activity.failed == true)
                //        {
                //            Iclass += " e_activity_fail";
                //        }
                //        else
                //        {
                //            Iclass += " e_activity_success";
                //        }
                //        html_progress += $"<span data-toggle='tooltip' data-html='true' data-container='#conta_{record.id}' data-placement='top' title='{activity.label}' class='{Iclass}'></span>";
                //    }
                //}
                //else
                //{
                //    html_progress = "--";
                //}
                //html_progress += "</div>";
                var html_action = "<div class='btn-group'>";
                html_action += "<a href='/admin/" + _type + "/delete/" + record.id + "' class='' title='Xóa?' data-type='confirm'>"
                        + "<i class='fas text-danger font-16 fa-trash-alt'>"
                        + "</i>"
                        + "</a>";
                html_action += "</div>";
                var data1 = new
                {
                    action = html_action,
                    process_version_id = record.process_version_id,
                    id = record.id,
                    title = record.title,
                    status = record.status,
                    status_id = record.status_id,
                    activities = activities,
                    user_create = html_user,
                    date_create = html_date,
                    process = html_process
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            if (_context.ExecutionModel == null)
            {
                return Json(new { success = false });
            }
            var ExecutionModel = await _context.ExecutionModel.FindAsync(id);
            if (ExecutionModel != null)
            {
                ExecutionModel.deleted_at = DateTime.Now;
                _context.ExecutionModel.Update(ExecutionModel);
            }

            _context.SaveChanges();
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

        public bool check_department(List<int> departments, List<int> in_departments)
        {
            if (departments == null)
                return false;
            foreach (var department in departments)
            {
                if (in_departments.Contains(department))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
