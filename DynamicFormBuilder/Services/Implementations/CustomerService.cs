using DynamicFormBuilder.Data.DBContext;
using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Interfaces;
using DynamicFormBuilder.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public CustomerModel GetCustomerById(int id)
        {
            var customers = _dbContext.CustomerModels.Find(id);
            return customers;
        }
        public CustomerViewModel GetCustomerDetailsById(int id)
        {
            var customer =
        (from c in _dbContext.CustomerModels.Where(i => i.CustomerID == id)

             // LEFT JOIN Division
         join d in _dbContext.DivisionModels
             on c.DivisionID equals d.DivisionID into divJoin
         from d in divJoin.DefaultIfEmpty()

             // LEFT JOIN District (correct join!)
         join dis in _dbContext.DistrictModels
             on c.DistrictID equals dis.DistrictID into disJoin
         from dis in disJoin.DefaultIfEmpty()

         select new CustomerViewModel
         {
             CustomerID = c.CustomerID,
             FullName = c.FirstName + " " + c.LastName,
             DivisionName = d.DivisionName,
             DistrictName = dis.DistrictName,
             DOB = c.DOB,
             DivisionID = c.DivisionID,
             DistrictID = c.DistrictID,
             Phone = c.Phone,
             Profession = c.Profession,
             Email = c.Email,
             NID = c.NID,
             Balance = c.Balance
         })
         .FirstOrDefault();

            return customer;
        }


        public List<CustomerViewModel> GetAllCustomers()
        {
            var customers =
                (from c in _dbContext.CustomerModels
                     // LEFT JOIN Division
                 join d in _dbContext.DivisionModels
                     on c.DivisionID equals d.DivisionID into divJoin
                 from d in divJoin.DefaultIfEmpty()

                     // LEFT JOIN District (correct join!)
                 join dis in _dbContext.DistrictModels
                     on c.DistrictID equals dis.DistrictID into disJoin
                 from dis in disJoin.DefaultIfEmpty()
                 select new CustomerViewModel
                 {
                     CustomerID = c.CustomerID,
                     FullName = c.FirstName + " " + c.LastName,
                     DivisionName = d.DivisionName,
                     DistrictName = dis.DistrictName,
                     DOB = c.DOB,
                     DivisionID = c.DivisionID,
                     DistrictID = c.DistrictID,
                     Phone = c.Phone,
                     Profession = c.Profession,
                     Email= c.Email,
                     NID = c.NID,
                     Balance= c.Balance,

                    
                 })
                .ToList();

            return customers;
        }
        public IEnumerable<CustomerViewModel> GetAllSearchCustomer(string phone, int? divisionId)
        {
            var customers = GetAllCustomers() ?? new List<CustomerViewModel>();

            if (!string.IsNullOrWhiteSpace(phone))
            {
                customers = customers
                    .Where(c => !string.IsNullOrEmpty(c.Phone) && c.Phone.Contains(phone))
                    .ToList();
            }

            if (divisionId.HasValue)
            {
                customers = customers
                    .Where(c => c.DivisionID == divisionId.Value)
                    .ToList();
            }

            return customers;
        }



        public void AddCustomer(CustomerModel customer)
        {
            _dbContext.CustomerModels.Add(customer);
            _dbContext.SaveChanges();
        }

        public void UpdateCustomer(CustomerModel customer)
        {
            _dbContext.CustomerModels.Update(customer);
            _dbContext.SaveChanges();
        }
        public void DeleteCustomer(int id)
        {
            var customer = new CustomerModel { CustomerID = id };
            _dbContext.CustomerModels.Remove(customer);
            _dbContext.SaveChanges();
        }

        public List<DivisionModel> GetAllDivision()
        {
            var divisions = _dbContext.DivisionModels.ToList();
            return divisions;
        }
        public List<DistrictModel> GetAllDistrict()
        {
            var district = _dbContext.DistrictModels.ToList();
            return district;
        }

        public List<DistrictModel> GetDistrictByDivisionId(int divisionId)
        {
            return _dbContext.DistrictModels
                             .Where(d => d.DivisionID == divisionId)
                             .ToList();
        }

        public void AddBalance(int customerId, decimal amount)
        {
            var customer = _dbContext.CustomerModels.FirstOrDefault(c => c.CustomerID == customerId);

            if (customer != null)
            {
                customer.Balance += amount;
                _dbContext.SaveChanges();
            }

        }

    }
}
