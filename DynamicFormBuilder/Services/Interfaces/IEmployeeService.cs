using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeModel> GetAll();
        EmployeeModel GetById(string id);
        void Create(EmployeeModel employee);
        public void Update(EmployeeModel employee);
        void Delete(string id); bool IsEmployeeIdExist(string employeeId);
        EmployeeModel GetEmployeeDataByEmployeeId(string employeeId);
        IEnumerable<EmployeeChangeHistoriesModel> GetEmployeeChangeHistoryRecord(string mainTableId);


    }
}

