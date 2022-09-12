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

namespace it.Areas.Admin.Controllers
{
    public class ProcessGroupController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private string _type = "ProcessGroup";
        public ProcessGroupController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        {
            ViewData["controller"] = _type;
            UserManager = UserMgr;
        }

        // GET: Admin/ProcessGroup
        public IActionResult Index()
        {
            return View();
        }


        // GET: Admin/ProcessGroup/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ProcessGroup/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(ProcessGroupModel ProcessGroupModel)
        {
            if (ModelState.IsValid)
            {
                ProcessGroupModel.created_at = DateTime.Now;
                _context.Add(ProcessGroupModel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return Ok(ModelState);
        }

        // GET: Admin/ProcessGroup/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.ProcessGroupModel == null)
            {
                return NotFound();
            }

            var ProcessGroupModel = _context.ProcessGroupModel
                .Where(d => d.id == id).FirstOrDefault();
            if (ProcessGroupModel == null)
            {
                return NotFound();
            }

            return View(ProcessGroupModel);
        }

        // POST: Admin/ProcessGroup/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProcessGroupModel ProcessGroupModel)
        {

            if (id != ProcessGroupModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var ProcessGroupModel_old = await _context.ProcessGroupModel.FindAsync(id);
                    ProcessGroupModel_old.updated_at = DateTime.Now;

                    foreach (string key in HttpContext.Request.Form.Keys)
                    {
                        var prop = ProcessGroupModel_old.GetType().GetProperty(key);

                        dynamic val = Request.Form[key].FirstOrDefault();

                        if (prop != null)
                        {
                            prop.SetValue(ProcessGroupModel_old, val);
                        }
                    }
                    _context.Update(ProcessGroupModel_old);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(ProcessGroupModel);
        }


        // GET: Admin/ProcessGroup/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.ProcessGroupModel == null)
            {
                return Problem("Entity set 'ItContext.ProcessGroupModel'  is null.");
            }
            var ProcessGroupModel = await _context.ProcessGroupModel.FindAsync(id);
            if (ProcessGroupModel != null)
            {
                ProcessGroupModel.deleted_at = DateTime.Now;
                _context.ProcessGroupModel.Update(ProcessGroupModel);
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
            var customerData = (from tempcustomer in _context.ProcessGroupModel.Where(u => u.deleted_at == null) select tempcustomer);
            int recordsTotal = customerData.Count();
            customerData = customerData.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(searchValue))
            {
                customerData = customerData.Where(m => m.name.Contains(searchValue));
            }
            int recordsFiltered = customerData.Count();
            var datapost = customerData.Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();
            foreach (var record in datapost)
            {
                var data1 = new
                {
                    action = "<div class='btn-group'><a href='/admin/" + _type + "/delete/" + record.id + "' class='btn btn-danger btn-sm' title='Xóa?' data-type='confirm'>'"
                        + "<i class='fas fa-trash-alt'>"
                        + "</i>"
                        + "</a></div>",
                    id = "<a href='/admin/" + _type + "/edit/" + record.id + "'><i class='fas fa-pencil-alt mr-2'></i> " + record.id + "</a>",
                    name = record.name
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }
    }
}
