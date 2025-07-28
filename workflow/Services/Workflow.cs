using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Org.BouncyCastle.Asn1.X509;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Vue.Data;
using Vue.Models;
using workflow.Areas.V1.Models;
using static Vue.Data.ItContext;

namespace it.Services
{
    public class Workflow
    {
        protected readonly ItContext _context;
        protected readonly EsignContext _esignContext;
        private IActionContextAccessor actionAccessor;

        private readonly IConfiguration _configuration;
        private static readonly string[] ChuSo = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static readonly string[] DonVi = { "", "nghìn", "triệu", "tỷ" };

        public Workflow(IConfiguration configuration, ItContext context, EsignContext esignContext, IActionContextAccessor ActionAccessor)
        {
            _configuration = configuration;
            _context = context;
            _esignContext = esignContext;
            actionAccessor = ActionAccessor;

            var listener = _context.GetService<DiagnosticSource>();
            (listener as DiagnosticListener).SubscribeWithAdapter(new CommandInterceptor());
        }
        public async Task<Boolean> create_next(ActivityModel activity)
        {
            var execution = _context.ExecutionModel.Where(d => d.id == activity.execution_id).Include(d => d.process_version).FirstOrDefault();
            var transitions = _context.TransitionModel.Where(d => d.execution_id == activity.execution_id && d.deleted_at == null).OrderBy(d => d.created_at).ToList();
            var activites = _context.ActivityModel.Where(d => d.execution_id == activity.execution_id && d.deleted_at == null).OrderBy(d => d.created_at).ToList();
            var process_version = execution.process_version;
            var process = process_version.process;
            var blocks = process.blocks;
            var links = process.links;
            var failed = activity.failed ?? false;

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
                return false;

            var outs = getOutEdges(process, node);
            outs = outs.Where(d => d.reverse == failed).ToList();
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
                        stt = transitions[transitions.Count() - 1].stt + 1,
                        id = Guid.NewGuid().ToString(),
                        created_by = activity.created_by,
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
                        if (target.clazz == "formTask" || target.clazz == "approveTask" || target.clazz == "suggestTask")
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
                            failed = failed,
                            blocking = blocking,
                            id = Guid.NewGuid().ToString(),
                            //fields= fields,
                            data_setting = target.data_setting,
                            //created_by= that.current_user.id,
                            started_at = DateTime.Now
                        };
                        if (blocking == false)
                        {
                            activity_new.created_by = activity.created_by;
                            activity_new.created_at = DateTime.Now;
                        }
                        if (target.clazz == "approveTask")
                        {
                            activity_new.esign_id = activity.esign_id;
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
                            if (type_performer == null || (type_performer == 1 && data_setting_block.block_id == null))
                            {
                                data_setting.type_performer = 4;
                                data_setting.listuser = new List<string>() { activity.created_by };
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
                            else if (type_performer == 6)
                            {
                                data_setting.type_performer = 4;
                                string? truongbophan_UserId = activity.created_by;
                                var user1 = _context.UserModel.Where(d => d.deleted_at == null && d.Id == activity.created_by).FirstOrDefault();
                                if (user1 != null)
                                {
                                    var person = _context.PersonnelModel.Where(d => d.EMAIL.ToLower() == user1.Email.ToLower()).FirstOrDefault();
                                    if (person != null)
                                    {
                                        var bophan = _context.PhongModel.SingleOrDefault(d => d.MAPHONG == person.MAPHONG);
                                        if (bophan != null)
                                        {
                                            var truongbophan_id = bophan.truongbophan_id;
                                            var truongbophan = _context.PersonnelModel.SingleOrDefault(d => d.id == truongbophan_id);
                                            var truongbophan_user = _context.UserModel.SingleOrDefault(d => d.Email.ToLower() == truongbophan.EMAIL.ToLower());
                                            if (truongbophan_user != null)
                                            {
                                                truongbophan_UserId = truongbophan_user.Id;
                                            }
                                        }
                                    }
                                }
                                data_setting.listuser = new List<string>() { truongbophan_UserId };
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
                    //// Thực hiện
                    if (activity_new.clazz == "fail")
                    {

                        execution.status_id = (int)ExecutionStatus.Fail;
                        _context.Update(execution);
                        _context.SaveChanges();
                        /////create event
                        //var user = _context.UserModel.Find(user_id);
                        EventModel EventModel = new EventModel
                        {
                            execution_id = activity_new.execution_id,
                            event_content = "Đã thất bại",
                            type = 2,
                            created_at = DateTime.Now,
                        };
                        _context.Add(EventModel);

                        await _context.SaveChangesAsync();
                    }
                    else if (activity_new.clazz == "success")
                    {
                        execution.status_id = (int)ExecutionStatus.Success;
                        _context.Update(execution);
                        _context.SaveChanges();
                        /////create event
                        //var user = _context.UserModel.Find(user_id);
                        EventModel EventModel = new EventModel
                        {
                            execution_id = activity_new.execution_id,
                            event_content = "Đã hoàn thành",
                            created_at = DateTime.Now,
                        };
                        _context.Add(EventModel);

                        await _context.SaveChangesAsync();

                    }
                    else if (activity_new.clazz == "printSystem")
                    {
                        await PrintTask(activity_new);
                    }
                    else if (activity_new.clazz == "outputSystem")
                    {
                        await OutputTask(activity_new);
                    }
                    else if (activity_new.blocking == false)
                    {
                        /////create event
                        var user = _context.UserModel.Find(activity_new.created_by);
                        EventModel EventModel = new EventModel
                        {
                            execution_id = activity_new.execution_id,
                            event_content = "<b>" + user.FullName + "</b> đã thực hiện <b>" + activity_new.label + "</b>",
                            created_at = DateTime.Now,
                        };
                        _context.Add(EventModel);
                        await _context.SaveChangesAsync();
                    }

                    await create_next(activity_new);
                }
            }
            return true;
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

        //public List<ProcessBlockModel>? getNeighbors(ProcessModel process, ProcessBlockModel node, string type)
        //{
        //    //var nodes = process.blocks.Where(d=>d);

        //}

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
        public async Task<Boolean> OutputTask(ActivityModel ActivityModel)
        {
            var ExecutionModel = _context.ExecutionModel.Where(d => d.id == ActivityModel.execution_id && d.deleted_at == null)
               .Include(d => d.process_version)
               .Include(d => d.user)
               .Include(d => d.activities).ThenInclude(d => d.user_created_by)
               .Include(d => d.transitions)
               .Include(d => d.fields)
               .FirstOrDefault();
            if (ExecutionModel == null)
                return false;
            var data_setting_block = ActivityModel.data_setting;
            var user = ActivityModel.user_created_by;
            var type_output = data_setting_block.type_output;
            if (type_output == "esign")
            {

                var field_output = data_setting_block.field_output;
                var field = ExecutionModel.fields.Where(d => d.process_field_id == field_output).LastOrDefault();
                if (field != null)
                {

                    data_setting_block.esign = new Esign();
                    //data_setting_block.esign.signatures = signatures;
                    data_setting_block.esign.files = field.values.files;
                    ActivityModel.data_setting = data_setting_block;

                    _context.Update(ActivityModel);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ///Tim activity trước
                    ///
                    var prev_transitions = ExecutionModel.transitions.Where(d => d.to_activity_id == ActivityModel.id).ToList();

                    foreach (var transition in prev_transitions)
                    {
                        var activity = ExecutionModel.activities.Where(d => d.id == transition.from_activity_id).FirstOrDefault();

                        if (activity != null && activity.clazz == "printSystem")
                        {
                            data_setting_block.esign = activity.data_setting.esign;




                            ///UPLOAD ESIGN
                            //System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                            //var user_id = UserManager.GetUserId(currentUser);
                            //var user = await UserManager.GetUserAsync(currentUser);

                            ///Document
                            var type_id = ActivityModel.data_setting.esign_type_id; /// Type ký điện tử
                            if (type_id == null || type_id == 0)
                            {
                                continue;
                            }
                            var DocumentModel = new DocumentModel()
                            {
                                name_vi = ExecutionModel.title,
                                priority = 2,
                                status_id = (int)DocumentStatus.Release,
                                type_id = type_id,
                                user_id = user.Id,
                                user_next_signature_id = user.Id,
                                is_sign_parellel = false,
                                created_at = DateTime.Now,
                            };
                            var date_now = DateTime.Now;
                            var count_type_in_day = _context.DocumentModel.Where(d => d.type_id == DocumentModel.type_id && d.created_at.Value.DayOfYear == date_now.DayOfYear).Count();
                            var type = _context.DocumentTypeModel.Where(d => d.id == DocumentModel.type_id).Include(d => d.users_receive).FirstOrDefault();
                            DocumentModel.code = type.symbol + date_now.ToString("ddMMyy") + (count_type_in_day < 9 ? "0" : "") + (count_type_in_day + 1);
                            //DocumentModel.code = ExecutionModel.code;
                            _context.Add(DocumentModel);
                            _context.SaveChanges();
                            var esign = data_setting_block.esign;
                            var file = esign.files.FirstOrDefault();
                            ///DocumentFile
                            DocumentFileModel DocumentFileModel = new DocumentFileModel
                            {
                                document_id = DocumentModel.id,
                                ext = ".pdf",
                                url = file.url,
                                name = file.name,
                                mimeType = "application/pdf",
                                created_at = DateTime.Now
                            };
                            _context.Add(DocumentFileModel);
                            //Đính kèm
                            var fields = ExecutionModel.fields.Where(d => d.type == "file_multiple" || d.type == "file").ToList();
                            var list_attachment = new List<DocumentAttachmentModel>();
                            foreach (var f in fields)
                            {
                                if (f.values.files != null && f.values.files.Count > 0)
                                {
                                    foreach (var file_up in f.values.files)
                                    {
                                        list_attachment.Add(new DocumentAttachmentModel()
                                        {
                                            document_id = DocumentModel.id,
                                            name = file_up.name,
                                            ext = file_up.ext,
                                            mimeType = file_up.mimeType,
                                            url = file_up.url,
                                            created_at = DateTime.Now
                                        });
                                    }
                                }
                            }
                            _context.AddRange(list_attachment);

                            ////Signature
                            //var signatures = esign.signatures;
                            var process_version = ExecutionModel.process_version;
                            var process = process_version.process;
                            var nodes = process.blocks.Where(d => d.data_setting.blocks_esign_id == ActivityModel.block_id && d.clazz == "approveTask").OrderBy(d => d.stt).ToList();
                            var stt = 0;
                            foreach (var node in nodes)
                            {
                                var customBlock = _context.CustomBlockModel.Where(d => d.execution_id == ActivityModel.execution_id && d.block_id == node.id).FirstOrDefault();
                                var node_data_setting = node.data_setting;
                                var type_performer = node.type_performer;
                                var listuser = node_data_setting.listuser;
                                if (customBlock != null)
                                {
                                    var custom_data_setting = customBlock.data_setting;
                                    type_performer = custom_data_setting.type_performer;
                                    listuser = custom_data_setting.listuser;
                                }
                                if (type_performer == 1 && node_data_setting.block_id != null)
                                {
                                    var findActivity = ExecutionModel.activities.Where(d => d.block_id == node_data_setting.block_id).FirstOrDefault();
                                    if (findActivity != null)
                                    {
                                        DocumentSignatureModel DocumentSignatureModel = new DocumentSignatureModel() { document_id = DocumentModel.id, user_id = findActivity.created_by, stt = stt++, block_id = node.id };
                                        _context.Add(DocumentSignatureModel);
                                    }
                                }
                                else if (type_performer == 5)
                                {
                                    DocumentSignatureModel DocumentSignatureModel = new DocumentSignatureModel() { document_id = DocumentModel.id, user_id = ExecutionModel.user_id, stt = stt++, block_id = node.id };
                                    _context.Add(DocumentSignatureModel);
                                }
                                else if (type_performer == 4)
                                {
                                    var user_id = listuser.FirstOrDefault();

                                    DocumentSignatureModel DocumentSignatureModel = new DocumentSignatureModel() { document_id = DocumentModel.id, user_id = user_id, stt = stt++, block_id = node.id };
                                    _context.Add(DocumentSignatureModel);

                                }
                                else if (type_performer == 6)
                                {
                                    string? truongbophan_UserId = activity.created_by;
                                    var user1 = _context.UserModel.Where(d => d.deleted_at == null && d.Id == activity.created_by).FirstOrDefault();
                                    if (user1 != null)
                                    {
                                        var person = _context.PersonnelModel.Where(d => d.EMAIL.ToLower() == user1.Email.ToLower()).FirstOrDefault();
                                        if (person != null)
                                        {
                                            var bophan = _context.PhongModel.SingleOrDefault(d => d.MAPHONG == person.MAPHONG);
                                            if (bophan != null)
                                            {
                                                var truongbophan_id = bophan.truongbophan_id;
                                                var truongbophan = _context.PersonnelModel.SingleOrDefault(d => d.id == truongbophan_id);
                                                var truongbophan_user = _context.UserModel.SingleOrDefault(d => d.Email.ToLower() == truongbophan.EMAIL.ToLower());
                                                if (truongbophan_user != null)
                                                {
                                                    truongbophan_UserId = truongbophan_user.Id;
                                                    DocumentSignatureModel DocumentSignatureModel = new DocumentSignatureModel() { document_id = DocumentModel.id, user_id = truongbophan_UserId, stt = stt++, block_id = node.id };
                                                    _context.Add(DocumentSignatureModel);
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            ////Receive
                            if (type.users_receive.Count() > 0)
                            {
                                foreach (var receive in type.users_receive)
                                {
                                    DocumentUserReceiveModel DocumentUserReceiveModel = new DocumentUserReceiveModel() { document_id = DocumentModel.id, user_id = receive.user_id };
                                    _context.Add(DocumentUserReceiveModel);
                                }
                            }
                            /////create event
                            DocumentEventModel DocumentEventModel = new DocumentEventModel
                            {
                                document_id = DocumentModel.id,
                                event_content = "<b>" + user.FullName + "</b> tạo hồ sơ mới",
                                created_at = DateTime.Now,
                            };
                            _context.Add(DocumentEventModel);
                            /////create Related 
                            //RelatedEsignModel RelatedEsignModel = new RelatedEsignModel()
                            //{
                            //    esign_id = DocumentModel.id,
                            //    related_id = data.id,
                            //    type = "dutru",
                            //    created_at = DateTime.Now
                            //};
                            //_context.Add(RelatedEsignModel);

                            ////_context.SaveChanges();
                            //data.status_id = (int)Status.Esign;
                            //data.activeStep = 1;
                            //data.esign_id = DocumentModel.id;
                            //data.code = DocumentModel.code;
                            //_context.Update(data);
                            ActivityModel.esign_id = DocumentModel.id;
                            ActivityModel.data_setting = data_setting_block;

                            _context.Update(ActivityModel);
                            ExecutionModel.esign_id = DocumentModel.id;
                            _context.Update(ExecutionModel);
                            await _context.SaveChangesAsync();
                            //_context.SaveChanges();
                        }
                    }
                }
            }
            /////Event
            EventModel EventModel = new EventModel
            {
                execution_id = ActivityModel.execution_id,
                event_content = "<b>" + user.FullName + "</b> đã thực hiện <b>" + ActivityModel.label + "</b>",
                created_at = DateTime.Now,
            };
            _context.Add(EventModel);
            await _context.SaveChangesAsync();
            return true;
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
            var type_template = data_setting_block.type_template;
            var type_output = data_setting_block.type_output;
            if (type_template == "html")
            {
                var type_template_html = data_setting_block.type_template_html;
                if (type_template_html == "nghiphep")
                {
                    var model = new Nghiphep()
                    {
                        user = ExecutionModel.user,
                        user_id = ExecutionModel.user_id
                    };
                    Type t = typeof(Nghiphep);

                    //var listname = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite).Select(d => d.Name).ToList();
                    var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

                    foreach (var activity in ExecutionModel.activities)
                    {
                        foreach (var field in activity.fields)
                        {
                            var values = field.values;
                            var text = values.value;
                            if (field.variable == "tongngaynghi")
                            {
                                model.tongngaynghi = Convert.ToDouble(text);
                            }
                        }
                    }
                    Console.WriteLine(model);
                }
            }
            else
            {
                //Creates Document instance
                Spire.Doc.Document document = new Spire.Doc.Document();
                var dir = _configuration["Source:Path_Private"].Replace("\\private", "").Replace("\\", "/");

                //Loads the word document
                document.LoadFromFile(dir + file_template.url, Spire.Doc.FileFormat.Docx);
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
                                                                  { "created_by_msnv", ExecutionModel.user.msnv},
                                                                  { "created_by_phone", ExecutionModel.user.PhoneNumber},
                                                                  { "created_by_department_text", ExecutionModel.user.department_text},
                                                                  { "title", ExecutionModel.title},
                                                            };


                Dictionary<string, string> replacements_email = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                                                                   { "created_by_name", ExecutionModel.user.Email},
                                                            };

                Dictionary<string, string> replacements_file = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { };

                Dictionary<string, Table> replacements_table = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase) { };

                var activities = ExecutionModel.activities.Where(d => d.deleted_at == null).ToList();
                foreach (var activity in activities)
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
                        if (!MergeFieldNames.Contains(field.variable) && field.type != "table" && field.type != "yesno")
                            continue;
                        var data_setting = field.data_setting;
                        var values = field.values;
                        var text = values.value;
                        if (field.type == "select" || field.type == "radio")
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
                        else if (field.type == "select_multiple" || field.type == "checkbox")
                        {
                            var options = data_setting.options;
                            var option = options.Where(d => values.value_array.Contains(d.id)).Select(d => d.name).ToList();
                            text = String.Join(", ", option);
                            replacements.Add(field.variable, text);
                        }
                        else if (field.type == "department_multiple")
                        {
                            var options = data_setting.options;
                            var option = _context.DepartmentModel.Where(d => values.value_array.Contains(d.id.ToString())).Select(d => d.name).ToList();
                            text = String.Join(", ", option);
                            replacements.Add(field.variable, text);
                        }
                        else if (field.type == "employee_multiple")
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
                            var text_string = DocTienBangChu(decimal.Parse(text));
                            var text_number = double.Parse(text).ToString("#,###.##", cul.NumberFormat);

                            replacements.Add(field.variable, text_number);
                            replacements.Add(field.variable + "_vn", text_string);
                        }
                        else if (field.type == "formular")
                        {
                            if (text == null)
                                continue;
                            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                            var text_string = DocTienBangChu(decimal.Parse(text));
                            var text_number = double.Parse(text).ToString("#,###.##", cul.NumberFormat);

                            replacements.Add(field.variable, text_number);
                            replacements.Add(field.variable + "_vn", text_string);
                        }
                        else if (field.type == "yesno")
                        {
                            var text_yes = "☐";
                            var text_no = "☐";
                            if (text == "true")
                            {
                                text = "√";
                                text_yes = "☒";
                            }
                            else
                            {
                                text_no = "☒";
                            }
                            replacements.Add(field.variable + "_yes", text_yes);
                            replacements.Add(field.variable + "_no", text_no);
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

                                        string value_column = d.ContainsKey(column.id) ? d[column.id] : "";
                                        if (column.type == "currency")
                                        {
                                            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                                            value_column = double.Parse(value_column).ToString("#,###.##", cul.NumberFormat);
                                        }
                                        else if (column.type == "formular")
                                        {
                                            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                                            value_column = double.Parse(value_column).ToString("#,###.##", cul.NumberFormat);
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
                            else if (MergeFieldNames.Contains(field.variable))
                            {
                                text = field.id;

                                Table table = section.AddTable(true);
                                //table.Width = 100;
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

                                    //TR.CharacterFormat.FontSize = 13;

                                    TR.CharacterFormat.Bold = true;
                                    i++;
                                }
                                //Add data to the rest of rows and set cell format


                                for (int r = 0; r < list_data.Count; r++)
                                {
                                    var data = list_data[r];
                                    TableRow DataRow = table.Rows[r + 1];

                                    DataRow.Height = 20;
                                    var c = 0;
                                    foreach (var column in columns)
                                    {


                                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;


                                        Paragraph p2 = DataRow.Cells[c].AddParagraph();

                                        string value_column = data.ContainsKey(column.id) ? data[column.id] : "";
                                        if (column.type == "currency")
                                        {
                                            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                                            value_column = double.Parse(value_column).ToString("#,###.##", cul.NumberFormat);
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

                                        //TR2.CharacterFormat.FontSize = 12;

                                        c++;

                                    }
                                }
                                replacements_table.Add(text, table);
                                replacements.Add(field.variable, text);
                            }
                            else
                            {
                                for (int r = 0; r < list_data.Count; r++)
                                {
                                    var data = list_data[r];

                                    foreach (var column in columns)
                                    {
                                        if (column.variable == null)
                                            continue;
                                        string value_column = data.ContainsKey(column.id) ? data[column.id] : "";
                                        if (column.type == "currency")
                                        {
                                            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                                            value_column = double.Parse(value_column).ToString("#,###.##", cul.NumberFormat);
                                        }
                                        else if (column.type == "yesno")
                                        {
                                            if (value_column == "true")
                                            {
                                                value_column = "√";
                                            }
                                        }
                                        var variable_child = $"{field.variable}_{column.variable}_{r}";
                                        replacements.Add(variable_child, value_column);
                                    }
                                }
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
                    //var fontsize = paragrap

                    body.ChildObjects.Remove(paragraph);
                    body.ChildObjects.Insert(index, entry.Value);
                }

                //Section section = document.Sections[0];
                foreach (Table table in section.Tables)
                {
                    table.AutoFit(AutoFitBehaviorType.AutoFitToContents);
                }
                var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                var newName = timeStamp + ".docx";
                string url = "/private/executions/" + ExecutionModel.id + "/" + newName;
                //var countpage = document.GetPageCount();
                var status_process = true;


                document.SaveToFile(dir + url, Spire.Doc.FileFormat.Docx);


                if (type_output == "word")
                {


                    var files = new List<FileUp>();

                    files.Add(new FileUp()
                    {
                        name = file_template.name,
                        url = url,
                        ext = ".docx",
                        mimeType = "application/word"
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
                }
                else
                {
                    ///Convert PDF
                    var file = dir + url;
                    file = file.Replace("/", "\\");
                    var output = dir + "/private/executions/" + ExecutionModel.id;
                    output = output.Replace("/", "\\");
                    status_process = ConvertWordFile(file, output);



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
                }

                ActivityModel.failed = !status_process;
            }




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
            ActivityModel.executed = true;
            //ActivityModel.created_by = activity.created_by;
            ActivityModel.created_at = DateTime.Now;

            _context.Update(ActivityModel);
            /////Event
            var user = _context.UserModel.Find(ActivityModel.created_by);
            EventModel EventModel = new EventModel
            {
                execution_id = ActivityModel.execution_id,
                event_content = "<b>" + user.FullName + "</b> đã thực hiện <b>" + ActivityModel.label + "</b>",
                created_at = DateTime.Now,
            };
            _context.Add(EventModel);
            await _context.SaveChangesAsync();

            await create_next(ActivityModel);
            return true;

        }
        private bool ConvertWordFile(string file, string outputDirectory)
        {
            string libreOfficePath = _configuration["LibreOffice:Path"];
            //// FIXME: file name escaping: I have not idea how to do it in .NET.
            //Console.WriteLine(string.Format("-env:UserInstallation=file:///C:/temp/libreoffice --convert-to pdf --nologo " + file + " --outdir " + outputDirectory));
            ProcessStartInfo procStartInfo = new ProcessStartInfo(libreOfficePath, string.Format("-env:UserInstallation=file:///C:/temp/libreoffice --convert-to pdf --nologo \"" + file + "\" --outdir \"" + outputDirectory + "\""));
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.WorkingDirectory = Environment.CurrentDirectory;

            Process process = new Process() { StartInfo = procStartInfo, };
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
                return false;
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
                                                                  { "link", "<a href='" + Domain +"/Execution/details/"+ ExecutionModel.process_version_id + "?execution_id=" + ExecutionModel.id + "'>Link</a>"},
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
            var activities = ExecutionModel.activities.Where(d => d.deleted_at == null).ToList();
            foreach (var activity in activities)
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
                if (activity.clazz == "outputSystem" || activity.clazz == "printSystem")
                {

                    var text1 = "";
                    var files = activity.data_setting.esign.files;

                    var list_file = new List<string>();
                    if (files != null)
                    {
                        var esign = files.LastOrDefault();
                        text1 += "<a href='" + Domain + esign.url + "'>" + esign.name + "</a>";
                        list_file.Add(esign.url);
                    }


                    replacements.Add("file_" + activity.variable, text1);
                    replacements_file.Add("file_" + activity.variable, String.Join(",", list_file));
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
                    else if (field.type == "department_multiple")
                    {
                        var options = data_setting.options;
                        var option = _context.DepartmentModel.Where(d => values.value_array.Contains(d.id.ToString())).Select(d => d.name).ToList();
                        text = String.Join(", ", option);
                        replacements.Add(field.variable, text);
                    }
                    else if (field.type == "employee_multiple")
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
                        var text_string = DocTienBangChu(decimal.Parse(text));
                        var text_number = double.Parse(text).ToString("#,###.##", cul.NumberFormat);

                        replacements.Add(field.variable, text_number);
                        replacements.Add(field.variable + "_vn", text_string);

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
                                var value_column = data.ContainsKey(column.id) ? data[column.id] : "";


                                if (column.type == "currency")
                                {
                                    CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
                                    value_column = double.Parse(value_column).ToString("#,###.##", cul.NumberFormat);
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

        public static string DocTienBangChu(decimal number)
        {
            if (number == 0) return "Không đồng";

            string s = "";
            string strSo = ((long)number).ToString();
            int i = 0;

            while (strSo.Length > 0)
            {
                string donvi = DonVi[i];
                string so3 = strSo.Length >= 3 ? strSo.Substring(strSo.Length - 3, 3) : strSo;
                strSo = strSo.Length >= 3 ? strSo.Substring(0, strSo.Length - 3) : "";

                string chu = Doc3So(so3);
                if (!string.IsNullOrEmpty(chu))
                    s = chu + " " + donvi + " " + s;

                i++;
            }

            s = s.Trim();
            s = char.ToUpper(s[0]) + s.Substring(1) + " đồng chẵn.";
            return s;
        }

        private static string Doc3So(string so)
        {
            while (so.Length < 3)
                so = "0" + so;

            int tram = int.Parse(so[0].ToString());
            int chuc = int.Parse(so[1].ToString());
            int donvi = int.Parse(so[2].ToString());

            string s = "";

            if (tram != 0)
                s += ChuSo[tram] + " trăm";
            else if (chuc != 0 || donvi != 0)
                s += "không trăm";

            if (chuc != 0)
            {
                if (chuc == 1)
                    s += " mười";
                else
                    s += " " + ChuSo[chuc] + " mươi";
            }
            else if (donvi != 0)
            {
                s += " linh";
            }

            if (donvi != 0)
            {
                if (chuc != 0 && donvi == 1)
                    s += " mốt";
                else if (chuc != 0 && donvi == 5)
                    s += " lăm";
                else
                    s += " " + ChuSo[donvi];
            }

            return s.Trim();
        }

    }
}
