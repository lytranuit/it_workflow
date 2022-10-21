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
        public IActionResult Create()
        {

            return View();
        }

        // POST: Admin/Execution/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<JsonResult> Create(ExecutionModel ExecutionModel)
        {
            //return RedirectToAction(nameof(Index));
            return Json(new { success = 1 });

            //return Json(ModelState);
        }

        // GET: Admin/Execution/Edit/5
        public IActionResult Details(string? id)
        {
            if (id == null || _context.ExecutionModel == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

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
                customerData = customerData.Where(m => m.name.Contains(searchValue));
            }
            int recordsFiltered = customerData.Count();
            //customerData = customerData.Include(m => m.group);
            var datapost = customerData.Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();

            foreach (var record in datapost)
            {
                //var group = record.group;
                var html_group = "";
                var html_status = "";
                var html_action = "<div class='btn-group'>";
                if (record.status_id == (int)ExecutionStatus.Draft)
                {
                    html_status = "<button class='btn btn-sm text-white btn-info'>" + ExecutionStatus.Draft + "</button>";
                    html_action += "<a href='/admin/" + _type + "/release/" + record.id + "' class='btn btn-success btn-sm' title='Phát hành?' data-type='confirm'>"
                        + "<i class='fas fa-arrow-up'></i>"
                        + "</i>"
                        + "</a>";
                }
                //else if (record.status_id == (int)ExecutionStatus.Release)
                //{
                //    html_status = "<button class='btn btn-sm text-white btn-success'>" + ExecutionStatus.Release + "</button>";
                //}

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
