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
    public class ProcessController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private string _type = "Process";
        public ProcessController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        {
            ViewData["controller"] = _type;
            UserManager = UserMgr;
        }

        // GET: Admin/Process
        public IActionResult Index()
        {
            return View();
        }


        // GET: Admin/Process/Create
        public IActionResult Create()
        {

            return View();
        }

        // GET: Admin/Process/Edit/5
        public IActionResult Edit(string? id)
        {
            if (id == null || _context.ProcessModel == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

            return View();
        }


        // GET: Admin/Process/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.ProcessModel == null)
            {
                return Problem("Entity set 'ItContext.ProcessModel'  is null.");
            }
            var ProcessModel = _context.ProcessModel.Where(d => d.id == id).FirstOrDefault();
            if (ProcessModel != null)
            {
                ProcessModel.deleted_at = DateTime.Now;
                _context.ProcessModel.Update(ProcessModel);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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
            var customerData = (from tempcustomer in _context.ProcessModel.Where(u => u.deleted_at == null) select tempcustomer);
            int recordsTotal = customerData.Count();
            customerData = customerData.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(searchValue))
            {
                customerData = customerData.Where(m => m.name.Contains(searchValue));
            }
            int recordsFiltered = customerData.Count();
            customerData = customerData.Include(m => m.group);
            var datapost = customerData.Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();

            foreach (var record in datapost)
            {
                var group = record.group;
                var html_group = "<button class='btn btn-sm text-white' style='background:" + group.color + "'>" + group.name + "</button>";
                var html_status = "";
                var html_action = "<div class='btn-group'>";
                if (record.status_id == (int)ProcessStatus.Draft)
                {
                    html_status = "<button class='btn btn-sm text-white btn-info'>" + ProcessStatus.Draft + "</button>";
                    html_action += "<a href='/admin/api/release/" + record.id + "' class='btn btn-success btn-sm' title='Phát hành?' data-type='confirm'>"
                        + "<i class='fas fa-arrow-up'></i>"
                        + "</i>"
                        + "</a>";
                }
                else if (record.status_id == (int)ProcessStatus.Release)
                {
                    html_status = "<button class='btn btn-sm text-white btn-success'>" + ProcessStatus.Release + "</button>";
                }

                html_action += "<a href='/admin/" + _type + "/delete/" + record.id + "' class='btn btn-danger btn-sm' title='Xóa?' data-type='confirm'>'"
                        + "<i class='fas fa-trash-alt'>"
                        + "</i>"
                        + "</a>";
                html_action += "</div>";
                var data1 = new
                {
                    action = html_action,
                    id = "<a href='/admin/" + _type + "/edit/" + record.id + "'><i class='fas fa-pencil-alt mr-2'></i> " + record.id + "</a>",
                    name = record.name,
                    status = html_status,
                    group = html_group
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
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
