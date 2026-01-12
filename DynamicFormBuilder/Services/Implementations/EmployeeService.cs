using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DynamicFormBuilder.Data.DBContext;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DynamicFormBuilder.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;



        public EmployeeService(ApplicationDbContext context

        )
        {
            _context = context;


        }
       
        public EmployeeModel GetById(string id)
        {
            // Fetch employee by integer Id
            var employee = _context.Employees.FirstOrDefault(x => x.Id == id);

            if (employee == null)
                return null;

            
            return new EmployeeModel
            {
                Id = employee.Id,                  // int type
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Email = employee.Email,
                DOB = employee.DOB,
                Designation = employee.Designation,
                IsActive = employee.IsActive
               
            };
        }


        public EmployeeModel GetEmployeeDataByEmployeeId(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return null;

            var employee = _context.Employees.FirstOrDefault(x => x.EmployeeId == employeeId);
            if (employee == null)
                return null;

            
            return employee;


        }

        public IEnumerable<EmployeeModel> GetAll()
        {
            return _context.Employees.ToList();
        }

       

        public void Create(EmployeeModel employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges(); 
        }

        public void Update(EmployeeModel employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            var existing = _context.Employees
                .FirstOrDefault(e => e.Id == employee.Id);

            if (existing == null)
                throw new Exception("Employee not found");

            existing.FullName = employee.FullName;
            existing.EmployeeId = employee.EmployeeId;
            existing.Email = employee.Email;
            existing.DOB = employee.DOB;
            existing.Designation = employee.Designation;
            existing.IsActive = employee.IsActive;

            _context.SaveChanges();
        }

     

        public EmployeeModel UpdateStatus(EmployeeModel employee)
        {
            var existing = _context.Employees.FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);
            if (existing == null)
                throw new Exception("Employee not found");

            existing.FullName = existing.FullName;
            existing.Email = existing.Email;
            existing.Designation = existing.Designation;
            existing.IsActive = employee.IsActive;
            existing.DOB = existing.DOB;
            _context.Employees.Update(existing);
            _context.SaveChanges();

            return employee; // return the updated model
        }

        public void Delete(string id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                throw new Exception("Employee not found");

            _context.Employees.Remove(employee);
            _context.SaveChanges(); // Synchronous
        }

        public IEnumerable<EmployeeChangeHistoriesModel> GetEmployeeChangeHistoryRecord(string mainTableId)
        {
            try
            {
                return _context.EmployeeChangeHistories
                               .Where(x => x.EmployeeId == mainTableId)
                               .OrderByDescending(x => x.ChangedAt)
                               .Select(x => new EmployeeChangeHistoriesModel
                               {
                                   EmployeeId = x.EmployeeId,
                                   PreviousData = x.PreviousData,
                                   UpdatedData = x.UpdatedData,
                                   ChangedAt = x.ChangedAt
                               })
                               .ToList();
            }
            catch
            {
                return new List<EmployeeChangeHistoriesModel>();
            }
        }





        public bool IsEmployeeIdExist(string employeeId)
        {
            try
            {
                var result = _context.Employees.Where(e => e.EmployeeId == employeeId).Select(e => e.Id);
                return result.Any();

            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }
}


  
