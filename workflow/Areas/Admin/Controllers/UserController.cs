﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using it.Data;
using it.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace it.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private RoleManager<IdentityRole> RoleManager;
        public UserController(ItContext context, UserManager<UserModel> UserMgr, RoleManager<IdentityRole> RoleMgr) : base(context)
        {
            UserManager = UserMgr;
            RoleManager = RoleMgr;
        }
        // GET: UserController
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        // GET: UserController/Create
        public ActionResult Create()
        {

            ViewData["groups"] = RoleManager.Roles.Select(a => new SelectListItem()
            {
                Value = a.Name,
                Text = a.Name
            }).ToList();
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        public async Task<IActionResult> Create(UserModel User, List<string> groups, List<int> equipments)
        {

            string password = "!PMP_it123456";
            UserModel user = new UserModel
            {
                Email = User.Email,
                UserName = User.Email,
                EmailConfirmed = true,
                FullName = User.FullName,
                image_url = User.image_url,
            };
            IdentityResult result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //return Ok(result);
                foreach (string group in groups)
                {
                    await UserManager.AddToRoleAsync(user, group);
                }
              
                return RedirectToAction("Index");
            }
            else
                return Ok(result);

        }

        // GET: UserController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewData["groups"] = RoleManager.Roles.Select(a => new SelectListItem()
            {
                Value = a.Name,
                Text = a.Name
            }).ToList();
            UserModel User = _context.UserModel.Where(d => d.Id == id).FirstOrDefault();
            var RolesForThisUser = await UserManager.GetRolesAsync(User);
            ViewData["RolesForThisUser"] = RolesForThisUser;
            return View(User);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserModel User, List<string> groups, List<int> equipments)
        {
            //return Ok(User);
            if (id != User.Id)
            {
                return Ok(User);
            }
            UserModel User_old = await UserManager.FindByIdAsync(User.Id);
            User_old.Email = User.Email;
            User_old.UserName = User.Email;
            User_old.FullName = User.FullName;
            User_old.image_url = User.image_url;

            var RolesForThisUser = await UserManager.GetRolesAsync(User_old);
            await UserManager.RemoveFromRolesAsync(User_old, RolesForThisUser);
            foreach (string group in groups)
            {
                await UserManager.AddToRoleAsync(User_old, group);
            }
           
            IdentityResult result = await UserManager.UpdateAsync(User_old);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                return Ok(result);

            return View("Index", UserManager.Users);
        }

        // GET: UserController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            UserModel User = await UserManager.FindByIdAsync(id);
            if (User != null)
            {
                User.deleted_at = DateTime.Now;
                IdentityResult result = await UserManager.UpdateAsync(User);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    return Ok(result);
            }
            else
                ModelState.AddModelError("", "No User found");
            return View("Index", UserManager.Users);
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
            var customerData = (from tempcustomer in UserManager.Users select tempcustomer);
            customerData = customerData.Where(m => m.deleted_at == null);
            int recordsTotal = customerData.Count();
            if (!string.IsNullOrEmpty(searchValue))
            {
                customerData = customerData.Where(m => m.UserName.Contains(searchValue) || m.Email.Contains(searchValue));
            }
            int recordsFiltered = customerData.Count();
            var datapost = customerData.Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();
            foreach (var record in datapost)
            {
                var data1 = new
                {
                    action = "<div class='btn-group'><a href='/admin/User/delete/" + record.Id + "' class='btn btn-danger btn-sm' title='Xóa?' data-type='confirm'>'"
                        + "<i class='fas fa-trash-alt'>"
                        + "</i>"
                        + "</a></div>",
                    Id = "<a href='/admin/User/edit/" + record.Id + "'><i class='fas fa-pencil-alt mr-2'></i> " + record.Id + "</a>",
                    email = record.Email,
                    username = record.UserName
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }
    }
}
