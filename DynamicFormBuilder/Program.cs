using DynamicFormBuilder.Data;
using DynamicFormBuilder.Data.DBContext;
using DynamicFormBuilder.Services;
using DynamicFormBuilder.Services.Implementations;
using DynamicFormBuilder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddControllersWithViews();


// Register services
builder.Services.RegisterApplicationServices();
//builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<FormRepository>();
//builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IParticipantsService, ParticipantsService>();



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}");

app.Run();
