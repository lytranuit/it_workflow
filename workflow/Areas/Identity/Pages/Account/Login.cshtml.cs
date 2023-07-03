﻿
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vue.Models;
using Vue.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;

namespace it.Areas.Identity.Pages.Account
{
	public class LoginModel : PageModel
	{
		private readonly SignInManager<UserModel> _signInManager;
		private readonly ILogger<LoginModel> _logger;
		private UserManager<UserModel> UserManager;

		private readonly IConfiguration _configuration;
		protected readonly ItContext _context;

		public LoginModel(SignInManager<UserModel> signInManager, ILogger<LoginModel> logger, ItContext context, UserManager<UserModel> UserMgr, IConfiguration configuration)
		{
			_signInManager = signInManager;
			_logger = logger;
			_context = context;
			UserManager = UserMgr;
			_configuration = configuration;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string ErrorMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Display(Name = "Ghi nhớ?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			returnUrl ??= Url.Content("~/");

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			var pass = Input.Password;
			var user = _context.UserModel.Where(x => x.Email.ToLower() == Input.Email.ToLower() && x.deleted_at == null).FirstOrDefault();

			if (user == null)
			{
				/// Audittrail
				var audit = new AuditTrailsModel();
				audit.Type = AuditType.LoginFailed.ToString();
				audit.DateTime = DateTime.Now;
				audit.description = $"Tài khoản {Input.Email} đăng nhập thất bại.";
				_context.Add(audit);
				await _context.SaveChangesAsync();
				ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng.");
				return Page();
			}

			LoginResponse responseJson = new LoginResponse() { authed = false };
			try
			{
				var client = new HttpClient();

				client.DefaultRequestHeaders.Accept
					.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
				var values = new Dictionary<string, string>
				{
					{ "email", Input.Email },
					{ "password", Input.Password }
				};
				var content = new FormUrlEncodedContent(values);
				var url = _configuration["CheckAuth"];
				var response = await client.PostAsync(url, content);
				if (response.IsSuccessStatusCode)
				{
					responseJson = await response.Content.ReadFromJsonAsync<LoginResponse>();
				}
			}
			catch
			{

			}
			if (responseJson.authed == true)
			{
				user.last_login = DateTime.Now;
				user.AccessFailedCount = 0;
				_context.Update(user);
				await _signInManager.SignInAsync(user, true);
				var authClaims = new List<Claim>
						{
							new Claim("Email", Input.Email),
							new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						};
				var token = GetToken(authClaims);
				var token_string = new JwtSecurityTokenHandler().WriteToken(token);
				var TokenModel = new TokenModel()
				{
					token = token_string,
					email = Input.Email,
					created_at = DateTime.Now,
					vaild_to = token.ValidTo
				};
				_context.Add(TokenModel);
				/// Audittrail
				var audit = new AuditTrailsModel();
				audit.UserId = user.Id;
				audit.Type = AuditType.Login.ToString();
				audit.DateTime = DateTime.Now;
				audit.description = $"Tài khoản {Input.Email} đăng nhập thành công";
				_context.Add(audit);
				await _context.SaveChangesAsync();
				////
				Response.Cookies.Append(
					_configuration["JWT:NameCookieAuth"],
					token_string,
					new CookieOptions()
					{
						Domain = _configuration["JWT:Domain"],
						Expires = DateTime.Now.AddHours(Int64.Parse(_configuration["JWT:Expire"]))
					}
				);
				return LocalRedirect(returnUrl);
			}
			var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				var authClaims = new List<Claim>
						{
							new Claim("Email", Input.Email),
							new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						};
				var token = GetToken(authClaims);
				var token_string = new JwtSecurityTokenHandler().WriteToken(token);
				var TokenModel = new TokenModel()
				{
					token = token_string,
					email = Input.Email,
					created_at = DateTime.Now,
					vaild_to = token.ValidTo
				};
				_context.Add(TokenModel);
				/// Audittrail
				user.last_login = DateTime.Now;
				user.AccessFailedCount = 0;
				_context.Update(user);

				var audit = new AuditTrailsModel();
				audit.UserId = user.Id;
				audit.Type = AuditType.Login.ToString();
				audit.DateTime = DateTime.Now;
				audit.description = $"Tài khoản {Input.Email} đăng nhập thành công";
				_context.Add(audit);
				await _context.SaveChangesAsync();
				////
				Response.Cookies.Append(
					_configuration["JWT:NameCookieAuth"],
					token_string,
					new CookieOptions()
					{
						Domain = _configuration["JWT:Domain"],
						Expires = DateTime.Now.AddHours(Int64.Parse(_configuration["JWT:Expire"]))
					}
				);
				return LocalRedirect(returnUrl);
			}
			if (result.IsLockedOut)
			{
				/// Audittrail
				var audit = new AuditTrailsModel();
				audit.Type = AuditType.Lockout.ToString();
				audit.DateTime = DateTime.Now;
				audit.description = $"Tài khoản {Input.Email} đã bị khóa trong 5 phút.";
				_context.Add(audit);
				await _context.SaveChangesAsync();

				ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa trong 5 phút.");
				return Page();
			}
			else
			{
				/// Audittrail
				var audit = new AuditTrailsModel();
				audit.Type = AuditType.LoginFailed.ToString();
				audit.DateTime = DateTime.Now;
				audit.description = $"Tài khoản {Input.Email} đăng nhập thất bại.";
				_context.Add(audit);
				await _context.SaveChangesAsync();
				ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng.");
				return Page();
			}



		}
		private JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(Int64.Parse(_configuration["JWT:Expire"])),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
				);

			return token;
		}
	}

	public class LoginResponse
	{
		[Key]
		public bool authed { get; set; }

		public string? error { get; set; }
		public string? parameter { get; set; }

		public string? session { get; set; }
		public string? user { get; set; }
		public string? token { get; set; }
	}
}
