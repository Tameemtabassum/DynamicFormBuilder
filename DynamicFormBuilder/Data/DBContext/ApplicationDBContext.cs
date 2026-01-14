using DynamicFormBuilder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.Data.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       
        public DbSet<StudentModel> StudentModels { get; set; }
        public DbSet<CustomerModel> CustomerModels { get; set; }
        public DbSet<DivisionModel> DivisionModels { get; set; }

        public DbSet<DistrictModel> DistrictModels { get; set; }
        public DbSet<ParticipantsModel> ParticipantsModels { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<EmployeeChangeHistoriesModel> EmployeeChangeHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmployeeChangeHistoriesModel>().ToTable("employeechangehistory");
        }
    }
}
