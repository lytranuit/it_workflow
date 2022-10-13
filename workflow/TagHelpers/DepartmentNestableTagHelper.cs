using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using it.Data;
using it.Areas.Admin.Models;

namespace workflow.TagHelpers
{
    public class DepartmentNestableTagHelper : TagHelper
    {
        private readonly ItContext _context;
        public DepartmentNestableTagHelper(ItContext context)
        {
            _context = context;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //var user = actionAccessor.ActionContext.HttpContext.User;
            //var uid = UserManager.GetUserId(user);
            //var count = _context.DocumentModel.Where(d => d.deleted_at == null && (d.user_next_signature_id == uid || d.user_next_representative_id == uid) && d.status_id == 2).Count();
            //if (count > 0)
            //{
            //    output.TagName = "span";    // Replaces <email> with <a> tag

            //    output.Attributes.SetAttribute("class", "badge badge-danger float-right mr-2");
            //    if (count < 10)
            //    {
            //        output.Content.SetContent(count.ToString());
            //    }
            //    else
            //    {
            //        output.Content.SetContent("9+");
            //    }
            //}
            var html = "<div class='dd' id='nestable_list_1'>";
            html += GetChild(0);
            html += "</div>";
            //output.TagName = "SelectGroup";
            //output.TagMode = TagMode.StartTagAndEndTag;

            //var sb = new StringBuilder();
            //sb.AppendFormat(html);

            //output.PreContent.SetHtmlContent(sb.ToString());
            output.TagName = "";
            output.Content.SetHtmlContent(html);
        }

        private string GetChild(int parent)
        {
            var DepartmentModel = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == parent).OrderBy(d => d.stt).ToList();
            var html = "";
            if (DepartmentModel.Count() > 0)
            {
                var data_id = "";
                if (parent == 0)
                    data_id = "id='nestable'";
                html += "<ol class='dd-list' " + data_id + ">";
                foreach (var department in DepartmentModel)
                {
                    var count_child = _context.DepartmentModel.Where(d => d.deleted_at == null && d.parent == department.id).Count();
                    var delete_html = "<div class='dd-nodrag btn-group ml-auto'><button class='btn btn-sm btn-outline-light dd-item-delete'><i class='far fa-trash-alt'></i></button></div>";
                    html += "<li class='dd-item' id='menuItem_" + department.id + "' data-id='" + department.id + "'><div class='dd-handle'><div>" + (count_child > 0 ? "<span class='showhide'>-</span>" : "") + "<a href='/admin/department/edit/" + department.id + "'>" + department.name + "</a></div>" + delete_html + "</div>";
                    html += GetChild(department.id);
                    html += "</li>";
                }
                html += "</ol>";
            }
            return html;
        }
    }
}