using DynamicFormBuilder.Data;
using DynamicFormBuilder.Data.DBContext;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DynamicFormBuilder.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDBContext _dbContext;

        public EmployeeService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<EmployeeModel> GetAllEmployees()
        {
            var employees = _dbContext.EmployeeModels.ToList();
            return employees;
        }

     
        public EmployeeModel GetEmployeeById(int id)
        {
            return _dbContext.EmployeeModels.Find(id);
        }

        
        public void AddEmployee(EmployeeModel employee)
        {
            //_dbContext.employees.Add(employee);
            _dbContext.EmployeeModels.Add(employee);
            _dbContext.SaveChanges();
        }

        // Update Employee
        public void UpdateEmployee(EmployeeModel employee)
        {
            _dbContext.EmployeeModels.Update(employee);
            _dbContext.SaveChanges();
        }

        // Delete Employee
        public void DeleteEmployee(int id)
        {
            var employee = _dbContext.EmployeeModels.Find(id);

            if (employee != null)
            {
                //_dbContext.Employees.Remove(employee);
                _dbContext. EmployeeModels.Remove(employee);
                _dbContext.SaveChanges();
            }
        }
    }
}
