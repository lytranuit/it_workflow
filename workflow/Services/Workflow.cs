
using it.Areas.Admin.Models;
using it.Data;
using Microsoft.EntityFrameworkCore;
using Spire.Doc.Documents;
using Spire.Doc;
using System.Diagnostics;
using System.Globalization;
using Spire.Doc.Fields;
using System.Data;
using System.Collections;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Linq;

namespace it.Services
{
	public class Workflow
	{
		protected readonly ItContext _context;
		private IActionContextAccessor actionAccessor;

		private readonly IConfiguration _configuration;
		public Workflow(IConfiguration configuration, ItContext context, IActionContextAccessor ActionAccessor)
		{
			_configuration = configuration;
			_context = context;
			actionAccessor = ActionAccessor;
		}
		public void create_next(ActivityModel activity)
		{
			var execution = _context.ExecutionModel.Where(d => d.id == activity.execution_id).Include(d => d.process_version).FirstOrDefault();
			var transitions = _context.TransitionModel.Where(d => d.execution_id == activity.execution_id && d.deleted_at == null).OrderBy(d => d.created_at).ToList();
			var activites = _context.ActivityModel.Where(d => d.execution_id == activity.execution_id && d.deleted_at == null).OrderBy(d => d.created_at).ToList();
			var process_version = execution.process_version;
			var process = process_version.process;
			var blocks = process.blocks;
			var links = process.links;

			var node = blocks.Where(d => d.id == activity.block_id).FirstOrDefault();
			if (activity.clazz == "inclusiveGateway")
			{
				var ins = getInEdges(process, node);
				ins = ins.Where(d => d.reverse == false).ToList();
				var findTransitions = transitions.Where(d => d.to_activity_id == activity.id).ToList();
				if (findTransitions.Count < ins.Count)
				{
					activity.blocking = true;
				}
				else
				{
					activity.blocking = false;
				}
				_context.Update(activity);
				_context.SaveChanges();
			}

			if (activity.blocking == true)
				return;

			var outs = getOutEdges(process, node);
			outs.Where(d => d.reverse == activity.failed).ToList();
			if (outs.Count > 0)
			{
				foreach (var outEdge in outs)
				{
					var source = getSource(process, outEdge);
					var target = getTarget(process, outEdge);
					var transition = new TransitionModel()
					{
						label = outEdge.label,
						reverse = outEdge.reverse,
						link_id = outEdge.id,
						execution_id = execution.id,
						from_block_id = source.id,
						to_block_id = target.id,
						from_activity_id = activity.id,
						//to_activity_id: activity.id,
						stt = transitions[transitions.Count - 1].stt + 1,
						id = Guid.NewGuid().ToString(),
						created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff",
						created_at = DateTime.Now
					};
					_context.Add(transition);
					transitions.Add(transition);

					///CREATE TARGET ACTIVITY
					var create_new = true;
					ActivityModel? activity_new = null;
					if (target.clazz == "inclusiveGateway")
					{
						activity_new = activites.Where(d => d.block_id == target.id).FirstOrDefault();
						if (activity_new != null)
						{
							create_new = false;
						}
					}
					if (create_new == true)
					{
						var blocking = false;
						if (target.clazz == "formTask" || target.clazz == "approveTask" || target.clazz == "suggestTask" || target.clazz == "mailSystem" || target.clazz == "printSystem")
						{
							blocking = true;
						}

						activity_new = new ActivityModel()
						{
							execution_id = execution.id,
							label = target.label,
							block_id = target.id,
							variable = target.variable,
							stt = activites[activites.Count - 1].stt + 1,
							clazz = target.clazz,
							executed = !blocking,
							failed = false,
							blocking = blocking,
							id = Guid.NewGuid().ToString(),
							//fields= fields,
							data_setting = target.data_setting,
							//created_by= that.current_user.id,
							started_at = DateTime.Now
						};
						if (blocking == false)
						{
							activity_new.created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
							activity_new.created_at = DateTime.Now;
						}
						_context.Add(activity_new);
						activites.Add(activity_new);
					}
					////
					if (activity_new != null)
						transition.to_activity_id = activity_new.id;
					////
					if (activity_new.blocking == true)
					{
						var custom_block = _context.CustomBlockModel.Where(d => d.block_id == activity_new.block_id && d.execution_id == activity_new.execution_id).FirstOrDefault();
						if (custom_block == null)
						{
							var data_setting_block = target.data_setting;
							var type_performer = target.type_performer;
							var data_setting = new CustomBlockSettings() { };
							if (type_performer == 1 && data_setting_block.block_id == null)
							{
								data_setting.type_performer = 4;
								//data_setting.listuser = [that.current_user.id];
							}
							else if (type_performer == 1 && data_setting_block.block_id != null)
							{
								data_setting.type_performer = 4;
								var findActivity = activites.Where(d => d.block_id == data_setting_block.block_id).FirstOrDefault();
								if (findActivity != null)
									data_setting.listuser = new List<string>() { findActivity.created_by };
							}
							else if (type_performer == 4)
							{
								data_setting.type_performer = type_performer;
								data_setting.listuser = data_setting_block.listuser;
							}
							else if (type_performer == 3)
							{
								data_setting.type_performer = type_performer;
								data_setting.listdepartment = data_setting_block.listdepartment;
							}
							else if (type_performer == 5)
							{
								data_setting.type_performer = 4;
								data_setting.listuser = new List<string>() { execution.user_id };
							}
							custom_block = new CustomBlockModel()
							{
								data_setting = data_setting,
								block_id = target.id,
								execution_id = execution.id
							};
							_context.Add(custom_block);
						}
					}
					_context.SaveChanges();
					create_next(activity_new);
				}
			}
		}
		public List<ProcessLinkModel>? getInEdges(ProcessModel process, ProcessBlockModel node)
		{
			//var blocks = process.blocks;
			var links = process.links;
			var node_id = node.id;
			var ins = links.Where(d => d.target == node_id).ToList();
			return ins;
		}
		public List<ProcessLinkModel>? getOutEdges(ProcessModel process, ProcessBlockModel node)
		{
			//var blocks = process.blocks;
			var links = process.links;
			var node_id = node.id;
			var ins = links.Where(d => d.source == node_id).ToList();
			return ins;
		}

		public ProcessBlockModel? getSource(ProcessModel process, ProcessLinkModel Edge)
		{
			//var blocks = process.blocks;
			var nodes = process.blocks;
			var node = nodes.Where(d => d.id == Edge.source).FirstOrDefault();
			return node;
		}
		public ProcessBlockModel? getTarget(ProcessModel process, ProcessLinkModel Edge)
		{
			//var blocks = process.blocks;
			var nodes = process.blocks;
			var node = nodes.Where(d => d.id == Edge.target).FirstOrDefault();
			return node;
		}
		public async Task<Boolean> PrintTask(ActivityModel ActivityModel)
		{

			var ExecutionModel = _context.ExecutionModel.Where(d => d.id == ActivityModel.execution_id && d.deleted_at == null)
				.Include(d => d.process_version)
				.Include(d => d.user)
				.Include(d => d.activities)
				.ThenInclude(d => d.user_created_by)
				.Include(d => d.activities)
				.ThenInclude(d => d.fields).FirstOrDefault();
			if (ExecutionModel == null)
				return false;
			var data_setting_block = ActivityModel.data_setting;
			var file_template = data_setting_block.file_template;


			//Creates Document instance
			Spire.Doc.Document document = new Spire.Doc.Document();

			//Loads the word document
			document.LoadFromFile("." + file_template.url, Spire.Doc.FileFormat.Docx);
			Section section = document.Sections[0];
			string[] MergeFieldNames = document.MailMerge.GetMergeFieldNames();
			string[] GroupNames = document.MailMerge.GetMergeGroupNames();


			Dictionary<string, string> replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
																  { "id", ExecutionModel.id.ToString()},
																  { "created_at", ExecutionModel.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss")},
																  { "created_at_day", ExecutionModel.created_at.Value.ToString("dd")},
																  { "created_at_month", ExecutionModel.created_at.Value.ToString("MM")},
																  { "created_at_year", ExecutionModel.created_at.Value.ToString("yyyy")},
																  { "created_by_name", ExecutionModel.user.FullName},
																  { "title", ExecutionModel.title},
															};

			Dictionary<string, string> replacements_email = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
																   { "created_by_name", ExecutionModel.user.Email},
															};

			Dictionary<string, string> replacements_file = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { };

			Dictionary<string, Table> replacements_table = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase) { };

			foreach (var activity in ExecutionModel.activities)
			{
				if (activity.variable != null && activity.variable != "")
				{
					if (activity.user_created_by != null)
						replacements.Add("created_by_name_" + activity.variable, activity.user_created_by.FullName);
					if (activity.created_at != null)
					{
						replacements.Add("created_at_" + activity.variable, activity.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss"));
						replacements.Add("created_at_day_" + activity.variable, activity.created_at.Value.ToString("dd"));
						replacements.Add("created_at_month_" + activity.variable, activity.created_at.Value.ToString("MM"));
						replacements.Add("created_at_year_" + activity.variable, activity.created_at.Value.ToString("yyyy"));
					}
				}

				foreach (var field in activity.fields)
				{
					if (!MergeFieldNames.Contains(field.variable))
						continue;
					var data_setting = field.data_setting;
					var values = field.values;
					var text = values.value;
					if (field.type == "select")
					{
						var options = data_setting.options;
						var option = options.Where(d => d.id == values.value).FirstOrDefault();
						text = option.name;
						replacements.Add(field.variable, text);
					}
					else if (field.type == "department")
					{
						var department = _context.DepartmentModel.Where(d => d.id == Int32.Parse(values.value)).FirstOrDefault();
						text = department.name;
						replacements.Add(field.variable, text);
					}
					else if (field.type == "employee")
					{
						var employee = _context.UserModel.Where(d => d.Id == values.value).FirstOrDefault();
						text = employee.FullName;
						replacements.Add(field.variable, text);
						replacements_email.Add(field.variable, employee.Email);
					}
					else if (field.type == "date")
					{
						if (values.value != null)
						{
							var datetime = DateTime.Parse(values.value);
							text = datetime.ToString("yyyy-MM-dd");
							replacements.Add(field.variable, text);
						}

					}
					else if (field.type == "date_month")
					{
						if (values.value != null)
						{
							var datetime = DateTime.Parse(values.value);

							text = datetime.ToString("yyyy-MM");
							replacements.Add(field.variable, text);
						}
					}
					else if (field.type == "date_time")
					{
						if (values.value != null)
						{
							var datetime = DateTime.Parse(values.value);
							text = datetime.ToString("yyyy-MM-dd HH:mm:ss");
							replacements.Add(field.variable, text);
						}
					}
					else if (field.type == "file" || field.type == "file_multiple")
					{
						text = "";

						replacements.Add(field.variable, text);
						//replacements_file.Add(field.variable, String.Join(",", list_file));
					}
					else if (field.type == "select_multiple")
					{
						var options = data_setting.options;
						var option = options.Where(d => values.value_array.Contains(d.id)).Select(d => d.name).ToList();
						text = String.Join(", ", option);
						replacements.Add(field.variable, text);
					}
					else if (field.type == "select_department")
					{
						var options = data_setting.options;
						var option = _context.DepartmentModel.Where(d => values.value_array.Contains(d.id.ToString())).Select(d => d.name).ToList();
						text = String.Join(", ", option);
						replacements.Add(field.variable, text);
					}
					else if (field.type == "select_employee")
					{
						var options = data_setting.options;
						var option = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.FullName).ToList();
						text = String.Join(", ", option);
						replacements.Add(field.variable, text);


						var option_email = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.Email).ToList();
						var text_email = String.Join(", ", option_email);
						replacements_email.Add(field.variable, text_email);
					}
					else if (field.type == "currency")
					{
						if (text == null)
							continue;
						CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
						text = double.Parse(text).ToString("#,###", cul.NumberFormat);
						replacements.Add(field.variable, text);

					}
					else if (field.type == "formular")
					{
						if (text == null)
							continue;
						CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
						text = double.Parse(text).ToString("#,###", cul.NumberFormat);
						replacements.Add(field.variable, text);
					}
					else if (field.type == "yesno")
					{
						if (text == "true")
						{
							text = "√";
						}
					}
					else if (field.type == "table")
					{
						var columns = data_setting.columns;
						var list_data = values.list_data;
						if (GroupNames.Contains(field.variable))
						{
							DataTable dt = new DataTable();
							dt.TableName = field.variable;
							foreach (var column in columns)
							{
								dt.Columns.Add(column.variable, typeof(string));

							}
							foreach (var d in list_data)
							{
								DataRow dr1 = dt.NewRow();
								foreach (var column in columns)
								{

									string value_column = d[column.id] ?? "";
									if (column.type == "currency")
									{
										CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
										value_column = double.Parse(value_column).ToString("#,###", cul.NumberFormat);
									}
									else if (column.type == "formular")
									{
										CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
										value_column = double.Parse(value_column).ToString("#,###", cul.NumberFormat);
									}
									else if (column.type == "yesno")
									{
										if (value_column == "true")
										{
											value_column = "√";
										}
									}
									dr1[column.variable] = value_column;
								}
								dt.Rows.Add(dr1);
							}

							DataSet dsTmp = new DataSet();
							dsTmp.Tables.Add(dt);
							List<DictionaryEntry> list = new List<DictionaryEntry>();
							DictionaryEntry dictionaryEntry = new DictionaryEntry(field.variable, string.Empty);
							list.Add(dictionaryEntry);

							//merge data in list to word table
							document.MailMerge.ExecuteWidthNestedRegion(dsTmp, list);
						}
						else
						{
							text = field.id;

							Table table = section.AddTable(true);
							table.ResetCells(list_data.Count + 1, columns.Count);
							//Set the first row as table header
							TableRow FRow = table.Rows[0];

							FRow.IsHeader = true;
							//Set the height and color of the first row
							FRow.Height = 30;
							var i = 0;
							foreach (var column in columns)
							{
								//Set alignment for cells

								Paragraph p = FRow.Cells[i].AddParagraph();

								FRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
								p.Format.HorizontalAlignment = HorizontalAlignment.Center;
								//Set data format
								TextRange TR = p.AppendText(column.name);

								TR.CharacterFormat.FontName = "Arial";

								TR.CharacterFormat.FontSize = 13;

								TR.CharacterFormat.Bold = true;
								i++;
							}
							//Add data to the rest of rows and set cell format


							for (int r = 0; r < list_data.Count; r++)
							{
								TableRow DataRow = table.Rows[r + 1];

								DataRow.Height = 20;
								var c = 0;
								foreach (var column in columns)
								{


									DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;


									Paragraph p2 = DataRow.Cells[c].AddParagraph();

									string value_column = list_data[r][column.id] ?? "";
									if (column.type == "currency")
									{
										CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
										value_column = double.Parse(value_column).ToString("#,###", cul.NumberFormat);
									}
									else if (column.type == "yesno")
									{
										if (value_column == "true")
										{
											value_column = "√";
										}
									}
									TextRange TR2 = p2.AppendText(value_column);


									p2.Format.HorizontalAlignment = HorizontalAlignment.Center;

									//Set data format

									TR2.CharacterFormat.FontName = "Arial";

									TR2.CharacterFormat.FontSize = 12;

									c++;

								}
							}
							replacements_table.Add(text, table);
							replacements.Add(field.variable, text);
						}

					}
					else
					{
						replacements.Add(field.variable, text);
					}

				}
			}




			string[] fieldName = replacements.Keys.ToArray();
			string[] fieldValue = replacements.Values.ToArray();

			document.MailMerge.Execute(fieldName, fieldValue);
			//document.MailMerge.ExecuteWidthRegion(table)

			foreach (KeyValuePair<string, Table> entry in replacements_table)
			{
				TextSelection selection = document.FindString(entry.Key, true, true);
				if (selection == null)
				{
					var table = entry.Value;
					//table
					continue;
				}
				TextRange range = selection.GetAsOneRange();
				Paragraph paragraph = range.OwnerParagraph;
				Body body = paragraph.OwnerTextBody;
				int index = body.ChildObjects.IndexOf(paragraph);


				body.ChildObjects.Remove(paragraph);
				body.ChildObjects.Insert(index, entry.Value);
			}


			var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

			var newName = timeStamp + ".docx";
			string url = "/private/executions/" + ExecutionModel.id + "/" + newName;
			document.SaveToFile("." + url, Spire.Doc.FileFormat.Docx);

			///Convert PDF
			string libreOfficePath = _configuration["LibreOffice:Path"];
			// FIXME: file name escaping: I have not idea how to do it in .NET.
			ProcessStartInfo procStartInfo = new ProcessStartInfo(libreOfficePath, string.Format("--convert-to pdf --nologo {0} --outdir ./private/executions/" + ExecutionModel.id, "." + url));
			procStartInfo.RedirectStandardOutput = true;
			procStartInfo.UseShellExecute = false;
			procStartInfo.CreateNoWindow = true;
			procStartInfo.WorkingDirectory = Environment.CurrentDirectory;

			Process process = new Process() { StartInfo = procStartInfo, };
			process.Start();
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				ActivityModel.failed = true;
			}
			else
			{
				ActivityModel.executed = true;
			}

			//data_setting_block.file_template.url = "/private/executions/" + ExecutionModel.id + "/" + timeStamp + ".pdf";
			////Tạo Esign
			var files = new List<FileUp>();

			files.Add(new FileUp()
			{
				name = file_template.name,
				url = "/private/executions/" + ExecutionModel.id + "/" + timeStamp + ".pdf",
				ext = ".pdf",
				mimeType = "application/pdf"
			});

			//var process1 = ExecutionModel.process_version.process;
			//var blocks = process1.blocks;
			//var blocks_approve = blocks.Where(d => d.data_setting.blocks_esign_id == blocking.block_id).ToList();

			//var signatures = new List<Signature>();
			//foreach (var block in blocks_approve)
			//{
			//	var Signature = new Signature()
			//	{
			//		block_id = block.id,
			//		status = 1,
			//	};
			//	signatures.Add(Signature);
			//}
			data_setting_block.esign = new Esign();
			//data_setting_block.esign.signatures = signatures;
			data_setting_block.esign.files = files;
			//document.MailMerge.
			//var firstChar = "!#";
			//var lastChar = "#";

			//var to = replacements_email.Aggregate(mail_setting.to, (current, value) =>
			//		current.Replace(firstChar + value.Key + lastChar, value.Value));
			//var title = replacements.Aggregate(mail_setting.title, (current, value) =>
			//		current.Replace(firstChar + value.Key + lastChar, value.Value));
			//var content = replacements.Aggregate(mail_setting.content, (current, value) =>
			//		current.Replace(firstChar + value.Key + lastChar, value.Value));
			//var filecontent = mail_setting.filecontent != null ? replacements_file.Aggregate(mail_setting.filecontent, (current, value) =>
			//		current.Replace(firstChar + value.Key + lastChar, value.Value)) : null;

			//data_setting_block.mail.to = to;
			//data_setting_block.mail.title = title;
			//data_setting_block.mail.content = content;
			//data_setting_block.mail.filecontent = filecontent;

			ActivityModel.data_setting = data_setting_block;
			ActivityModel.blocking = false;
			ActivityModel.created_by = "a76834c7-c4b7-48aa-bf95-05dbd33210ff";
			ActivityModel.created_at = DateTime.Now;


			_context.Update(ActivityModel);
			await _context.SaveChangesAsync();

			create_next(ActivityModel);
			return true;
		}
		public List<UserModel> getListReciever(ActivityModel ActivityModel)
		{
			var list = new List<UserModel>();
			var customBlock = _context.CustomBlockModel.Where(d => d.execution_id == ActivityModel.execution_id && d.block_id == ActivityModel.block_id).FirstOrDefault();
			var data_setting = customBlock.data_setting;
			if (data_setting != null)
			{
				var type_performer = data_setting.type_performer;
				if (type_performer == 4)
				{
					var listuser = data_setting.listuser;
					list = _context.UserModel.Where(d => listuser.Contains(d.Id)).ToList();
				}
				else if (type_performer == 3)
				{
					var listdepartment = data_setting.listdepartment;
					list = _context.UserDepartmentModel.Where(d => listdepartment.Contains(d.department_id)).Include(d => d.user).Select(d => d.user).Distinct().ToList();

				}
			}
			return list;
		}

		public MailSetting fillMail(MailSetting mail, ActivityModel ActivityModel)
		{
			string Domain = (actionAccessor.ActionContext.HttpContext.Request.IsHttps ? "https://" : "http://") + actionAccessor.ActionContext.HttpContext.Request.Host.Value;

			var ExecutionModel = _context.ExecutionModel.Where(d => d.id == ActivityModel.execution_id && d.deleted_at == null)
				.Include(d => d.user)
				.Include(d => d.activities)
				.ThenInclude(d => d.user_created_by)
				.Include(d => d.activities)
				.ThenInclude(d => d.fields).FirstOrDefault();
			if (ExecutionModel == null)
				return mail;
			Dictionary<string, string> replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
																  { "id", ExecutionModel.id.ToString()},
																  { "created_at", ExecutionModel.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss")},
																  { "created_by_name", ExecutionModel.user.FullName},
																  { "title", ExecutionModel.title},
																  { "link", "<a href='" + Domain +"/admin/Execution/details/"+ ExecutionModel.process_version_id + "?execution_id=" + ExecutionModel.id + "'>Link</a>"},
															};

			Dictionary<string, string> replacements_email = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
																   { "created_by_name", ExecutionModel.user.Email},
															};

			Dictionary<string, string> replacements_file = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { };
			if (ActivityModel.clazz == "formTask" || ActivityModel.clazz == "approveTask")
			{
				var list_reciever = getListReciever(ActivityModel);
				var list_FullName = list_reciever.Select(d => d.FullName).ToList();
				var list_Email = list_reciever.Select(d => d.Email).ToList();
				replacements.Add("reciever", String.Join(",", list_FullName));
				replacements_email.Add("reciever", String.Join(",", list_Email));
			}
			foreach (var activity in ExecutionModel.activities)
			{
				if (activity.variable != null && activity.variable != "")
				{
					if (activity.user_created_by != null)
						replacements.Add("created_by_name_" + activity.variable, activity.user_created_by.FullName);
					if (activity.created_at != null)
					{
						replacements.Add("created_at_" + activity.variable, activity.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss"));
					}
				}
				foreach (var field in activity.fields)
				{

					var data_setting = field.data_setting;
					var values = field.values;
					var text = values.value;
					if (field.type == "select")
					{
						var options = data_setting.options;
						var option = options.Where(d => d.id == values.value).FirstOrDefault();
						text = option.name;
						replacements.Add(field.variable, text);
					}
					else if (field.type == "department")
					{
						var department = _context.DepartmentModel.Where(d => d.id == Int32.Parse(values.value)).FirstOrDefault();
						text = department.name;
						replacements.Add(field.variable, text);
					}
					else if (field.type == "employee")
					{
						var employee = _context.UserModel.Where(d => d.Id == values.value).FirstOrDefault();
						text = employee.FullName;
						replacements.Add(field.variable, text);
						replacements_email.Add(field.variable, employee.Email);
					}
					else if (field.type == "date")
					{
						if (values.value != null)
						{
							var datetime = DateTime.Parse(values.value);
							text = datetime.ToString("yyyy-MM-dd");
							replacements.Add(field.variable, text);
						}

					}
					else if (field.type == "date_month")
					{
						if (values.value != null)
						{
							var datetime = DateTime.Parse(values.value);

							text = datetime.ToString("yyyy-MM");
							replacements.Add(field.variable, text);
						}
					}
					else if (field.type == "date_time")
					{
						if (values.value != null)
						{
							var datetime = DateTime.Parse(values.value);
							text = datetime.ToString("yyyy-MM-dd HH:mm:ss");
							replacements.Add(field.variable, text);
						}
					}
					else if (field.type == "file" || field.type == "file_multiple")
					{
						text = "";
						var list_file = new List<string>();
						if (values.files != null)
						{
							foreach (var file in values.files)
							{
								text += "<a href='" + Domain + file.url + "'>" + file.name + "</a>";
								list_file.Add(file.url);
							}
						}


						replacements.Add(field.variable, text);
						replacements_file.Add(field.variable, String.Join(",", list_file));
					}
					else if (field.type == "select_multiple")
					{
						var options = data_setting.options;
						var option = options.Where(d => values.value_array.Contains(d.id)).Select(d => d.name).ToList();
						text = String.Join(", ", option);
						replacements.Add(field.variable, text);
					}
					else if (field.type == "select_department")
					{
						var options = data_setting.options;
						var option = _context.DepartmentModel.Where(d => values.value_array.Contains(d.id.ToString())).Select(d => d.name).ToList();
						text = String.Join(", ", option);
						replacements.Add(field.variable, text);
					}
					else if (field.type == "select_employee")
					{
						var options = data_setting.options;
						var option = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.FullName).ToList();
						text = String.Join(", ", option);
						replacements.Add(field.variable, text);


						var option_email = _context.UserModel.Where(d => values.value_array.Contains(d.Id.ToString())).Select(d => d.Email).ToList();
						var text_email = String.Join(", ", option_email);
						replacements_email.Add(field.variable, text_email);
					}
					else if (field.type == "currency")
					{
						if (text == null)
							continue;
						CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
						text = double.Parse(text).ToString("#,###", cul.NumberFormat);
						replacements.Add(field.variable, text);

					}
					else if (field.type == "table")
					{
						var columns = data_setting.columns;
						var list_data = values.list_data;
						text = $"<table style='width:100%;border:1px solid #eaf0f7;border-collapse:collapse;'><thead style='background:aliceblue;'><tr>";
						foreach (var column in columns)
						{
							text += $"<td style='padding: 10px;border:1px solid white;'>{column.name}</td>";
						}
						text += "</thead></tr>";
						text += "<tbody>";
						foreach (var data in list_data)
						{
							text += "<tr>";
							foreach (var column in columns)
							{
								var value_column = data[column.id] ?? "";


								if (column.type == "currency")
								{
									CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
									value_column = double.Parse(value_column).ToString("#,###", cul.NumberFormat);
								}
								text += $"<td style='padding: 10px;border:1px solid #eaf0f7;'>{value_column}</td>";

							}
							text += "</tr>";
						}
						text += "</tbody>";
						text += "</table>";
						replacements.Add(field.variable, text);
					}
					else
					{
						replacements.Add(field.variable, text);
					}

				}
			}
			var firstChar = "!#";
			var lastChar = "#";
			var to = replacements_email.Aggregate(mail.to, (current, value) =>
					current.Replace(firstChar + value.Key + lastChar, value.Value));
			var title = replacements.Aggregate(mail.title, (current, value) =>
					current.Replace(firstChar + value.Key + lastChar, value.Value));
			var content = replacements.Aggregate(mail.content, (current, value) =>
					current.Replace(firstChar + value.Key + lastChar, value.Value));
			var filecontent = mail.filecontent != null ? replacements_file.Aggregate(mail.filecontent, (current, value) =>
					current.Replace(firstChar + value.Key + lastChar, value.Value)) : null;

			mail.to = to;
			mail.title = title;
			mail.content = content;
			mail.filecontent = filecontent;
			return mail;
		}
	}

}