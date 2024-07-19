using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.AspNetCore.Identity;
using workflow.Services;
using Spire.Xls;
using System.Drawing;
using workflow.Areas.V1.Models;
using Vue.Models;
using Vue.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.VisualBasic;

namespace workflow.Areas.V1.Controllers
{
    public class ProcessController : BaseController
    {
        private UserManager<UserModel> UserManager;
        private string _type = "Process";
        private readonly IConfiguration _configuration;
        public ProcessController(ItContext context, UserManager<UserModel> UserMgr, IConfiguration configuration) : base(context)
        {
            ViewData["controller"] = _type;
            UserManager = UserMgr;
            _configuration = configuration;

        }
        [HttpPost]
        public async Task<JsonResult> Remove(List<string> item)
        {
            var jsonData = new { success = true, message = "" };
            try
            {
                var list = _context.ProcessModel.Where(d => item.Contains(d.id)).ToList();
                foreach (var i in list)
                {
                    i.deleted_at = DateTime.Now;
                }
                _context.UpdateRange(list);
                _context.SaveChanges();
                var list2 = _context.ProcessVersionModel.Where(d => item.Contains(d.process_id)).ToList();
                foreach (var i in list2)
                {
                    i.deleted_at = DateTime.Now;
                }
                _context.UpdateRange(list2);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                jsonData = new { success = false, message = ex.Message };
            }


            return Json(jsonData);
        }
        [HttpPost]
        public async Task<JsonResult> Release(string id)
        {

            var jsonData = new { success = true, message = "" };
            try
            {
                var ProcessModel = _context.ProcessModel.Where(d => d.id == id)
                    .Include(d => d.versions).Include(x => x.blocks.OrderBy(x => x.stt))
                    .ThenInclude(d => d.fields.OrderBy(x => x.stt))
                    .Include(x => x.links)
                    .FirstOrDefault();
                if (ProcessModel != null)
                {
                    ProcessModel.updated_at = DateTime.Now;
                    ProcessModel.status_id = (int)ProcessStatus.Release;
                    _context.ProcessModel.Update(ProcessModel);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy Process");
                }






                var version = 1;
                var ProcessVersionModel_old = ProcessModel.versions.OrderByDescending(d => d.version).ToList();
                if (ProcessVersionModel_old.Count > 0)
                {
                    foreach (var item in ProcessVersionModel_old)
                    {
                        item.active = false;
                    }
                    version = ProcessVersionModel_old[0].version + 1;
                }
                var versions = ProcessModel.versions;
                ProcessModel.versions = null;
                var ProcessVersionModel = new ProcessVersionModel()
                {
                    id = Guid.NewGuid().ToString(),
                    process_id = ProcessModel.id,
                    active = true,
                    version = version,
                    created_at = DateTime.Now,
                    group_id = ProcessModel.group_id
                };


                ProcessVersionModel.process = ProcessModel;
                _context.ProcessVersionModel.Add(ProcessVersionModel);
                _context.SaveChanges();
                foreach (var item in versions)
                {
                    var find = _context.ProcessVersionModel.Where(d => d.id == item.id).FirstOrDefault();
                    find.process_id = ProcessModel.id;

                    _context.Update(find);
                    _context.SaveChanges();
                }
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
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            var customerData = (from tempcustomer in _context.ProcessModel.Where(u => u.deleted_at == null) select tempcustomer);
            int recordsTotal = customerData.Count();
            customerData = customerData.Where(m => m.deleted_at == null);
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    customerData = customerData.Where(m => m.name.Contains(searchValue));
            //}
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
                    html_action += "<a href='/admin/api/release/" + record.id + "' class='btn btn-success btn-sm' title='Phát hành?' data-type='confirm'>"
                        + "<i class='fas fa-arrow-up'></i>"
                        + "</i>"
                        + "</a>";
                }
                else if (record.status_id == (int)ProcessStatus.Release)
                {
                    html_status = "<button class='btn btn-sm text-white btn-success'>" + ProcessStatus.Release + "</button>";
                }

                html_action += "<a href='/admin/process/export?process_id=" + record.id + "' class='btn btn-primary btn-sm export'>"
                        + "<i class=\"fas fa-download\"></i>"
                        + "</i>"
                        + "</a>";
                html_action += "<a href='/admin/" + _type + "/delete/" + record.id + "' class='btn btn-danger btn-sm' title='Xóa?' data-type='confirm'>'"
                        + "<i class='fas fa-trash-alt'>"
                        + "</i>"
                        + "</a>";
                html_action += "</div>";
                var data1 = new
                {
                    id = record.id,
                    name = record.name,
                    group = record.group,
                    status_id = record.status_id,
                };
                data.Add(data1);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Json(jsonData);
        }
        public async Task<JsonResult> exportVersion(string process_version_id)
        {
            var viewPath = "wwwroot/excel/template/process.xlsx";
            var documentPath = "/excel/data/" + DateTime.Now.ToFileTimeUtc() + ".xlsx";
            string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(viewPath);
            Worksheet sheet = workbook.Worksheets[0];

            var version = _context.ProcessVersionModel.Where(d => d.id == process_version_id).Include(d => d.executions).ThenInclude(d => d.user).FirstOrDefault();
            if (version != null)
            {
                Worksheet newSheet = workbook.CreateEmptySheet("Version " + version.version);
                newSheet.CopyFrom(sheet);


                var exclude_merge = new List<int>();
                var process = version.process;
                var blocks = process.blocks.OrderBy(d => d.stt).ToList();
                var start_c = 9;
                foreach (var block in blocks)
                {
                    block.start_c = start_c;
                    newSheet.InsertColumn(start_c, 3);
                    //newSheet.Range["F1:H1"].Merge();
                    var columnName = GetExcelLetter(start_c);
                    var columnNameEnd = GetExcelLetter(start_c + 3);
                    newSheet.Copy(newSheet.Range["F1:H3"], newSheet.Range[columnName + "1:" + columnNameEnd + "3"], true);
                    var row = newSheet.Rows[0];
                    row.Cells[start_c - 1].Value = block.label;


                    start_c += 3;
                    var fields = block.fields.OrderBy(d => d.stt).ToList();
                    if (fields.Count > 0)
                    {
                        foreach (var field in fields)
                        {
                            var data_setting = field.data_setting;
                            field.start_c = start_c;
                            if (field.type != "table")
                            {
                                newSheet.InsertColumn(start_c, 1, InsertOptionsType.FormatAsAfter);
                                var columnName_filed = GetExcelLetter(start_c);
                                newSheet.Copy(newSheet.Range["F2:F3"], newSheet.Range[columnName_filed + "2:" + columnName_filed + "3"], true);
                                var row_filed = newSheet.Rows[1];
                                row_filed.Cells[start_c - 1].Value = field.name;
                                start_c++;
                            }
                            else
                            {
                                var columns = data_setting.columns;
                                foreach (var column in columns)
                                {
                                    exclude_merge.Add(start_c);
                                    column.start_c = start_c;
                                    newSheet.InsertColumn(start_c, 1, InsertOptionsType.FormatAsAfter);
                                    var columnName_filed = GetExcelLetter(start_c);
                                    newSheet.Copy(newSheet.Range["F2:F3"], newSheet.Range[columnName_filed + "2:" + columnName_filed + "3"], true);
                                    var row_filed = newSheet.Rows[1];
                                    row_filed.Cells[start_c - 1].Value = column.name;
                                    start_c++;
                                }
                                //Console.WriteLine("table: " + columns.Count);
                            }
                            field.settings = Newtonsoft.Json.JsonConvert.SerializeObject(data_setting);
                        }

                    }





                    columnNameEnd = GetExcelLetter(start_c - 1);
                    newSheet.Range[columnName + "1"].UnMerge();
                    newSheet.Range[columnName + "1:" + columnNameEnd + "1"].Merge();


                }
                var user_create = _context.UserModel.Where(d => d.Id == process.user_id).FirstOrDefault();
                var executions = version.executions.Where(d => d.deleted_at == null).ToList();
                var start_r = 3;
                foreach (var execution in executions)
                {
                    newSheet.InsertRow(start_r + 1);
                    //newSheet.Copy(newSheet.Range["A" + start_r + ":HZ" + start_r], newSheet.Range["A" + (start_r + 1) + ":HZ" + (start_r + 1)], true);
                    var row = newSheet.Rows[start_r];
                    var first_r = start_r;
                    var end_row = start_r;
                    row.Cells[0].Value = execution.id.ToString();
                    row.Cells[1].Value = execution.title.ToString();
                    row.Cells[2].DateTimeValue = execution.created_at.Value;
                    row.Cells[2].Style.NumberFormat = "yyyy-MM-dd HH:mm:ss"; // set US datetime format
                    row.Cells[3].Value = execution.user.FullName;
                    row.Cells[4].Value = execution.status;
                    foreach (var block in blocks)
                    {
                        var block_id = block.id;
                        var activity = _context.ActivityModel.Where(d => d.block_id == block_id && d.execution_id == execution.id).Include(d => d.user_created_by).Include(d => d.fields).OrderBy(d => d.stt).LastOrDefault();
                        if (activity == null || activity.executed == false)
                            continue;
                        var start_col = (int)block.start_c;
                        if (activity.created_at != null)
                        {
                            row.Cells[(start_col - 1)].DateTimeValue = activity.created_at.Value;
                            row.Cells[(start_col - 1)].Style.NumberFormat = "yyyy-MM-dd HH:mm:ss"; // set US datetime format
                        }
                        row.Cells[(start_col - 1) + 1].Value = activity.user_created_by != null ? activity.user_created_by.FullName : "";
                        row.Cells[(start_col - 1) + 2].Value = "";
                        var fields_ac = activity.fields.OrderBy(d => d.stt).ToList();
                        var fields = block.fields.OrderBy(d => d.stt).ToList();
                        foreach (var field in fields)
                        {
                            var start_col_field = (int)field.start_c;
                            var data_setting = field.data_setting;
                            var field_ac = activity.fields.Where(d => d.name == field.name).FirstOrDefault();
                            var values = field_ac.values;
                            var text = values.value;
                            if (field.type == "select" || field.type == "radio")
                            {
                                var options = data_setting.options;
                                var option = options.Where(d => d.id == values.value).FirstOrDefault();
                                text = option.name;
                                row.Cells[(start_col_field - 1)].Value = text;
                            }
                            else if (field.type == "department")
                            {
                                var department = _context.DepartmentModel.Where(d => d.id == Int32.Parse(values.value)).FirstOrDefault();
                                text = department.name;
                                row.Cells[(start_col_field - 1)].Value = text;
                            }
                            else if (field.type == "employee")
                            {
                                var employee = _context.UserModel.Where(d => d.Id == values.value).FirstOrDefault();
                                text = employee.FullName;
                                row.Cells[(start_col_field - 1)].Value = text;
                            }
                            else if (field.type == "date")
                            {
                                if (values.value != null)
                                {
                                    var datetime = DateTime.Parse(values.value);
                                    row.Cells[(start_col_field - 1)].DateTimeValue = datetime;
                                    row.Cells[(start_col_field - 1)].Style.NumberFormat = "yyyy-MM-dd"; // set US datetime format
                                }

                            }
                            else if (field.type == "date_month")
                            {
                                if (values.value != null)
                                {
                                    var datetime = DateTime.Parse(values.value);
                                    row.Cells[(start_col_field - 1)].DateTimeValue = datetime;
                                    row.Cells[(start_col_field - 1)].Style.NumberFormat = "yyyy-MM"; // set US datetime format
                                }
                            }
                            else if (field.type == "date_time")
                            {
                                if (values.value != null)
                                {
                                    var datetime = DateTime.Parse(values.value);
                                    row.Cells[(start_col_field - 1)].DateTimeValue = datetime;
                                    row.Cells[(start_col_field - 1)].Style.NumberFormat = "yyyy-MM-dd HH:mm:ss"; // set US datetime format
                                }
                            }
                            else if (field.type == "file" || field.type == "file_multiple")
                            {
                                CellRange cell = row.Cells[(start_col_field - 1)];
                                if (values.files != null)
                                {
                                    foreach (var file in values.files)
                                    {
                                        HyperLink urlLink = newSheet.HyperLinks.Add(cell);

                                        urlLink.Type = HyperLinkType.Url;

                                        urlLink.TextToDisplay = file.name;


                                        urlLink.Address = Domain + file.url + "?token=" + _configuration["Key_Access"];

                                    }
                                }


                                //var files = values.files.Select(d => d.name).ToList();
                                //text = String.Join(", ", files);
                                //row.Cells[(start_col_field - 1)].Value = text;
                            }
                            else if (field.type == "select_multiple" || field.type == "checkbox")
                            {
                                if (values.value_array != null)
                                {
                                    var options = data_setting.options;
                                    var option = options.Where(d => values.value_array.Contains(d.id)).Select(d => d.name).ToList();
                                    text = option != null ? String.Join(", ", option) : "";
                                    row.Cells[(start_col_field - 1)].Value = text;
                                }
                            }
                            else if (field.type == "department_multiple")
                            {
                                //var options = data_setting.options;
                                if (values.value_array != null)
                                {
                                    var option = _context.DepartmentModel.Where(d => values.value_array.Contains(d.id.ToString())).Select(d => d.name).ToList();
                                    text = option != null ? String.Join(", ", option) : "";
                                    row.Cells[(start_col_field - 1)].Value = text;
                                }
                            }
                            else if (field.type == "employee_multiple")
                            {
                                if (values.value_array != null)
                                {
                                    //var options = data_setting.options;
                                    var option = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.FullName).ToList();
                                    text = option != null ? String.Join(", ", option) : "";
                                    row.Cells[(start_col_field - 1)].Value = text;
                                }

                            }
                            else if (field.type == "table")
                            {
                                var columns = data_setting.columns;
                                var list_data = values.list_data;
                                var start_r_column = start_r;
                                ///CHECK UPDATE END ROW
                                if ((start_r_column + list_data.Count) > end_row)
                                {
                                    ///THÊM ROW VÀO
                                    newSheet.InsertRow((start_r_column + 1) + (end_row - start_r_column + 1), (start_r_column + list_data.Count - 1) - end_row);

                                    end_row = (start_r_column + list_data.Count) - 1;
                                }
                                for (var i = 0; i < list_data.Count; i++)
                                {
                                    var data = list_data[i];

                                    CellRange? new_row = null;
                                    if (i != 0)
                                    {
                                        new_row = newSheet.Rows[start_r_column + 1];
                                        start_r_column++;
                                    }
                                    foreach (var column in columns)
                                    {
                                        var start_col_table = (int)column.start_c;
                                        var value_column = data.ContainsKey(column.id) ? data[column.id] : "";
                                        if (column.type == "currency")
                                        {
                                            if (i != 0)
                                            {
                                                new_row.Cells[(start_col_table - 1)].NumberValue = double.Parse(value_column.ToString());
                                                new_row.Cells[(start_col_table - 1)].NumberFormat = "#,##0.00";
                                            }
                                            else
                                            {
                                                row.Cells[(start_col_table - 1)].NumberValue = double.Parse(value_column.ToString());
                                                row.Cells[(start_col_table - 1)].NumberFormat = "#,##0.00";
                                            }
                                        }
                                        else
                                        {
                                            if (i != 0)
                                            {
                                                new_row.Cells[(start_col_table - 1)].Value = value_column;
                                            }
                                            else
                                            {
                                                row.Cells[(start_col_table - 1)].Value = value_column;
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                row.Cells[(start_col_field - 1)].Value = text;
                            }
                        }
                    }
                    ///MERGE
                    if (first_r != end_row)
                    {
                        var count_column = newSheet.Columns.Count();
                        for (var i = 0; i < count_column; i++)
                        {
                            if (!exclude_merge.Contains(i + 1))
                            {
                                var columnName = GetExcelLetter(i + 1);
                                //Console.WriteLine(columnName);
                                Console.WriteLine("Columns: " + columnName + first_r + ":" + columnName + end_row);
                                newSheet.Range[columnName + (first_r + 1) + ":" + columnName + (end_row + 1)].Merge();
                            }

                        }
                    }

                    start_r = start_r + (end_row - first_r) + 1;
                }

                ////STYLE
                var last_col = GetExcelLetter(newSheet.Columns.Count() - 1);
                var last_row = newSheet.Rows.Count() - 1;
                var range = newSheet.Range["A3:" + last_col + last_row];

                //Console.WriteLine("Columns: " + first_col + "3:" + last_col + start_r);
                range.BorderInside(LineStyleType.Thin, Color.Black);
                range.BorderAround(LineStyleType.Thin, Color.Black);
                range.Style.VerticalAlignment = VerticalAlignType.Center;
                range.Style.HorizontalAlignment = HorizontalAlignType.Center;



                //////
                newSheet.DeleteRow(3);
                newSheet.DeleteColumn(6, 3);
                newSheet.AllocatedRange.AutoFitColumns();

            }

            sheet.Remove();

            sheet = workbook.Worksheets[0];
            sheet.Activate();
            workbook.SaveToFile("./wwwroot" + documentPath, ExcelVersion.Version2013);

            return Json(new
            {
                success = true,
                link = documentPath
            });
        }

        [HttpPost]
        public async Task<JsonResult> export(string process_id)
        {
            var viewPath = "wwwroot/excel/template/process.xlsx";
            var documentPath = "/excel/data/" + DateTime.Now.ToFileTimeUtc() + ".xlsx";
            string Domain = (HttpContext.Request.IsHttps ? "https://" : "http://") + HttpContext.Request.Host.Value;

            Workbook workbook = new Workbook();
            workbook.LoadFromFile(viewPath);
            Worksheet sheet = workbook.Worksheets[0];

            var ProcessVersion = _context.ProcessVersionModel.Where(d => d.process_id == process_id).Include(d => d.executions).ThenInclude(d => d.user).OrderByDescending(d => d.version).ToList();
            if (ProcessVersion.Count > 0)
            {

                foreach (var version in ProcessVersion)
                {
                    var executions = version.executions.Where(d => d.deleted_at == null).ToList();
                    if (executions.Count() == 0)
                        continue;
                    Worksheet newSheet = workbook.CreateEmptySheet("Version " + version.version);
                    newSheet.CopyFrom(sheet);


                    var exclude_merge = new List<int>();
                    var process = version.process;
                    var blocks = process.blocks.OrderBy(d => d.stt).ToList();
                    var start_c = 9;
                    foreach (var block in blocks)
                    {
                        block.start_c = start_c;
                        newSheet.InsertColumn(start_c, 3);
                        //newSheet.Range["F1:H1"].Merge();
                        var columnName = GetExcelLetter(start_c);
                        var columnNameEnd = GetExcelLetter(start_c + 3);
                        newSheet.Copy(newSheet.Range["F1:H3"], newSheet.Range[columnName + "1:" + columnNameEnd + "3"], true);
                        var row = newSheet.Rows[0];
                        row.Cells[start_c - 1].Value = block.label;


                        start_c += 3;
                        var fields = block.fields.OrderBy(d => d.stt).ToList();
                        if (fields.Count > 0)
                        {
                            foreach (var field in fields)
                            {
                                var data_setting = field.data_setting;
                                field.start_c = start_c;
                                if (field.type != "table")
                                {
                                    newSheet.InsertColumn(start_c, 1, InsertOptionsType.FormatAsAfter);
                                    var columnName_filed = GetExcelLetter(start_c);
                                    newSheet.Copy(newSheet.Range["F2:F3"], newSheet.Range[columnName_filed + "2:" + columnName_filed + "3"], true);
                                    var row_filed = newSheet.Rows[1];
                                    row_filed.Cells[start_c - 1].Value = field.name;
                                    start_c++;
                                }
                                else
                                {
                                    var columns = data_setting.columns;
                                    foreach (var column in columns)
                                    {
                                        exclude_merge.Add(start_c);
                                        column.start_c = start_c;
                                        newSheet.InsertColumn(start_c, 1, InsertOptionsType.FormatAsAfter);
                                        var columnName_filed = GetExcelLetter(start_c);
                                        newSheet.Copy(newSheet.Range["F2:F3"], newSheet.Range[columnName_filed + "2:" + columnName_filed + "3"], true);
                                        var row_filed = newSheet.Rows[1];
                                        row_filed.Cells[start_c - 1].Value = column.name;
                                        start_c++;
                                    }
                                    //Console.WriteLine("table: " + columns.Count);
                                }
                                field.settings = Newtonsoft.Json.JsonConvert.SerializeObject(data_setting);
                            }

                        }





                        columnNameEnd = GetExcelLetter(start_c - 1);
                        newSheet.Range[columnName + "1"].UnMerge();
                        newSheet.Range[columnName + "1:" + columnNameEnd + "1"].Merge();


                    }
                    var user_create = _context.UserModel.Where(d => d.Id == process.user_id).FirstOrDefault();

                    var start_r = 3;
                    foreach (var execution in executions)
                    {
                        newSheet.InsertRow(start_r + 1);
                        //newSheet.Copy(newSheet.Range["A" + start_r + ":HZ" + start_r], newSheet.Range["A" + (start_r + 1) + ":HZ" + (start_r + 1)], true);
                        var row = newSheet.Rows[start_r];
                        var first_r = start_r;
                        var end_row = start_r;
                        row.Cells[0].Value = execution.id.ToString();
                        row.Cells[1].Value = execution.title.ToString();
                        row.Cells[2].DateTimeValue = execution.created_at.Value;
                        row.Cells[2].Style.NumberFormat = "yyyy-MM-dd HH:mm:ss"; // set US datetime format
                        row.Cells[3].Value = execution.user.FullName;
                        row.Cells[4].Value = execution.status;
                        foreach (var block in blocks)
                        {
                            var block_id = block.id;
                            var activity = _context.ActivityModel.Where(d => d.block_id == block_id && d.execution_id == execution.id).Include(d => d.user_created_by).Include(d => d.fields).OrderBy(d => d.stt).LastOrDefault();
                            if (activity == null || activity.executed == false)
                                continue;
                            var start_col = (int)block.start_c;
                            if (activity.created_at != null)
                            {
                                row.Cells[(start_col - 1)].DateTimeValue = activity.created_at.Value;
                                row.Cells[(start_col - 1)].Style.NumberFormat = "yyyy-MM-dd HH:mm:ss"; // set US datetime format
                            }
                            row.Cells[(start_col - 1) + 1].Value = activity.user_created_by != null ? activity.user_created_by.FullName : "";
                            row.Cells[(start_col - 1) + 2].Value = "";
                            var fields_ac = activity.fields.OrderBy(d => d.stt).ToList();
                            var fields = block.fields.OrderBy(d => d.stt).ToList();
                            foreach (var field in fields)
                            {
                                var start_col_field = (int)field.start_c;
                                var data_setting = field.data_setting;
                                var field_ac = activity.fields.Where(d => d.name == field.name).FirstOrDefault();
                                var values = field_ac.values;
                                var text = values.value;
                                if (field.type == "select" || field.type == "radio")
                                {
                                    var options = data_setting.options;
                                    var option = options.Where(d => d.id == values.value).FirstOrDefault();
                                    text = option.name;
                                    row.Cells[(start_col_field - 1)].Value = text;
                                }
                                else if (field.type == "department")
                                {
                                    var department = _context.DepartmentModel.Where(d => d.id == Int32.Parse(values.value)).FirstOrDefault();
                                    text = department.name;
                                    row.Cells[(start_col_field - 1)].Value = text;
                                }
                                else if (field.type == "employee")
                                {
                                    var employee = _context.UserModel.Where(d => d.Id == values.value).FirstOrDefault();
                                    text = employee.FullName;
                                    row.Cells[(start_col_field - 1)].Value = text;
                                }
                                else if (field.type == "date")
                                {
                                    if (values.value != null)
                                    {
                                        var datetime = DateTime.Parse(values.value);
                                        row.Cells[(start_col_field - 1)].DateTimeValue = datetime;
                                        row.Cells[(start_col_field - 1)].Style.NumberFormat = "yyyy-MM-dd"; // set US datetime format
                                    }

                                }
                                else if (field.type == "date_month")
                                {
                                    if (values.value != null)
                                    {
                                        var datetime = DateTime.Parse(values.value);
                                        row.Cells[(start_col_field - 1)].DateTimeValue = datetime;
                                        row.Cells[(start_col_field - 1)].Style.NumberFormat = "yyyy-MM"; // set US datetime format
                                    }
                                }
                                else if (field.type == "date_time")
                                {
                                    if (values.value != null)
                                    {
                                        var datetime = DateTime.Parse(values.value);
                                        row.Cells[(start_col_field - 1)].DateTimeValue = datetime;
                                        row.Cells[(start_col_field - 1)].Style.NumberFormat = "yyyy-MM-dd HH:mm:ss"; // set US datetime format
                                    }
                                }
                                else if (field.type == "file" || field.type == "file_multiple")
                                {
                                    CellRange cell = row.Cells[(start_col_field - 1)];
                                    cell.Style.WrapText = true;
                                    if (values.files != null)
                                    {
                                        foreach (var file in values.files)
                                        {
                                            HyperLink urlLink = newSheet.HyperLinks.Add(cell);

                                            urlLink.Type = HyperLinkType.Url;

                                            urlLink.TextToDisplay = file.name;

                                            urlLink.Address = Domain + file.url + "?token=" + _configuration["Key_Access"];

                                        }
                                    }


                                    //var files = values.files.Select(d => d.name).ToList();
                                    //text = String.Join(", ", files);
                                    //row.Cells[(start_col_field - 1)].Value = text;
                                }
                                else if (field.type == "select_multiple" || field.type == "checkbox")
                                {
                                    if (values.value_array != null)
                                    {
                                        var options = data_setting.options;
                                        var option = options.Where(d => values.value_array.Contains(d.id)).Select(d => d.name).ToList();
                                        text = option != null ? String.Join(", ", option) : "";
                                        row.Cells[(start_col_field - 1)].Value = text;
                                    }
                                }
                                else if (field.type == "department_multiple")
                                {
                                    if (values.value_array != null)
                                    {
                                        //var options = data_setting.options;
                                        var option = _context.DepartmentModel.Where(d => values.value_array.Contains(d.id.ToString())).Select(d => d.name).ToList();
                                        text = option != null ? String.Join(", ", option) : "";
                                        row.Cells[(start_col_field - 1)].Value = text;
                                    }
                                }
                                else if (field.type == "employee_multiple")
                                {
                                    if (values.value_array != null)
                                    {
                                        //var options = data_setting.options;
                                        var option = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.FullName).ToList();
                                        text = option != null ? String.Join(", ", option) : "";
                                        row.Cells[(start_col_field - 1)].Value = text;
                                    }
                                }
                                else if (field.type == "table")
                                {
                                    var columns = data_setting.columns;
                                    var list_data = values.list_data;
                                    var start_r_column = start_r;
                                    ///CHECK UPDATE END ROW
                                    if ((start_r_column + list_data.Count) > end_row)
                                    {
                                        ///THÊM ROW VÀO
                                        newSheet.InsertRow((start_r_column + 1) + (end_row - start_r_column + 1), (start_r_column + list_data.Count - 1) - end_row);

                                        end_row = (start_r_column + list_data.Count) - 1;
                                    }
                                    for (var i = 0; i < list_data.Count; i++)
                                    {
                                        var data = list_data[i];

                                        CellRange? new_row = null;
                                        if (i != 0)
                                        {
                                            new_row = newSheet.Rows[start_r_column + 1];
                                            start_r_column++;
                                        }
                                        foreach (var column in columns)
                                        {
                                            var start_col_table = (int)column.start_c;
                                            var value_column = data.ContainsKey(column.id) ? data[column.id] : "";
                                            if (column.type == "currency")
                                            {
                                                if (i != 0)
                                                {
                                                    new_row.Cells[(start_col_table - 1)].NumberValue = double.Parse(value_column.ToString());
                                                    new_row.Cells[(start_col_table - 1)].NumberFormat = "#,##0.00";
                                                }
                                                else
                                                {
                                                    row.Cells[(start_col_table - 1)].NumberValue = double.Parse(value_column.ToString());
                                                    row.Cells[(start_col_table - 1)].NumberFormat = "#,##0.00";
                                                }
                                            }
                                            else
                                            {
                                                if (i != 0)
                                                {
                                                    new_row.Cells[(start_col_table - 1)].Value = value_column;
                                                }
                                                else
                                                {
                                                    row.Cells[(start_col_table - 1)].Value = value_column;
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    row.Cells[(start_col_field - 1)].Value = text;
                                }
                            }
                        }
                        ///MERGE
                        if (first_r != end_row)
                        {
                            var count_column = newSheet.Columns.Count();
                            for (var i = 0; i < count_column; i++)
                            {
                                if (!exclude_merge.Contains(i + 1))
                                {
                                    var columnName = GetExcelLetter(i + 1);
                                    //Console.WriteLine(columnName);
                                    Console.WriteLine("Columns: " + columnName + first_r + ":" + columnName + end_row);
                                    newSheet.Range[columnName + (first_r + 1) + ":" + columnName + (end_row + 1)].Merge();
                                }

                            }
                        }

                        start_r = start_r + (end_row - first_r) + 1;
                    }

                    ////STYLE
                    var last_col = GetExcelLetter(newSheet.Columns.Count() - 1);
                    var last_row = newSheet.Rows.Count() - 1;
                    var range = newSheet.Range["A3:" + last_col + last_row];

                    //Console.WriteLine("Columns: " + first_col + "3:" + last_col + start_r);
                    range.BorderInside(LineStyleType.Thin, Color.Black);
                    range.BorderAround(LineStyleType.Thin, Color.Black);
                    range.Style.VerticalAlignment = VerticalAlignType.Center;
                    range.Style.HorizontalAlignment = HorizontalAlignType.Center;



                    //////
                    newSheet.DeleteRow(3);
                    newSheet.DeleteColumn(6, 3);
                    newSheet.AllocatedRange.AutoFitColumns();

                }

                sheet.Remove();
            }
            sheet = workbook.Worksheets[0];
            sheet.Activate();
            workbook.SaveToFile("./wwwroot" + documentPath, ExcelVersion.Version2013);

            return Json(new { success = true, link = documentPath });
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
        private static string GetExcelLetter(int columnNum)
        {
            int num = columnNum;
            int mod = 0;
            string result = String.Empty;
            while (num > 0)
            {
                mod = (num - 1) % 26;
                result = (char)(65 + mod) + result;
                num = (int)((num - mod) / 26);
            }
            return result;
        }
    }
}
