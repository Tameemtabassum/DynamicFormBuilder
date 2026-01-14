using DynamicFormBuilder.Models;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeModel> GetAll();
        EmployeeModel GetById(string id);
        void Create(EmployeeModel employee);
        public EmployeeModel Update(EmployeeModel employee);
        void Delete(string id); bool IsEmployeeIdExist(string employeeId);
        EmployeeModel GetEmployeeDataByEmployeeId(string employeeId);
      
        IEnumerable<EmployeeChangeHistoriesViewModel> GetEmployeeChangeHistoryRecord(string employeeId);

    }
}

