
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using it.Areas.Admin.Models;
using it.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace it.Areas.Admin.Controllers
{
    public class ApiController : BaseController
    {
        private UserManager<UserModel> UserManager;


        public ApiController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        {
            UserManager = UserMgr;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }


        public async Task<JsonResult> Employee()
        {
            var UserModel = _context.UserModel.Where(x => x.deleted_at == null).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(UserModel);
        }

        public async Task<JsonResult> Department()
        {
            var All = GetChild(0);
            //var jsonData = new { data = ProcessModel };
            return Json(All);
        }
        private List<DepartmentResponse> GetChild(int parent)
        {
            var DepartmentModel = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == parent).OrderBy(d => d.stt).ToList();
            var list = new List<DepartmentResponse>();
            if (DepartmentModel.Count() > 0)
            {
                foreach (var department in DepartmentModel)
                {
                    var DepartmentResponse = new DepartmentResponse
                    {

                        id = department.id,
                        label = department.name
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
    }
    public class DepartmentResponse
    {
        public int id { get; set; }
        public string label { get; set; }

        public virtual List<DepartmentResponse> children { get; set; }
    }
}
