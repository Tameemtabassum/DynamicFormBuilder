using DynamicFormBuilder.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.Data.DBContext
{
    public class ApplicationDbContext : DbContext
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
    }
}
