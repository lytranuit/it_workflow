
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using workflow.Areas.V1.Models;
using Vue.Models;
using workflow.Models;
using System.ComponentModel.DataAnnotations;

namespace Vue.Data
{
    public class EsignContext : DbContext
    {
        private IActionContextAccessor actionAccessor;
        private UserManager<UserModel> UserManager;
        public EsignContext(DbContextOptions<EsignContext> options) : base(options)
        {

        }

        public DbSet<UserEsignModel> UserEsignModel { get; set; }




        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
        }

    }

}
