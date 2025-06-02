
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
  public interface ICustomerRepository
  {

    public Task<List<Customer>> GetAllCustomer();

    public Customer GetCustomer(int id);
    public IQueryable<Customer> GetAllCustomerExport();

    public List<Customer> GetCustomersWithCompletedOrderByDate(DateTime fromdate);

  }
}