using DynamicFormBuilder.Data.DBContext;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DynamicFormBuilder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the DbContext via constructor
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            
            var vm = new DashboardViewModel
            {
               
                StudentsCount = _context.StudentModels.Count(),
                CustomersCount = _context.CustomerModels.Count(),
                ParticipantsCount = _context.ParticipantsModels.Count(),
                EmployeesCount = _context.Employees.Count(),

                RecentActivities = new List<string>
                {
                    "Dashboard initialized"
                }
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
