
using PizzaShop.service.Interfaces;
using Pizzashop.entity.ViewModels;
using PizzaShop.repository.Interfaces;


namespace PizzaShop.service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IKotRepository _kotrepository;

        public OrderService(IOrderRepository repository, IKotRepository kotrepository)
        {
            _repository = repository;
            _kotrepository = kotrepository;
        }



        // public async Task<AddPaginationViewmodel<OrderViewModel>> GetAllOrder(int page, int pageSize, string search, string date, string status, DateTime fromDate, DateTime toDate)
        // {
        //     var items = await _repository.GetAllOrder();
        //     int tableCount;
        //     toDate = toDate.AddDays(1);

        //     var itemViewModels = items.Select(c => new OrderViewModel
        //     {
        //         OrderId = c.OrderId,
        //         OrderDate = c.CreatedAt,
        //         CustomerName = c.Customer.Name ?? string.Empty,
        //         OrderStatus = c.OrderStatus,
        //         PaymentMethod = c.Payments.Any() && c.Payments.FirstOrDefault() != null ? c.Payments.FirstOrDefault().PaymentMethod : string.Empty,
        //         Rating = c.Feedbacks.Any() ? (int?)c.Feedbacks.Average(f => f.Rating) : 0,
        //         TotalAmount = c.Amount,

        //     }).ToList();


        //     if (!string.IsNullOrEmpty(search))
        //     {
        //         itemViewModels = itemViewModels.Where(u => u.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        //     }
        //     if (!string.IsNullOrEmpty(status))
        //     {
        //         itemViewModels = itemViewModels.Where(u => u.OrderStatus.Contains(status, StringComparison.OrdinalIgnoreCase)).ToList();
        //     }
        //     if (!string.IsNullOrEmpty(date) && date != "all")
        //     {
        //         DateTime now = DateTime.Now;

        //         if (date == "7")
        //         {
        //             var last7Days = now.AddDays(-7);
        //             itemViewModels = itemViewModels.Where(o => o.OrderDate >= last7Days).ToList();
        //         }
        //         else if (date == "30")
        //         {
        //             var last30Days = now.AddDays(-30);
        //             itemViewModels = itemViewModels.Where(o => o.OrderDate >= last30Days).ToList();
        //         }
        //         else if (date == "month")
        //         {
        //             var startOfMonth = new DateTime(now.Year, now.Month, 1);
        //             itemViewModels = itemViewModels.Where(o => o.OrderDate >= startOfMonth).ToList();
        //         }
        //     }

        //     if (fromDate != default(DateTime))
        //     {
        //         itemViewModels = itemViewModels.Where(x => x.OrderDate > fromDate).ToList();
        //     }
        //     if (toDate != default(DateTime).AddDays(1))
        //     {
        //         itemViewModels = itemViewModels.Where(x => x.OrderDate < toDate).ToList();
        //     }


        //     tableCount = itemViewModels.Count;

        //     itemViewModels = itemViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //     var order = new AddPaginationViewmodel<OrderViewModel>
        //     {
        //         Items = itemViewModels,
        //         TotalCount = tableCount,
        //         CurrentPage = page,
        //         PageSize = pageSize,
        //     };
        //     return order;
        // }

        public async Task<AddPaginationViewmodel<OrderViewModel>> GetAllOrder(int page, int pageSize, string search, string date, string status, DateTime fromDate, DateTime toDate, string orderbyorderid, string orderbyorderdate)
        {
            var items = await _repository.GetAllOrder();
            int tableCount;
            toDate = toDate.AddDays(1);

            var itemViewModels = items.Select(c => new OrderViewModel
            {
                OrderId = c.OrderId,
                OrderDate = c.CreatedAt,
                CustomerName = c.Customer.Name ?? string.Empty,
                OrderStatus = c.OrderStatus,
                PaymentMethod = c.Payments.Any() && c.Payments.FirstOrDefault() != null ? c.Payments.FirstOrDefault().PaymentMethod : string.Empty,
                Rating = c.Feedbacks.Any() ? (int?)c.Feedbacks.Average(f => f.Rating) : 0,
                TotalAmount = c.Amount,

            }).ToList();

            itemViewModels = (orderbyorderid, orderbyorderdate) switch
            {
                ("asc_name", "asc_name") => itemViewModels.OrderBy(u => u.OrderId).ThenBy(u => u.OrderDate).ToList(),
                ("asc_name", "dec_name") => itemViewModels.OrderBy(u => u.OrderId).ThenByDescending(u => u.OrderDate).ToList(),
                ("dec_name", "asc_name") => itemViewModels.OrderByDescending(u => u.OrderId).ThenBy(u => u.OrderDate).ToList(),
                ("dec_name", "dec_name") => itemViewModels.OrderByDescending(u => u.OrderId).ThenByDescending(u => u.OrderDate).ToList(),
                ("asc_name", _) => itemViewModels.OrderBy(u => u.OrderId).ToList(),
                ("dec_name", _) => itemViewModels.OrderByDescending(u => u.OrderId).ToList(),
                (_, "asc_name") => itemViewModels.OrderBy(u => u.OrderDate).ToList(),
                (_, "dec_name") => itemViewModels.OrderByDescending(u => u.OrderDate).ToList(),
                _ => itemViewModels.OrderBy(u => u.OrderId).ToList()
            };

            if (!string.IsNullOrEmpty(search))
            {
                itemViewModels = itemViewModels.Where(u => u.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                itemViewModels = itemViewModels.Where(u => u.OrderStatus.Contains(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(date) && date != "all")
            {
                DateTime now = DateTime.Now;

                if (date == "7")
                {
                    var last7Days = now.AddDays(-7);
                    itemViewModels = itemViewModels.Where(o => o.OrderDate >= last7Days).ToList();
                }
                else if (date == "30")
                {
                    var last30Days = now.AddDays(-30);
                    itemViewModels = itemViewModels.Where(o => o.OrderDate >= last30Days).ToList();
                }
                else if (date == "month")
                {
                    var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    itemViewModels = itemViewModels.Where(o => o.OrderDate >= startOfMonth).ToList();
                }
            }

            if (fromDate != default(DateTime))
            {
                itemViewModels = itemViewModels.Where(x => x.OrderDate > fromDate).ToList();
            }
            if (toDate != default(DateTime).AddDays(1))
            {
                itemViewModels = itemViewModels.Where(x => x.OrderDate < toDate).ToList();
            }


            tableCount = itemViewModels.Count;

            itemViewModels = itemViewModels.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var order = new AddPaginationViewmodel<OrderViewModel>
            {
                Items = itemViewModels,
                TotalCount = tableCount,
                CurrentPage = page,
                PageSize = pageSize,
            };
            return order;
        }



        public ExportOrdersResult GetExportOrders(string search, string status, int time, DateTime fromDate, DateTime toDate)
        {
            var query = _repository.GetAllOrderExport();


            var Date = DateTime.Now.AddDays(-time);

            if (time > 0)
            {
                query = query.Where(o => o.CreatedAt >= Date);
            }

            // Status filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.Customer.Name.Contains(search) ||
                    o.Payments.Any(p => p.PaymentMethod.Contains(search)));

            }


            if (fromDate != default(DateTime))
            {
                query = query.Where(x => x.CreatedAt > fromDate);
            }
            if (toDate != default(DateTime))
            {
                query = query.Where(x => x.CreatedAt < toDate);
            }

            var result = query.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                Date = o.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                CustomerName = o.Customer.Name,
                OrderStatus = o.OrderStatus,
                PaymentMethod = o.Payments.FirstOrDefault() != null ? o.Payments.FirstOrDefault().PaymentMethod : string.Empty,
                Rating = o.Feedbacks.Any() ? (int?)o.Feedbacks.Average(f => f.Rating) : 0,
                TotalAmount = o.Amount,
            }).ToList();

            return new ExportOrdersResult
            {
                status = status,
                search = search,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                record = result.Count.ToString(),
                orderData = result
            };
        }

        public OrderDetailViewModel GetOrderDetails(int id)
        {
            try
            {


                var ordered = _repository.GetOrderDetails(id);

                var result = new OrderDetailViewModel
                {
                    //InvoiceId = ordered.Invoices.FirstOrDefault()?.InvoiceId ?? 0,

                    OrderId = ordered.OrderId,
                    StartDate = ordered.CreatedAt,
                    EndDate = ordered.ModifiedAt ?? DateTime.Now,
                    OrderStatus = ordered.OrderStatus ?? string.Empty,
                    CustomerId = ordered.CustomerId,
                    CustomerName = ordered.Customer.Name,
                    CustomerEmail = ordered.Customer.Email ?? string.Empty,
                    CustomerPhone = ordered.Customer.Phone,
                    TotalPerson = ordered.TotalPerson,
                    OderAmount = ordered.Amount,
                    Subtotal = ordered.SubTotal!,
                    OtherTax = ordered.OtherTax!,

                    //  PaymentMode = ordered.Payments.Where(c => c.OrderId == id).Select(c => c.PaymentMethod).First(),
                    PaymentMode = _kotrepository.CheckPayment(id)?.PaymentMethod ?? string.Empty,
                    SectionName = ordered.OrderTableMappings.Select(o => o.Table.Section.SectionName).First(),
                    OrderItem = ordered.OrderDetails.Where(c => !c.IsDeleted).Select(m => new OrderManyItem
                    {
                        OrderDetailId = m.OrderDetailId,
                        ItemName = m.ItemId != null ? m.Item.ItemName : string.Empty,
                        ItemId = m.ItemId,
                        Quantity = m.Quntity,
                        Price = m.Item.Price,
                        OtherTax = m.Item.Tax,


                        modifier = m.OrderedItemModifiers.Select(x => new ModifierViewModel
                        {
                            // ModifierName = m.Modifier.ModifierName,
                            ModifierId = x.OrderedItemModifierId,
                            ModifierName = _repository.GetModifierName(x.OrderedItemModifierId),
                            ModifierPrice = _repository.GetModifierPrice(x.OrderedItemModifierId),
                            Quantity = m.Quntity
                        }).ToList(),
                    }).ToList(),

                    ManyTableList = _repository.TableMapppingAfterorder(id),

                    tax = _repository.TaxFind(id)
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetOrderDetails: " + ex.Message);
            }
        }

    }



}