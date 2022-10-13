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
    public class DepartmentController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private string _type = "Department";
        public DepartmentController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        {
            ViewData["controller"] = _type;
            UserManager = UserMgr;
        }

        // GET: Admin/Department
        public IActionResult Index()
        {
            return View();
        }


        // GET: Admin/Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Department/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentModel DepartmentModel)
        {
            if (ModelState.IsValid)
            {
                DepartmentModel.created_at = DateTime.Now;
                _context.Add(DepartmentModel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return Ok(ModelState);
        }

        // GET: Admin/Department/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.DepartmentModel == null)
            {
                return NotFound();
            }

            var DepartmentModel = _context.DepartmentModel
                .Where(d => d.id == id).FirstOrDefault();
            if (DepartmentModel == null)
            {
                return NotFound();
            }

            return View(DepartmentModel);
        }

        // POST: Admin/Department/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, DepartmentModel DepartmentModel)
        {

            if (id != DepartmentModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var DepartmentModel_old = await _context.DepartmentModel.FindAsync(id);
                    DepartmentModel_old.updated_at = DateTime.Now;

                    foreach (string key in HttpContext.Request.Form.Keys)
                    {
                        var prop = DepartmentModel_old.GetType().GetProperty(key);

                        dynamic val = Request.Form[key].FirstOrDefault();

                        if (prop != null)
                        {
                            prop.SetValue(DepartmentModel_old, val);
                        }
                    }
                    _context.Update(DepartmentModel_old);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(DepartmentModel);
        }


        // GET: Admin/Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.DepartmentModel == null)
            {
                return Problem("Entity set 'ItContext.DepartmentModel'  is null.");
            }
            var DepartmentModel = await _context.DepartmentModel.FindAsync(id);
            if (DepartmentModel != null)
            {
                DepartmentModel.deleted_at = DateTime.Now;
                _context.DepartmentModel.Update(DepartmentModel);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> saveorder(List<DepartmentOrder> data)
        {
            var index = 0;
            foreach (var item in data)
            {
                var DepartmentModel = _context.DepartmentModel.Find(item.id);
                DepartmentModel.parent = item.parent_id != null ? item.parent_id : 0;
                DepartmentModel.count_child = item.count_child;
                DepartmentModel.stt = index++;
                _context.Update(DepartmentModel);
            }
            _context.SaveChanges();
            return Json(new { success = 1 });
        }


    }
    public class DepartmentOrder
    {
        public int id { get; set; }
        public int count_child { get; set; }

        public int parent_id { get; set; }
    }
}
