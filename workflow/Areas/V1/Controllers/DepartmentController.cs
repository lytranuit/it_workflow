
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Vue.Data;
using Vue.Models;
using System.Collections;

namespace workflow.Areas.V1.Controllers
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
        [HttpPost]
        public async Task<JsonResult> Save(DepartmentModel DepartmentModel)
        {
            var jsonData = new { success = true, message = "" };
            try
            {
                if (DepartmentModel.id > 0)
                {
                    _context.Update(DepartmentModel);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(DepartmentModel);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                jsonData = new
                {
                    success = false,
                    message = ex.Message
                };
            }


            return Json(jsonData);
        }

        [HttpPost]
        public async Task<JsonResult> Remove(List<int> item)
        {
            var jsonData = new { success = true, message = "" };
            try
            {
                var list = _context.DepartmentModel.Where(d => item.Contains(d.id)).ToList();
                foreach (var dep in list)
                {
                    dep.deleted_at = DateTime.Now;
                    _context.Update(dep);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                jsonData = new { success = false, message = ex.Message };
            }


            return Json(jsonData);
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
        
        public async Task<JsonResult> Get()
        {
            var All = GetChild(0);
            //var jsonData = new { data = ProcessModel };
            return Json(All);
        }
        private List<DepartmentModel> GetChild(int parent)
        {
            var DepartmentModel = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == parent).OrderBy(d => d.stt).ToList();
            var list = new List<DepartmentModel>();
            if (DepartmentModel.Count() > 0)
            {
                foreach (var department in DepartmentModel)
                {
                    var count_child = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == department.id).Count();
                    if (count_child > 0)
                    {
                        var child = GetChild(department.id);
                        department.children = child;
                    }
                    list.Add(department);
                }
            }
            return list;
        }

    }
    public class DepartmentOrder
    {
        public int id { get; set; }
        public int count_child { get; set; }

        public int parent_id { get; set; }
    }
}
