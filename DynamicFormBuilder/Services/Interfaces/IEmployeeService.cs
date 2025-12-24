using DynamicFormBuilder.Models;
using System.Collections.Generic;

namespace DynamicFormBuilder.Services
{
    public interface IEmployeeService
    {
        List<EmployeeModel> GetAllEmployees();
        EmployeeModel GetEmployeeById(int id);
        void AddEmployee(EmployeeModel employee);
        void UpdateEmployee(EmployeeModel employee);
        void DeleteEmployee(int id);
    }
}
