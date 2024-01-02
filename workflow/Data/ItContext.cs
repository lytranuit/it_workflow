
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using workflow.Areas.V1.Models;
using Vue.Models;
using workflow.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DiagnosticAdapter;
using System.Data.Common;

namespace Vue.Data
{
    public class ItContext : DbContext
    {
        private IActionContextAccessor actionAccessor;
        private UserManager<UserModel> UserManager;
        public ItContext(DbContextOptions<ItContext> options, UserManager<UserModel> UserMgr, IActionContextAccessor ActionAccessor) : base(options)
        {
            actionAccessor = ActionAccessor;
            UserManager = UserMgr;

        }
        public DbSet<ProcessGroupModel> ProcessGroupModel { get; set; }
        public DbSet<ProcessModel> ProcessModel { get; set; }
        public DbSet<ProcessBlockModel> ProcessBlockModel { get; set; }
        public DbSet<ProcessLinkModel> ProcessLinkModel { get; set; }
        public DbSet<ProcessFieldModel> ProcessFieldModel { get; set; }
        public DbSet<ExecutionModel> ExecutionModel { get; set; }
        public DbSet<ExecutionFieldModel> ExecutionFieldModel { get; set; }
        public DbSet<ActivityModel> ActivityModel { get; set; }
        public DbSet<CustomBlockModel> CustomBlockModel { get; set; }
        public DbSet<TransitionModel> TransitionModel { get; set; }
        public DbSet<ProcessVersionModel> ProcessVersionModel { get; set; }

        public DbSet<CommentModel> CommentModel { get; set; }
        public DbSet<CommentFileModel> CommentFileModel { get; set; }
        public DbSet<EventModel> EventModel { get; set; }
        public DbSet<UserReadModel> UserReadModel { get; set; }
        public DbSet<UserUnreadModel> UserUnreadModel { get; set; }
        public DbSet<AuditTrailsModel> AuditTrailsModel { get; set; }

        public DbSet<UserModel> UserModel { get; set; }
        public DbSet<UserRoleModel> UserRoleModel { get; set; }

        //public DbSet<User2Model> User2Model { get; set; }
        public DbSet<EmailModel> EmailModel { get; set; }
        public DbSet<TokenModel> TokenModel { get; set; }
        public DbSet<DepartmentModel> DepartmentModel { get; set; }
        public DbSet<UserDepartmentModel> UserDepartmentModel { get; set; }

        public DbSet<LibraryPermissionModel> LibraryPermissionModel { get; set; }
        public DbSet<LibraryModel> LibraryModel { get; set; }
        public DbSet<FilterIdRaw> FilterIdRaw { get; set; }
        public DbSet<SizeRaw> SizeRaw { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRoleModel>().ToTable("AspNetUserRoles").HasKey(table => new
            {
                table.RoleId,
                table.UserId
            });

        }
        public override int SaveChanges()
        {
            OnBeforeSaveChanges();
            return base.SaveChanges();
        }
        private void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            var user_http = actionAccessor.ActionContext.HttpContext.User;
            var user_id = UserManager.GetUserId(user_http);
            var changes = ChangeTracker.Entries();
            foreach (var entry in changes)
            {
                if (entry.Entity is AuditTrailsModel || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = user_id;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                var Original = entry.GetDatabaseValues().GetValue<object>(propertyName);
                                var Current = property.CurrentValue;
                                if (JsonConvert.SerializeObject(Original) == JsonConvert.SerializeObject(Current))
                                    continue;
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = Original;
                                auditEntry.NewValues[propertyName] = Current;

                            }
                            break;
                    }

                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditTrailsModel.Add(auditEntry.ToAudit());
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
        }
        public List<int> ChildFolder(List<int>? parents)
        {
            try
            {
                if (parents == null)
                    return new List<int>();

                var selectList = string.Join(", ", parents);

                string query = $@"WITH cte AS(SELECT a.ItemID, a.ParentID, a.Name FROM Library a WHERE ItemID IN({selectList}) UNION ALL SELECT a.ItemID, a.ParentID, a.Name FROM Library a JOIN cte c ON a.ParentID = c.ItemID where a.Type = 'Folder') 
								SELECT  ItemID, name FROM cte";

                var folder = FilterIdRaw.FromSqlRaw(query).AsEnumerable().Select(d => (int)d.ItemId).ToList();
                return folder;
            }
            catch (Exception e)
            {
                return new List<int>();
            }
        }
        public class CommandInterceptor
        {
            [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting")]
            public void OnCommandExecuting(DbCommand command, DbCommandMethod executeMethod, Guid commandId, Guid connectionId, bool async, DateTimeOffset startTime)
            {
                var secondaryDatabaseName = "OrgData";
                var schemaName = "dbo";
                var list_talbe = new List<string>()
            {
                "AspNetUsers",
                    "AspNetUserRoles",
                    "emails",
                    "Token",
                    "department",
                    "user_department"
            };
                //var tableName = "AspNetUsers";
                foreach (var tableName in list_talbe)
                {
                    command.CommandText = command.CommandText.Replace($" [{tableName}]", $" [{schemaName}].[{tableName}]")
                                                         .Replace($" [{schemaName}].[{tableName}]", $" [{secondaryDatabaseName}].[{schemaName}].[{tableName}]");
                }


            }
        }
    }
    public class FilterIdRaw
    {
        [Key]
        public int ItemId { get; set; }
        public string Name { get; set; }
    }
    public class SizeRaw
    {
        [Key]
        public long Size { get; set; }
    }
}
