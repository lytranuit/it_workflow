
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using Vue.Data;
using Vue.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using CertificateManager;
using System.Security.Cryptography.X509Certificates;
using CertificateManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Data;
using NuGet.Packaging;
using Spire.Xls;
using workflow.Areas.V1.Models;
using workflow.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace workflow.Areas.V1.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class UserController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private RoleManager<IdentityRole> RoleManager;
        private readonly IConfiguration _configuration;
        private EsignContext _esignContext;
        public UserController(ItContext context, EsignContext EsignContext, UserManager<UserModel> UserMgr, RoleManager<IdentityRole> RoleMgr, IConfiguration configuration) : base(context)
        {
            _esignContext = EsignContext;
            _configuration = configuration;
            UserManager = UserMgr;
            RoleManager = RoleMgr;
        }

        // POST: UserController/Create
        [HttpPost]
        public async Task<JsonResult> Create(UserModel User, string password, List<string> roles, List<int> departments)

        {

            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var user_current = await UserManager.GetUserAsync(currentUser); // Get user id:
                                                                            //string password = "!PMP_it123456";
            UserModel user = new UserModel
            {
                Email = User.Email,
                UserName = User.Email,
                EmailConfirmed = true,
                FullName = User.FullName,
                image_url = User.image_url,
                image_sign = User.image_sign,
            };
            IdentityResult result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //return Ok(result);
                foreach (string role_id in roles)
                {
                    _context.Add(new UserRoleModel()
                    {
                        UserId = user.Id,
                        RoleId = role_id,
                    });
                }
                foreach (int department in departments)
                {
                    var UserDepartmentModel = new UserDepartmentModel()
                    {
                        user_id = user.Id,
                        department_id = department,
                    };
                    _context.Add(UserDepartmentModel);
                }
                await _context.SaveChangesAsync();
                /// Audittrail
                var audit = new AuditTrailsModel();
                audit.UserId = user_current.Id;
                audit.Type = AuditType.Create.ToString();
                audit.DateTime = DateTime.Now;
                audit.description = $"Tài khoản {user_current.FullName} đã tạo tài khoản mới.";
                audit.TableName = "UserModel";
                audit.PrimaryKey = user.Id;
                audit.NewValues = JsonConvert.SerializeObject(user);

                _context.Add(audit);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tạo thành công" });
            }
            else
            {   /// Audittrail
				var audit = new AuditTrailsModel();
                audit.UserId = user_current.Id;
                audit.Type = AuditType.None.ToString();
                audit.DateTime = DateTime.Now;
                audit.description = result.ToString();
                audit.TableName = "UserModel";

                _context.Add(audit);
                await _context.SaveChangesAsync();
                return Json(new { success = false, message = result.ToString() });
            }

        }


        // POST: UserController/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit(UserModel User, List<string> roles, List<int> departments)
        {
            UserModel User_old = await UserManager.FindByIdAsync(User.Id);
            if (User_old == null)
            {
                return Json(new { success = false, message = "Fail" });
            }
            var OldValues = JsonConvert.SerializeObject(User_old);
            User_old.Email = User.Email;
            User_old.UserName = User.Email;
            User_old.FullName = User.FullName;
            User_old.image_url = User.image_url;
            User_old.image_sign = User.image_sign;
            User_old.PhoneNumber = User.PhoneNumber;
            User_old.reportId = User.reportId;
            User_old.department_text = User.department_text;
            User_old.msnv = User.msnv;
            User_old.ngaynghi = User.ngaynghi;

            _context.Update(User_old);
            _context.SaveChanges();
            var role_avaliable = _configuration.GetSection("Roles").Get<string[]>().ToList();
            var roles_old = RoleManager.Roles.Where(d => role_avaliable.Contains(d.Name)).Select(a => a.Id).ToList();

            var UserRoleModel_old = _context.UserRoleModel.Where(d => d.UserId == User.Id && roles_old.Contains(d.RoleId)).Select(d => d.RoleId).ToList();
            IEnumerable<string> list_delete = UserRoleModel_old.Except(roles);
            IEnumerable<string> list_add = roles.Except(UserRoleModel_old);
            if (list_add != null)
            {
                foreach (string key in list_add)
                {

                    _context.Add(new UserRoleModel()
                    {
                        UserId = User.Id,
                        RoleId = key,
                    });
                }
                await _context.SaveChangesAsync();
            }
            if (list_delete != null)
            {
                foreach (string key in list_delete)
                {
                    UserRoleModel UserRoleModel = _context.UserRoleModel.Where(d => d.UserId == User.Id && d.RoleId == key).First();
                    _context.Remove(UserRoleModel);
                }
                await _context.SaveChangesAsync();
            }
            ///
            var departments_old = _context.UserDepartmentModel.Where(d => d.user_id == User_old.Id).ToList();
            _context.RemoveRange(departments_old);
            foreach (int department in departments)
            {
                var UserDepartmentModel = new UserDepartmentModel()
                {
                    user_id = User_old.Id,
                    department_id = department,
                };
                _context.Add(UserDepartmentModel);
            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Thành công" });

        }

        // POST: UserController/Edit/5
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            UserModel User = await UserManager.FindByIdAsync(id);
            if (User != null)
            {
                //var OldValues = JsonConvert.SerializeObject(User);
                User.deleted_at = DateTime.Now;
                IdentityResult result = await UserManager.UpdateAsync(User);
                if (result.Succeeded)
                {
                    /// Audittrail
                    System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                    var user = await UserManager.GetUserAsync(currentUser); // Get user
                    var audit = new AuditTrailsModel();
                    audit.UserId = user.Id;
                    audit.Type = AuditType.Delete.ToString();
                    audit.DateTime = DateTime.Now;
                    audit.description = $"Tài khoản {user.FullName} đã xóa tài khoản {User.FullName}.";
                    audit.TableName = "UserModel";
                    audit.PrimaryKey = User.Id;
                    _context.Add(audit);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }

        public async Task<JsonResult> Get(string id)
        {
            UserModel User = await _context.UserModel.Where(d => d.Id == id).Include(d => d.departments).FirstOrDefaultAsync();
            var role_avaliable = _configuration.GetSection("Roles").Get<string[]>().ToList();
            var roles_old = RoleManager.Roles.Where(d => role_avaliable.Contains(d.Name)).Select(a => a.Id).ToList();
            var roles = _context.UserRoleModel.Where(d => d.UserId == id && roles_old.Contains(d.RoleId)).Select(d => d.RoleId).ToList();
            return Json(new { success = true, id = User.Id, roles = roles, departments = User.departments.Select(d => d.department_id.ToString()).ToList(), email = User.Email, FullName = User.FullName, image_url = User.image_url, image_sign = User.image_sign, PhoneNumber = User.PhoneNumber, department_text = User.department_text, reportId = User.reportId, ngaynghi = User.ngaynghi, msnv = User.msnv });
        }

        [HttpPost]
        public async Task<JsonResult> Table()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var id = Request.Form["filters[id]"].FirstOrDefault();
            var email = Request.Form["filters[email]"].FirstOrDefault();
            var fullName = Request.Form["filters[fullName]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var customerData = (from tempcustomer in UserManager.Users select tempcustomer);
            customerData = customerData.Where(m => m.deleted_at == null);
            int recordsTotal = customerData.Count();
            if (id != null && id != "")
            {
                customerData = customerData.Where(d => d.Id == id);
            }
            if (email != null && email != "")
            {
                customerData = customerData.Where(d => d.Email.Contains(email));
            }
            if (fullName != null && fullName != "")
            {
                customerData = customerData.Where(d => d.FullName.Contains(fullName));
            }
            int recordsFiltered = customerData.Count();
            var datapost = customerData.Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();
            foreach (var record in datapost)
            {
                var image = "<img src='" + record.image_url + "' class='thumb-sm rounded-circle'>";
                var image_sign = "<img src='" + record.image_sign + "' class='' width='100'>";
                var data1 = new
                {
                    Id = record.Id,
                    email = record.Email,
                    fullName = record.FullName,
                    image = image,
                    image_sign = image_sign
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string id { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string? NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(InputModel Input)
        {
            //Get User By Id
            var User = await UserManager.FindByIdAsync(Input.id);

            //Generate Token
            var token = await UserManager.GeneratePasswordResetTokenAsync(User);

            //Set new Password
            var changePasswordResult = await UserManager.ResetPasswordAsync(User, token, Input.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                var ErrorMessage = "";
                foreach (var error in changePasswordResult.Errors)
                {
                    ErrorMessage += error.Description + "<br>";
                }
                return Json(new { success = false, message = ErrorMessage });
            }
            User.AccessFailedCount = 0;
            User.LockoutEnd = null;
            await UserManager.UpdateAsync(User);

            /// Audittrail
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var user = await UserManager.GetUserAsync(currentUser); // Get user
            var audit = new AuditTrailsModel();
            audit.UserId = user.Id;
            audit.Type = AuditType.Update.ToString();
            audit.DateTime = DateTime.Now;
            audit.description = $"Tài khoản {user.FullName} đã thay đổi mật khẩu tài khoản {User.FullName}.";
            _context.Add(audit);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mật khẩu đã được thay đổi" });

        }



        public async Task<JsonResult> Roles()
        {
            var role_avaliable = _configuration.GetSection("Roles").Get<string[]>().ToList();
            var Model = RoleManager.Roles.Where(d => role_avaliable.Contains(d.Name)).Select(a => new
            {
                id = a.Id,
                label = a.Name
            }).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(Model);
        }
        public async Task<JsonResult> Departments()
        {
            var All = GetChild(0);
            //var jsonData = new { data = ProcessModel };
            return Json(All);
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
        [HttpPost]
        public async Task<JsonResult> sync()
        {
            var user_esign = _esignContext.UserEsignModel.ToList();

            foreach (var user in user_esign)
            {
                var find = _context.UserModel.Where(d => d.Email.ToLower() == user.Email.ToLower()).FirstOrDefault();
                if (find != null)
                {
                    find.FullName = user.FullName;
                    find.image_sign = user.image_sign;
                    find.image_url = user.image_url;
                    find.signature = $"/private/pfx/{user.Id}.pfx";
					find.deleted_at = user.deleted_at;
					find.LockoutEnd = user.LockoutEnd;
					_context.Update(find);
                    _context.SaveChanges();
                }
                else
                {
                    UserModel newuser = new UserModel
                    {
                        Email = user.Email,
                        UserName = user.Email,
                        EmailConfirmed = true,
                        FullName = user.FullName,
                        image_url = user.image_url,
                        image_sign = user.image_sign,
                        signature = $"/private/pfx/{user.Id}.pfx",
						deleted_at = user.deleted_at,
						LockoutEnd = user.LockoutEnd,
					};
                    IdentityResult result = await UserManager.CreateAsync(newuser, "!PMP_it123456");

                }
            }
            return Json(new { success = true });
        }

        public async Task<JsonResult> active()
        {
            var users = _context.UserModel.Where(d => d.deleted_at == null && d.image_sign != "/private/images/tick.png").ToList();
            var role_user = await RoleManager.FindByNameAsync("User");
            if (role_user == null)
                return Json(new { success = false });
            foreach (var item in users)
            {
                var in_user = await UserManager.IsInRoleAsync(item, "User");
                if (in_user)
                    continue;
                _context.Add(new UserRoleModel()
                {
                    UserId = item.Id,
                    RoleId = role_user.Id,
                });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<JsonResult> excel()
        {
            try
            {

                ////Lấy 
                var list = new List<UserModel>();
                var role_avaliable = _configuration.GetSection("Roles").Get<string[]>().ToList();
                foreach (var role in role_avaliable)
                {
                    var list_user = await UserManager.GetUsersInRoleAsync(role);
                    list.AddRange(list_user);
                }

                var data = new ArrayList();

                var viewPath = "wwwroot/report/excel/SOP1600XX.02 - Requisition list of user privileges – defining software access levels1.xlsx";
                var documentPath = "/tmp/" + DateTime.Now.ToFileTimeUtc() + ".xlsx";
                string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;

                Workbook workbook = new Workbook();
                workbook.LoadFromFile(viewPath);
                list = list.Where(d => d.deleted_at == null).Distinct().ToList();
                if (list.Count > 0)
                {
                    Worksheet sheet = workbook.Worksheets[0];
                    int stt = 0;
                    var start_r = 8;

                    DataTable dt = new DataTable();
                    dt.Columns.Add("stt", typeof(int));
                    dt.Columns.Add("ten", typeof(string));
                    dt.Columns.Add("email", typeof(string));
                    dt.Columns.Add("level", typeof(string));
                    dt.Columns.Add("kho", typeof(string));
                    dt.Columns.Add("note", typeof(string));
                    sheet.InsertRow(9, list.Count());
                    foreach (var item in list)
                    {
                        var groups = await UserManager.GetRolesAsync(item);
                        var level = groups.ToList();
                        DataRow dr1 = dt.NewRow();
                        dr1["stt"] = (++stt);
                        dr1["ten"] = item.FullName;
                        dr1["email"] = item.Email;
                        dr1["level"] = string.Join(",", level);
                        dr1["note"] = "Đã tồn tại";
                        dt.Rows.Add(dr1);
                        start_r++;

                        CellRange originDataRang = sheet.Range["A8:G8"];
                        CellRange targetDataRang = sheet.Range["A" + start_r + ":G" + start_r];
                        sheet.Copy(originDataRang, targetDataRang, true);
                    }
                    sheet.InsertDataTable(dt, false, 8, 1);
                    sheet.DeleteRow(start_r);
                }

                workbook.SaveToFile("./wwwroot" + documentPath, ExcelVersion.Version2013);
                //var congthuc_ct = _QLSXcontext.Congthuc_CTModel.Where()
                var jsonData = new { success = true, link = documentPath };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public class SelectResponse
        {
            public string id { get; set; }
            public string label { get; set; }

            public string name { get; set; }
            public virtual List<SelectResponse> children { get; set; }
        }
    }
}
