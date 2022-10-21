
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using it.Areas.Admin.Models;
using it.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace it.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApiController : Controller
    {
        private UserManager<UserModel> UserManager;


        //public ApiController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        // {
        //    UserManager = UserMgr;
        //}
        private ItContext _context;
        public ApiController(ItContext context, UserManager<UserModel> UserMgr)
        {
            _context = context;
            UserManager = UserMgr;
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
                    label = user.FullName + "(" + user.Email + ")"
                };
                list.Add(DepartmentResponse);
            }
            //var jsonData = new { data = ProcessModel };
            return Json(list);
        }

        public async Task<JsonResult> Department()
        {
            var All = GetChild(0);
            //var jsonData = new { data = ProcessModel };
            return Json(All);
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
    public class SelectResponse
    {
        public string id { get; set; }
        public string label { get; set; }

        public virtual List<SelectResponse> children { get; set; }
    }
}
