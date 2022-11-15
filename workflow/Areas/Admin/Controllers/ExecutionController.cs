using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using it.Areas.Admin.Models;
using it.Data;
using System.Collections;
using Microsoft.AspNetCore.Identity;
using workflow.Services;
using Newtonsoft.Json;

namespace it.Areas.Admin.Controllers
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
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var customerData = (from tempcustomer in _context.ExecutionModel.Where(u => u.deleted_at == null) select tempcustomer);
            int recordsTotal = customerData.Count();
            customerData = customerData.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(searchValue))
            {
                customerData = customerData.Where(m => m.title.Contains(searchValue));
            }
            int recordsFiltered = customerData.Count();
            //customerData = customerData.Include(m => m.group);
            var datapost = customerData.Include(d => d.process_version).Include(d => d.user).Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();

            foreach (var record in datapost)
            {
                //var group = record.group;
                var html_process = record.process_version.process.name;
                var html_status = $"<div class='status status_{record.status_id}'>{record.status}</div>";
                var html_action = "<div class='btn-group'>";
                var html_user = record.user.FullName;
                var html_date = ((DateTime)record.created_at).ToString("dd/MM/yyyy");
                html_action += "<a href='/admin/" + _type + "/delete/" + record.id + "' class='' title='Xóa?' data-type='confirm'>"
                        + "<i class='fas text-danger font-16 fa-trash-alt'>"
                        + "</i>"
                        + "</a>";
                html_action += "</div>";
                var data1 = new
                {
                    action = html_action,
                    id = "<a href='/admin/" + _type + "/details/" + record.process_version_id + "?execution_id=" + record.id + "'><i class='fas fa-pencil-alt mr-2'></i> " + record.id + "</a>",
                    title = record.title,
                    status = html_status,
                    user_create = html_user,
                    date_create = html_date,
                    process = html_process
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ExecutionModel == null)
            {
                return Problem("Entity set 'ItContext.ExecutionModel'  is null.");
            }
            var ExecutionModel = await _context.ExecutionModel.FindAsync(id);
            if (ExecutionModel != null)
            {
                ExecutionModel.deleted_at = DateTime.Now;
                _context.ExecutionModel.Update(ExecutionModel);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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
    }
}
