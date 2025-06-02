using PizzaShop.repository.Interfaces;
using Pizzashop.entity.ViewModels;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }




        public async Task<AddPaginationViewmodel<CustomerDetailViewModel>> GetAllCutomer(int page, int pageSize, string search, string time, string SortbyName, string SortbyDate)
        {
            var customers = await _repository.GetAllCustomer();
            int tableCount;



            var customerViewModels = customers.Select(c => new CustomerDetailViewModel
            {
                CustomerId = c.CustomerId,
                CustomerEmail = c.Email,
                CustomerName = c.Name,
                CustomerPhone = c.Phone,
                TotalOrder = c.Orders.Count(),
                Date = c.CreatedAt
            }).ToList();


            if (!string.IsNullOrEmpty(search))
            {
                customerViewModels = customerViewModels.Where(u => u.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            customerViewModels = (SortbyName, SortbyDate) switch
            {
                ("asc_name", "asc_name") => customerViewModels.OrderBy(u => u.CustomerName).ThenBy(u => u.Date).ToList(),
                ("asc_name", "dec_name") => customerViewModels.OrderBy(u => u.CustomerName).ThenByDescending(u => u.Date).ToList(),
                ("dec_name", "asc_name") => customerViewModels.OrderByDescending(u => u.CustomerName).ThenBy(u => u.Date).ToList(),
                ("dec_name", "dec_name") => customerViewModels.OrderByDescending(u => u.CustomerName).ThenByDescending(u => u.Date).ToList(),
                ("asc_name", _) => customerViewModels.OrderBy(u => u.CustomerName).ToList(),
                ("dec_name", _) => customerViewModels.OrderByDescending(u => u.CustomerName).ToList(),
                (_, "asc_name") => customerViewModels.OrderBy(u => u.Date).ToList(),
                (_, "dec_name") => customerViewModels.OrderByDescending(u => u.Date).ToList(),
                _ => customerViewModels.OrderBy(u => u.CustomerName).ToList()
            };

            if (!string.IsNullOrEmpty(time) && time != "all")
            {
                DateTime now = DateTime.Now;

                if (time == "7")
                {
                    var last7Days = now.AddDays(-7);
                    customerViewModels = customerViewModels.Where(o => o.Date >= last7Days).ToList();
                }
                else if (time == "30")
                {
                    var last30Days = now.AddDays(-30);
                    customerViewModels = customerViewModels.Where(o => o.Date >= last30Days).ToList();
                }
                else if (time == "month")
                {
                    var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    customerViewModels = customerViewModels.Where(o => o.Date >= startOfMonth).ToList();
                }
            }
            tableCount = customerViewModels.Count;

            customerViewModels = customerViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var order = new AddPaginationViewmodel<CustomerDetailViewModel>
            {
                Items = customerViewModels,
                TotalCount = tableCount,
                CurrentPage = page,
                PageSize = pageSize,
            };

            return order;
        }


        public ExportOrdersResult GetExportCustomer(string search, string time)
        {
            var query = _repository.GetAllCustomerExport();


            if (!string.IsNullOrEmpty(time) && time != "all")
            {
                DateTime now = DateTime.Now;

                if (time == "7")
                {
                    var last7Days = now.AddDays(-7);
                    query = query.Where(o => o.CreatedAt >= last7Days);
                }
                else if (time == "30")
                {
                    var last30Days = now.AddDays(-30);
                    query = query.Where(o => o.CreatedAt >= last30Days);
                }
                else if (time == "month")
                {
                    var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    query = query.Where(o => o.CreatedAt >= startOfMonth);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.Name.Contains(search));

            }




            var customerDetails = query.Select(o => new CustomerDetailViewModel
            {
                CustomerId = o.CustomerId,
                Date = o.CreatedAt,
                CustomerName = o.Name,
                CustomerEmail = o.Email,
                CustomerPhone = o.Phone,
                TotalOrder = o.Orders.Count()
            }).ToList();

            return new ExportOrdersResult
            {

                search = search,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                record = customerDetails.Count.ToString(),
                CustomerData = customerDetails
            };
        }

        public CustomerHistoryViewModel GetCustomer(int id)
        {

            try
            {
                var customer = _repository.GetCustomer(id);


                var result = new CustomerHistoryViewModel
                {
                    CustomerName = customer.Name,
                    CreatedOn = customer.CreatedAt,
                    CustomerEmail = customer.Email ?? string.Empty,
                    CustomerId = customer.CustomerId,
                    CustomerPhone = customer.Phone,
                    Visits = customer.Visits ?? 0,
                    //  OrderAmount = customer.Orders.Count(),
                    OrderAmount = (int)(customer.Orders.Any() ? customer.Orders.Sum(o => o.Amount) : 0),
                    Maxbill = (int)(customer.Orders.Any() ? customer.Orders.Max(o => o.Amount):0),
                    Avgbill = (float)(customer.Orders.Any() ? customer.Orders.Average(o => o.Amount):0),


                    orderDetailViewModels = customer.Orders.Select(o => new OrderDetailVM
                    {
                        OrderAmount = (decimal?)o.Amount,
                        PaymentMethod = o.Payments.FirstOrDefault() != null ? o.Payments.FirstOrDefault().PaymentMethod : string.Empty,
                        OrderDate = o.CreatedAt,
                        OrderType = o.OrderType ?? "Unknown",
                        TotalItem = o.OrderDetails.Count(),


                    }).ToList()

                };
                return result;

            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                throw new Exception("An error occurred while retrieving customer details.", ex);
            }
        }

    }
}