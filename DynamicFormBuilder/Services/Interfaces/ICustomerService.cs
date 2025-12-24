using DynamicFormBuilder.Models;
using DynamicFormBuilder.ViewModels;

namespace DynamicFormBuilder.Services.Interfaces
{
    public interface ICustomerService
    {
        CustomerModel GetCustomerById(int id);
        CustomerViewModel GetCustomerDetailsById(int id);
        List<CustomerViewModel> GetAllCustomers();
        void AddCustomer(CustomerModel customer);
        void UpdateCustomer(CustomerModel customer);
        void DeleteCustomer(int id);
        IEnumerable<CustomerViewModel> GetAllSearchCustomer(string phone, int? divisionId);

        List<DivisionModel> GetAllDivision();
        List<DistrictModel> GetAllDistrict();
        List<DistrictModel> GetDistrictByDivisionId(int divisionId);
        void AddBalance(int customerId, decimal amount);

    }
}
