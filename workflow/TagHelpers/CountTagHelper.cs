using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using it.Data;
using it.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace workflow.TagHelpers
{
	public class CountTagHelper : TagHelper
	{
		private readonly ItContext _context;
		private IActionContextAccessor actionAccessor;
		private UserManager<UserModel> UserManager;

		public CountTagHelper(ItContext context, UserManager<UserModel> UserMgr, IActionContextAccessor ActionAccessor)
		{
			_context = context;
			UserManager = UserMgr;
			actionAccessor = ActionAccessor;
		}
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var user = actionAccessor.ActionContext.HttpContext.User;
			var user_id = UserManager.GetUserId(user);

			var customerData = _context.ExecutionModel.Where(m => m.deleted_at == null);
			var user_departments = _context.UserDepartmentModel.Where(d => d.user_id == user_id).Select(d => d.department_id).ToList();
			var list_exe = _context.CustomBlockModel.ToList();
			var list = list_exe.Where(d => (d.data_setting.type_performer == 4 && d.data_setting.listuser != null && d.data_setting.listuser.Contains(user_id))
			|| (d.data_setting.type_performer == 3 && check_department(d.data_setting.listdepartment, user_departments))).Select(d => d.block_id + d.execution_id).ToList();
			var execution = _context.ActivityModel.Where(d => d.blocking == true && d.deleted_at == null && list.Contains(d.block_id + d.execution_id)).Select(d => d.execution_id).ToList();
			customerData = customerData.Where(d => execution.Contains(d.id));

			var count = customerData.Count();
			if (count > 0)
			{
				output.TagName = "span";    // Replaces <email> with <a> tag

				output.Attributes.SetAttribute("class", "badge badge-danger float-right mr-2");
				if (count < 10)
				{
					output.Content.SetContent(count.ToString());
				}
				else
				{
					output.Content.SetContent("9+");
				}
			}
		}

		private bool check_department(List<int> departments, List<int> in_departments)
		{
			foreach (var department in departments)
			{
				if (in_departments.Contains(department))
				{
					return true;
				}
			}
			return false;
		}
	}
}