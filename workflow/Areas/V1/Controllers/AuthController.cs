
using Microsoft.AspNetCore.Mvc;
using Vue.Models;
using Vue.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace workflow.Areas.V1.Controllers
{

	[Area("V1")]
	[Authorize]
	public class AuthController : Controller
	{
		private readonly ItContext _context;
		private readonly UserManager<UserModel> UserManager;

		private readonly IConfiguration _configuration;
		private readonly SignInManager<UserModel> _signInManager;

		public AuthController(ItContext context, UserManager<UserModel> UserMgr, SignInManager<UserModel> signInManager, IConfiguration configuration)
		{
			UserManager = UserMgr;
			_configuration = configuration;
			_signInManager = signInManager;
			_context = context;
		}
		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[DataType(DataType.Password)]
			[Display(Name = "Current password")]
			public string? oldpassword { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "New password")]
			public string? newpassword { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[DataType(DataType.Password)]
			[Display(Name = "Confirm new password")]
			[Compare("newpassword", ErrorMessage = "The new password and confirmation password do not match.")]
			public string? confirm { get; set; }
		}
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			/// Audittrail
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			var user = await UserManager.GetUserAsync(currentUser); // Get user
			var audit = new AuditTrailsModel();
			audit.UserId = user.Id;
			audit.Type = AuditType.Logout.ToString();
			audit.DateTime = DateTime.Now;
			audit.description = $"Tài khoản {user.FullName} đã đăng xuất";
			_context.Add(audit);
			await _context.SaveChangesAsync();

			await _signInManager.SignOutAsync();
			////Remove Cookie
			Response.Cookies.Delete(_configuration["JWT:NameCookieAuth"], new CookieOptions()
			{
				Domain = _configuration["JWT:Domain"]
			});
			return Redirect("/");
		}
		[HttpPost]
		public async Task<JsonResult> ChangePassword(InputModel input)
		{
			System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			string id = UserManager.GetUserId(currentUser); // Get user id:

			var user = await UserManager.GetUserAsync(currentUser);
			if (user == null)
			{
				return Json(new { success = false, message = $"Unable to load user with ID '{UserManager.GetUserId(User)}'." });
			}

			var changePasswordResult = await UserManager.ChangePasswordAsync(user, input.oldpassword, input.newpassword);
			if (!changePasswordResult.Succeeded)
			{
				var ErrorMessage = "";
				foreach (var error in changePasswordResult.Errors)
				{
					ErrorMessage += error.Description + "<br>";
				}
				return Json(new { success = false, message = ErrorMessage });
			}

			user.AccessFailedCount = 0;
			user.LockoutEnd = null;
			await UserManager.UpdateAsync(user);

			/// Audittrail
			var audit = new AuditTrailsModel();
			audit.UserId = user.Id;
			audit.Type = AuditType.ChangePassword.ToString();
			audit.DateTime = DateTime.Now;
			audit.description = $"Tài khoản {user.FullName} đã đổi mật khẩu";
			_context.Add(audit);
			await _context.SaveChangesAsync();

			var StatusMessage = "Mật khẩu đã được thay đổi";

			return Json(new { success = true, message = StatusMessage });
		}

		public async Task<JsonResult> TokenInfo(string token)
		{
			var find = _context.TokenModel.Where(d => d.deleted_at == null && d.token == token && d.vaild_to > DateTime.Now).FirstOrDefault();
			if (find != null)
			{
				var user = _context.UserModel.Where(d => d.deleted_at == null && d.Email.ToLower() == find.email.ToLower()).FirstOrDefault();
				if (user != null)
				{
					var roles = await UserManager.GetRolesAsync(user);
					return Json(new
					{
						success = true,
						roles = roles,
						email = user.Email,
						FullName = user.FullName,
						image_url = user.image_url,
						token = token,
						vaild_to = find.vaild_to.Value.ToString("yyyy-MM-dd HH:mm:ss")
					});
				}
			}
			return Json(new { success = false });


		}
		public JsonResult Index()
		{
			return Json("auth");

		}
	}
}
