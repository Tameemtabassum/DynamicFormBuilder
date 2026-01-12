using DynamicFormBuilder.Services.Implementations;
using DynamicFormBuilder.Services.Interfaces;

namespace DynamicFormBuilder.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IParticipantsService, ParticipantsService>();
            services.AddScoped<IEmployeeService, EmployeeService>();





            return services;

        }
    }
}
