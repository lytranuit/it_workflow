using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using it.Data;

namespace workflow.TagHelpers
{
    public class GroupTagHelper : TagHelper
    {
        private readonly ItContext _context;
        public GroupTagHelper(ItContext context)
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
            var ProcessGroupModel = _context.ProcessGroupModel.Where(d => d.deleted_at == null).ToList();
            var html = "<select id='group_id' class='form-control' name='group_id' v-model='item.group_id'>";
            foreach (var group in ProcessGroupModel)
            {
                html += "<option value='" + group.id + "'>" + group.name + "</option>";
            }
            html += "</select>";

            //output.TagName = "SelectGroup";
            //output.TagMode = TagMode.StartTagAndEndTag;

            //var sb = new StringBuilder();
            //sb.AppendFormat(html);

            //output.PreContent.SetHtmlContent(sb.ToString());
            output.TagName = "";
            output.Content.SetHtmlContent(html);
        }
    }
}