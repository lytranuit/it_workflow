
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using it.Areas.Admin.Models;
using it.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace it.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApiController : Controller
    {
        private UserManager<UserModel> UserManager;
        private RoleManager<IdentityRole> RoleManager;


        //public ApiController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        // {
        //    UserManager = UserMgr;
        //}
        private ItContext _context;
        public ApiController(ItContext context, UserManager<UserModel> UserMgr, RoleManager<IdentityRole> RoleMgr)
        {
            _context = context;
            UserManager = UserMgr;
            RoleManager = RoleMgr;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<JsonResult> Employee()
        {
            var UserModel = _context.UserModel.Where(x => x.deleted_at == null).ToList();
            var list = new List<SelectResponse>();
            foreach (var user in UserModel)
            {
                var DepartmentResponse = new SelectResponse
                {

                    id = user.Id,
                    label = user.FullName + "(" + user.Email + ")"
                };
                list.Add(DepartmentResponse);
            }
            //var jsonData = new { data = ProcessModel };
            return Json(list);
        }

        public async Task<JsonResult> Roles()
        {
            var Model = RoleManager.Roles.Select(a => new
            {
                id = a.Name,
                label = a.Name
            }).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(Model);
        }
        public async Task<JsonResult> ProcessGroupWithProcess()
        {
            var ProcessGroupModel = _context.ProcessGroupModel.Where(x => x.deleted_at == null).Include(x => x.list_process_version.Where(d => d.active == true)).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(ProcessGroupModel);
        }

        public async Task<JsonResult> Process(string id)
        {
            var ProcessModel = _context.ProcessModel.Where(x => x.id == id).Include(x => x.blocks).ThenInclude(d => d.fields.OrderBy(x => x.stt)).Include(x => x.links).FirstOrDefault();
            //var jsonData = new { data = ProcessModel };
            return Json(ProcessModel);
        }
        public async Task<JsonResult> ProcessVersion(string id)
        {
            var ProcessVersionModel = _context.ProcessVersionModel.Where(x => x.id == id).FirstOrDefault();
            //var jsonData = new { data = ProcessModel };
            return Json(ProcessVersionModel);
        }

        public async Task<JsonResult> Execution(int id)
        {
            var ExecutionModel = _context.ExecutionModel.Where(x => x.id == id).Include(d => d.user).FirstOrDefault();
            //var jsonData = new { data = ProcessModel };
            return Json(ExecutionModel);
        }

        public async Task<JsonResult> TransitionByExecution(int execution_id)
        {
            var TransitionModel = _context.TransitionModel.Where(x => x.execution_id == execution_id).OrderBy(d => d.stt).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(TransitionModel);
        }

        public async Task<JsonResult> ActivityByExecution(int execution_id)
        {
            var ActivityModel = _context.ActivityModel.Where(x => x.execution_id == execution_id).OrderBy(d => d.stt).ToList();
            //var jsonData = new { data = ProcessModel };
            return Json(ActivityModel);
        }
        public async Task<JsonResult> Department()
        {
            var All = GetChild(0);
            //var jsonData = new { data = ProcessModel };
            return Json(All);
        }

        public async Task<JsonResult> UserInfo()
        {

            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string user_id = UserManager.GetUserId(currentUser); // Get user id:
            UserModel User = await UserManager.FindByIdAsync(user_id);
            return Json(User);
        }
        public async Task<JsonResult> ProcessGroup()
        {
            var ProcessGroupModel = _context.ProcessGroupModel.Where(x => x.deleted_at == null).ToList();
            var list = new List<SelectResponse>();
            foreach (var item in ProcessGroupModel)
            {
                var Response = new SelectResponse
                {

                    id = item.id.ToString(),
                    label = item.name
                };
                list.Add(Response);
            }
            //var jsonData = new { data = ProcessModel };
            return Json(list);
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
        public async Task<JsonResult> Saveprocess(string id, ProcessModel ProcessModel, List<ProcessBlockModel> blocks, List<ProcessLinkModel> links)
        {
            var fields = new List<ProcessFieldModel>();
            if (id != ProcessModel.id)
            {
                return Json(new { success = 0 });
            }
            try
            {
                ProcessModel.blocks = null;
                ProcessModel.fields = null;
                ProcessModel.links = null;
                var ProcessModel_old = await _context.ProcessModel.FindAsync(id);
                if (ProcessModel_old == null)
                {
                    System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                    string user_id = UserManager.GetUserId(currentUser); // Get user id:
                    ProcessModel.created_at = DateTime.Now;
                    ProcessModel.user_id = user_id;
                    ProcessModel.status_id = (int)ProcessStatus.Draft;
                    _context.Add(ProcessModel);
                }
                else
                {
                    CopyValues<ProcessModel>(ProcessModel_old, ProcessModel);
                    ProcessModel_old.updated_at = DateTime.Now;
                    ProcessModel_old.status_id = (int)ProcessStatus.Draft;
                    _context.Update(ProcessModel_old);
                }
                _context.SaveChanges();

                ///Block
                ///

                var blocks_old = _context.ProcessBlockModel.Where(d => d.process_id == ProcessModel.id).Select(a => a.id).ToList();
                var list_blocks = blocks.Select(block => block.id).ToList();
                IEnumerable<string> list_delete_block = blocks_old.Except(list_blocks);
                if (list_delete_block != null)
                {
                    var removeBlocks = _context.ProcessBlockModel.Where(d => list_delete_block.Contains(d.id)).ToList();
                    _context.RemoveRange(removeBlocks);
                }
                if (blocks.Count() > 0)
                {
                    foreach (ProcessBlockModel block in blocks)
                    {
                        if (block.fields != null)
                            fields.AddRange(block.fields);
                        block.fields = null;
                        block.process_id = ProcessModel.id;
                        var existing = _context.ProcessBlockModel.Where(d => d.id == block.id).FirstOrDefault();
                        if (existing == null)
                        {
                            block.created_at = DateTime.Now;
                            _context.Add(block);
                        }
                        else
                        {
                            CopyValues<ProcessBlockModel>(existing, block);
                            _context.Update(existing);
                        }
                    }
                    //_context.SaveChanges();
                }




                ///Link
                ///
                var links_old = _context.ProcessLinkModel.Where(d => d.process_id == ProcessModel.id).Select(a => a.id).ToList();
                var list_links = links.Select(d => d.id).ToList();
                IEnumerable<string> list_delete = links_old.Except(list_links);
                if (list_delete != null)
                {
                    var removeLink = _context.ProcessLinkModel.Where(d => list_delete.Contains(d.id)).ToList();
                    _context.RemoveRange(removeLink);
                }
                if (links.Count() > 0)
                {
                    foreach (ProcessLinkModel link in links)
                    {
                        var existing = _context.ProcessLinkModel.Find(link.id);
                        if (existing == null)
                        {
                            link.process_id = ProcessModel.id;
                            _context.ProcessLinkModel.Add(link);
                        }
                        else
                        {
                            CopyValues<ProcessLinkModel>(existing, link);
                            _context.ProcessLinkModel.Update(existing);
                        }
                    }
                }


                ///Fields
                var fields_old = _context.ProcessFieldModel.Where(d => d.process_id == ProcessModel.id).Select(a => a.id).ToList();
                var list_fields = fields.Select(d => d.id).ToList();
                IEnumerable<string> list_delete_fields = fields_old.Except(list_fields);
                if (list_delete_fields != null)
                {
                    var remove = _context.ProcessFieldModel.Where(d => list_delete_fields.Contains(d.id)).ToList();
                    _context.RemoveRange(remove);
                }
                if (fields.Count() > 0)
                {
                    int index = 0;
                    foreach (ProcessFieldModel field in fields)
                    {
                        field.block = null;
                        field.stt = index++;
                        field.settings = JsonConvert.SerializeObject(field.data_setting);
                        var existing = _context.ProcessFieldModel.Find(field.id);
                        if (existing == null)
                        {
                            field.process_id = ProcessModel.id;
                            _context.ProcessFieldModel.Add(field);
                        }
                        else
                        {
                            CopyValues<ProcessFieldModel>(existing, field);
                            _context.ProcessFieldModel.Update(existing);
                        }
                    }
                }
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1 });

        }
        [HttpPost]
        public async Task<JsonResult> CreateExecution(ExecutionModel ExecutionModel)
        {
            try
            {

                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                string user_id = UserManager.GetUserId(currentUser); // Get user id:
                ExecutionModel.created_at = DateTime.Now;
                ExecutionModel.user_id = user_id;
                ExecutionModel.status_id = (int)ExecutionStatus.Executing;
                _context.Add(ExecutionModel);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1, data = ExecutionModel });

        }
        [HttpPost]
        public async Task<JsonResult> UpdateExecution(int id, ExecutionModel ExecutionModel)
        {
            var ExecutionModel_old = await _context.ExecutionModel.FindAsync(id);
            if (ExecutionModel_old != null)
            {
                CopyValues<ExecutionModel>(ExecutionModel_old, ExecutionModel);
                ExecutionModel_old.updated_at = DateTime.Now;
                _context.Update(ExecutionModel_old);
                _context.SaveChanges();
            }
            return Json(new { success = 1 });

        }

        [HttpPost]
        public async Task<JsonResult> CreateTransition(TransitionModel TransitionModel)
        {
            try
            {

                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                string user_id = UserManager.GetUserId(currentUser); // Get user id:
                TransitionModel.created_at = DateTime.Now;
                TransitionModel.created_by = user_id;
                _context.Add(TransitionModel);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1, data = TransitionModel });

        }

        [HttpPost]
        public async Task<JsonResult> CreateActivity(ActivityModel ActivityModel)
        {
            try
            {

                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                string user_id = UserManager.GetUserId(currentUser); // Get user id:
                ActivityModel.created_at = DateTime.Now;
                ActivityModel.created_by = user_id;
                ActivityModel.performer_id = user_id;
                _context.Add(ActivityModel);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Json(new { success = 1, data = ActivityModel });

        }
        // GET: Admin/Process/Delete/5
        public async Task<IActionResult> Release(string id)
        {

            if (_context.ProcessModel == null)
            {
                return Problem("Entity set 'ItContext.ProcessModel'  is null.");
            }
            var ProcessModel = _context.ProcessModel.Where(d => d.id == id).FirstOrDefault();
            if (ProcessModel != null)
            {
                ProcessModel.updated_at = DateTime.Now;
                ProcessModel.status_id = (int)ProcessStatus.Release;
                _context.ProcessModel.Update(ProcessModel);
            }
            _context.SaveChanges();
            var version = 1;
            var ProcessVersionModel_old = _context.ProcessVersionModel.Where(d => d.process_id == ProcessModel.id).OrderByDescending(d => d.version).ToList();
            if (ProcessVersionModel_old.Count > 0)
            {
                foreach (var item in ProcessVersionModel_old)
                {
                    item.active = false;
                }
                version = ProcessVersionModel_old[0].version + 1;
            }
            var json = _context.ProcessModel.Where(x => x.id == ProcessModel.id)
                .Include(x => x.blocks)
                .ThenInclude(d => d.fields.OrderBy(x => x.stt))
                .Include(x => x.links)
                .FirstOrDefault();
            var ProcessVersionModel = new ProcessVersionModel()
            {
                id = Guid.NewGuid().ToString(),
                process_id = ProcessModel.id,
                active = true,
                version = version,
                created_at = DateTime.Now,
                group_id = ProcessModel.group_id
            };


            ProcessVersionModel.process = json;
            _context.Add(ProcessVersionModel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index), nameof(Process));
        }
        public void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

    }
    public class SelectResponse
    {
        public string id { get; set; }
        public string label { get; set; }

        public virtual List<SelectResponse> children { get; set; }
    }
}
