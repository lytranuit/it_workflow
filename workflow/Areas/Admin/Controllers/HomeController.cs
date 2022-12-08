
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using it.Areas.Admin.Models;
using it.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Drawing.Printing;
using System.ComponentModel.DataAnnotations;

namespace it.Areas.Admin.Controllers
{
	public class HomeController : BaseController
	{
		private UserManager<UserModel> UserManager;


		public HomeController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
		{
			UserManager = UserMgr;
		}
		public async Task<IActionResult> Index()
		{
			//System.Security.Claims.ClaimsPrincipal currentUser = this.User;

			//var user = await UserManager.GetUserAsync(currentUser);
			//var is_admin = await UserManager.IsInRoleAsync(user, "Administrator");
			//if (!is_admin)
			//	return Redirect("/admin/");
			ViewBag.count = _context.ExecutionModel.Where(d => d.deleted_at == null).Count();
			ViewBag.wait_count = _context.ExecutionModel.Where(d => d.deleted_at == null && d.status_id == (int)ExecutionStatus.Executing).Count();
			ViewBag.done_count = _context.ExecutionModel.Where(d => d.deleted_at == null && d.status_id == (int)ExecutionStatus.Success).Count();
			ViewBag.cancle_count = _context.ExecutionModel.Where(d => d.deleted_at == null && d.status_id == (int)ExecutionStatus.Fail).Count();
			return View();
		}
		[HttpPost]
		public async Task<JsonResult> tableUser()
		{

			var type = Request.Form["type"].FirstOrDefault();
			var draw = Request.Form["draw"].FirstOrDefault();
			var start = Request.Form["start"].FirstOrDefault();
			var length = Request.Form["length"].FirstOrDefault();
			var searchValue = Request.Form["search[value]"].FirstOrDefault();
			int pageSize = length != null ? Convert.ToInt32(length) : 0;
			int skip = start != null ? Convert.ToInt32(start) : 0;
			//System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			//string user_id = UserManager.GetUserId(currentUser); // Get user id:
			//var user_current = await UserManager.GetUserAsync(currentUser);
			//var subsql = "";
			//var sql = $"select type_id,COUNT(1) as num from document where deleted_at is null and status_id IN(4,5,6,7) {subsql} GROUP BY type_id";

			//var data = _context.ChartType
			//	 .FromSqlRaw(sql);
			//var d = data.Include(d => d.type).OrderByDescending(d => d.num).ToList();

			var blocking_activity = _context.ActivityModel.Where(d => d.deleted_at == null && d.blocking == true).Select(d => d.block_id + d.execution_id).ToList();
			var custom_block = _context.CustomBlockModel.Where(d => blocking_activity.Contains(d.block_id + d.execution_id)).ToList();
			var list = new List<string>();
			foreach (var block in custom_block)
			{
				var data_setting = block.data_setting;
				if (data_setting.type_performer == 4)
				{
					list.AddRange(data_setting.listuser);
				}
			}
			var groupedCustomerList = list
				.GroupBy(u => u)
				.Select(grp => new
				{
					count = grp.Count(),
					user = _context.UserModel.Find(grp.Key)
				})
				.Where(d => d.user != null);

			int recordsTotal = groupedCustomerList.Count();
			int recordsFiltered = groupedCustomerList.Count();
			var records = groupedCustomerList.OrderByDescending(d => d.count).Skip(skip).Take(pageSize)
				.ToList();
			var data = new ArrayList();

			foreach (var record in records)
			{
				var data1 = new
				{
					fullName = $"<a href='/admin/execution/wait?user_id={record.user.Id}'>{record.user.FullName}</a>",
					email = record.user.Email,
					count = record.count,
				};
				data.Add(data1);
			}
			//var labels = new List<string>() { };
			//var datasets = new List<ChartDataSet>();
			//var backgroundColor = new List<string>();
			//var data1 = new List<int?>();
			//foreach (var type in d)
			//{
			//	labels.Add(type.type.name.ToString());
			//	backgroundColor.Add(type.type.color);
			//	data1.Add(type.num);
			//}
			//datasets.Add(new ChartDataSet
			//{
			//	backgroundColor = backgroundColor,
			//	data = data1
			//});

			var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
			return Json(jsonData);
			//return Json(new { labels = labels, datasets = datasets });
		}
		[HttpPost]
		public async Task<JsonResult> tableProcess()
		{

			var type = Request.Form["type"].FirstOrDefault();
			var draw = Request.Form["draw"].FirstOrDefault();
			var start = Request.Form["start"].FirstOrDefault();
			var length = Request.Form["length"].FirstOrDefault();
			var searchValue = Request.Form["search[value]"].FirstOrDefault();
			int pageSize = length != null ? Convert.ToInt32(length) : 0;
			int skip = start != null ? Convert.ToInt32(start) : 0;
			//System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			//string user_id = UserManager.GetUserId(currentUser); // Get user id:
			//var user_current = await UserManager.GetUserAsync(currentUser);
			//var subsql = "";
			//var sql = $"select type_id,COUNT(1) as num from document where deleted_at is null and status_id IN(4,5,6,7) {subsql} GROUP BY type_id";

			//var data = _context.ChartType
			//	 .FromSqlRaw(sql);
			//var d = data.Include(d => d.type).OrderByDescending(d => d.num).ToList();

			var list = _context.ExecutionModel.Where(d => d.deleted_at == null);
			var groupedCustomerList = list
				.GroupBy(u => u.process_version_id)
				.Select(grp => new
				{
					count = grp.Count(),
					process_version_id = grp.Key,
					process_version = _context.ProcessVersionModel.Where(d => d.id == grp.Key).FirstOrDefault(),
				}).ToList();



			var records = groupedCustomerList.OrderByDescending(d => d.count).Skip(skip).Take(pageSize)
				.ToList();

			int recordsTotal = groupedCustomerList.Count();
			int recordsFiltered = groupedCustomerList.Count();
			var data = new ArrayList();

			foreach (var record in records)
			{
				var process_version = record.process_version;
				var process = process_version.process;
				var data1 = new
				{
					name = $"<a href='#'>{process.name}</a>",
					version = process_version.version,
					count = record.count,
					excel = $"<a href='/admin/process/exportVersion?process_version_id={process_version.id}' class='export'><i class=\"fas fa-download\"></i></a>",
				};
				data.Add(data1);
			}
			//var labels = new List<string>() { };
			//var datasets = new List<ChartDataSet>();
			//var backgroundColor = new List<string>();
			//var data1 = new List<int?>();
			//foreach (var type in d)
			//{
			//	labels.Add(type.type.name.ToString());
			//	backgroundColor.Add(type.type.color);
			//	data1.Add(type.num);
			//}
			//datasets.Add(new ChartDataSet
			//{
			//	backgroundColor = backgroundColor,
			//	data = data1
			//});

			var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
			return Json(jsonData);
			//return Json(new { labels = labels, datasets = datasets });
		}
		public async Task<JsonResult> datachartDepartment()
		{

			//System.Security.Claims.ClaimsPrincipal currentUser = this.User;
			//string user_id = UserManager.GetUserId(currentUser); // Get user id:
			//var user_current = await UserManager.GetUserAsync(currentUser);
			//var subsql = "";
			//var sql = $"select type_id,COUNT(1) as num from document where deleted_at is null and status_id IN(4,5,6,7) {subsql} GROUP BY type_id";

			//var data = _context.ChartType
			//	 .FromSqlRaw(sql);
			//var d = data.Include(d => d.type).OrderByDescending(d => d.num).ToList();

			var blocking_activity = _context.ActivityModel.Where(d => d.deleted_at == null && d.blocking == true).Select(d => d.block_id + d.execution_id).ToList();
			var custom_block = _context.CustomBlockModel.Where(d => blocking_activity.Contains(d.block_id + d.execution_id)).ToList();
			var list = new List<int>();
			foreach (var block in custom_block)
			{
				var data_setting = block.data_setting;
				if (data_setting.type_performer == 3)
				{
					list.AddRange(data_setting.listdepartment);
				}
			}
			var groupedCustomerList = list
				.GroupBy(u => u)
				.Select(grp => new
				{
					count = grp.Count(),
					department = _context.DepartmentModel.Find(grp.Key)
				})
				.Where(d => d.department != null)
				.OrderByDescending(d => d.count)
				.ToList();

			var labels = new List<string>() { };
			var datasets = new List<ChartDataSet>();
			var backgroundColor = new List<string>();
			var data1 = new List<int?>();
			foreach (var record in groupedCustomerList)
			{
				labels.Add(record.department.name);
				backgroundColor.Add(record.department.color);
				data1.Add(record.count);
			}
			datasets.Add(new ChartDataSet
			{
				backgroundColor = backgroundColor,
				data = data1
			});

			//var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
			//return Json(jsonData);
			return Json(new { labels = labels, datasets = datasets });
		}

	}
	public class ChartDataSet
	{
		[Key]
		public string label { get; set; }

		public List<string> backgroundColor { get; set; }
		public List<int?> data { get; set; }

	}
}
