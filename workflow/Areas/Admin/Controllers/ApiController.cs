
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using it.Areas.Admin.Models;
using it.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

    }
}
