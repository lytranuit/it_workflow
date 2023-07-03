using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.AspNetCore.Identity;
using Vue.Data;
using workflow.Areas.V1.Models;
using Vue.Models;

namespace workflow.Areas.V1.Controllers
{
    public class ProcessGroupController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private string _type = "ProcessGroup";
        public ProcessGroupController(ItContext context, ItContext itContext, UserManager<UserModel> UserMgr) : base(context)
        {
            ViewData["controller"] = _type;
            UserManager = UserMgr;
        }
        [HttpPost]
        public async Task<JsonResult> Save(ProcessGroupModel ProcessGroupModel)
        {
            var jsonData = new { success = true, message = "" };
            try
            {
                if (ProcessGroupModel.id > 0)
                {
                    _context.Update(ProcessGroupModel);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Add(ProcessGroupModel);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                jsonData = new
                {
                    success = false,
                    message = ex.Message
                };
            }


            return Json(jsonData);
        }

        [HttpPost]
        public async Task<JsonResult> Remove(List<int> item)
        {
            var jsonData = new { success = true, message = "" };
            try
            {
                var list = _context.ProcessGroupModel.Where(d => item.Contains(d.id)).ToList();
                _context.RemoveRange(list);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                jsonData = new { success = false, message = ex.Message };
            }


            return Json(jsonData);
        }
        [HttpPost]
        public async Task<JsonResult> Table()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var id = Request.Form["filters[id]"].FirstOrDefault();
            int id_int = id != null ? Convert.ToInt32(id) : 0;
            var name = Request.Form["filters[name]"].FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var customerData = from tempcustomer in _context.ProcessGroupModel.Where(u => u.deleted_at == null) select tempcustomer;
            int recordsTotal = customerData.Count();
            customerData = customerData.Where(m => m.deleted_at == null);
            if (id != null && id != "")
            {
                customerData = customerData.Where(d => d.id == id_int);
            }
            if (name != null && name != "")
            {
                customerData = customerData.Where(d => d.name.Contains(name));
            }
            int recordsFiltered = customerData.Count();
            var datapost = customerData.Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();
            foreach (var record in datapost)
            {
                var data1 = new
                {
                    id = record.id,
                    name = record.name,
                    html_color = "<div style='background:" + record.color + ";display:inline-block;width:50px;height:20px;'></div>",
                    color = record.color,
                };
                data.Add(data1);
            }
            var jsonData = new { draw, recordsFiltered, recordsTotal, data };
            return Json(jsonData);
        }


    }
}
