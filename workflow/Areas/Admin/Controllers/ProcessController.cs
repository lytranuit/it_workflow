using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using it.Areas.Admin.Models;
using it.Data;
using System.Collections;
using Microsoft.AspNetCore.Identity;
using workflow.Services;
using Newtonsoft.Json;

namespace it.Areas.Admin.Controllers
{
    public class ProcessController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private string _type = "Process";
        public ProcessController(ItContext context, UserManager<UserModel> UserMgr) : base(context)
        {
            ViewData["controller"] = _type;
            UserManager = UserMgr;
        }

        // GET: Admin/Process
        public IActionResult Index()
        {
            return View();
        }


        // GET: Admin/Process/Create
        public IActionResult Create()
        {

            return View();
        }

        // GET: Admin/Process/Edit/5
        public IActionResult Edit(string? id)
        {
            if (id == null || _context.ProcessModel == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Save(string id, ProcessModel ProcessModel, List<ProcessBlockModel> blocks, List<ProcessLinkModel> links)
        {
            var fields = new List<ProcessFieldModel>();
            if (id != ProcessModel.id)
            {
                return Json(new { success = 0 });
            }
            try
            {
                var ProcessModel_old = await _context.ProcessModel.FindAsync(id);
                if (ProcessModel_old == null)
                {
                    System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                    string user_id = UserManager.GetUserId(currentUser); // Get user id:
                    ProcessModel.created_at = DateTime.Now;
                    ProcessModel.user_id = user_id;
                    ProcessModel.status_id = (int)ProcessStatus.Draft;
                    ProcessModel.blocks = null;
                    ProcessModel.fields = null;
                    ProcessModel.links = null;
                    _context.Add(ProcessModel);
                }
                else
                {
                    CopyValues<ProcessModel>(ProcessModel_old, ProcessModel);
                    ProcessModel_old.updated_at = DateTime.Now;
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


        // GET: Admin/Process/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.ProcessModel == null)
            {
                return Problem("Entity set 'ItContext.ProcessModel'  is null.");
            }
            var ProcessModel = _context.ProcessModel.Where(d => d.id == id).FirstOrDefault();
            if (ProcessModel != null)
            {
                ProcessModel.deleted_at = DateTime.Now;
                _context.ProcessModel.Update(ProcessModel);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
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
            var customerData = (from tempcustomer in _context.ProcessModel.Where(u => u.deleted_at == null) select tempcustomer);
            int recordsTotal = customerData.Count();
            customerData = customerData.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(searchValue))
            {
                customerData = customerData.Where(m => m.name.Contains(searchValue));
            }
            int recordsFiltered = customerData.Count();
            customerData = customerData.Include(m => m.group);
            var datapost = customerData.Skip(skip).Take(pageSize).ToList();
            var data = new ArrayList();

            foreach (var record in datapost)
            {
                var group = record.group;
                var html_group = "<button class='btn btn-sm text-white' style='background:" + group.color + "'>" + group.name + "</button>";
                var html_status = "";
                var html_action = "<div class='btn-group'>";
                if (record.status_id == (int)ProcessStatus.Draft)
                {
                    html_status = "<button class='btn btn-sm text-white btn-info'>" + ProcessStatus.Draft + "</button>";
                    html_action += "<a href='/admin/" + _type + "/release/" + record.id + "' class='btn btn-success btn-sm' title='Phát hành?' data-type='confirm'>"
                        + "<i class='fas fa-arrow-up'></i>"
                        + "</i>"
                        + "</a>";
                }
                else if (record.status_id == (int)ProcessStatus.Release)
                {
                    html_status = "<button class='btn btn-sm text-white btn-success'>" + ProcessStatus.Release + "</button>";
                }

                html_action += "<a href='/admin/" + _type + "/delete/" + record.id + "' class='btn btn-danger btn-sm' title='Xóa?' data-type='confirm'>'"
                        + "<i class='fas fa-trash-alt'>"
                        + "</i>"
                        + "</a>";
                html_action += "</div>";
                var data1 = new
                {
                    action = html_action,
                    id = "<a href='/admin/" + _type + "/edit/" + record.id + "'><i class='fas fa-pencil-alt mr-2'></i> " + record.id + "</a>",
                    name = record.name,
                    status = html_status,
                    group = html_group
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }


        public async Task<JsonResult> Get(string id)
        {
            var ProcessModel = _context.ProcessModel.Where(x => x.id == id).Include(x => x.blocks).Include(x => x.links).FirstOrDefault();
            //var jsonData = new { data = ProcessModel };
            return Json(ProcessModel);
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
}
