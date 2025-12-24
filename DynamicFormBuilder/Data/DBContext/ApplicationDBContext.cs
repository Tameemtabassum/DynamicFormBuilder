using DynamicFormBuilder.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.Data.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        // This constructor is required for DI
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<StudentModel> StudentModels { get; set; }
        public DbSet<EmployeeModel> EmployeeModels { get; set; }
        public DbSet<CustomerModel> CustomerModels { get; set; }
        public DbSet<DivisionModel> DivisionModels { get; set; }
        public DbSet<DistrictModel> DistrictModels { get; set; }
        public DbSet<ParticipantsModel> ParticipantsModels { get; set; }



    }
}
