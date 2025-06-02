
using Microsoft.EntityFrameworkCore;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {

        private readonly NewPizzashopContext _context;

        public OrderRepository(NewPizzashopContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrder()
        {
            try
            {
                var result = _context.Orders.Include(c => c.Customer)
                                               .Include(c => c.Feedbacks)
                                               .Include(c => c.Payments)
                                               .ToList();
                return result;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }



        public List<AddTableViewModel> TableMappping(int id)
        {
            try
            {
                var tableQuery = _context.OrderTableMappings.Where(c => !c.IsDelete).Where(x => x.OrderId == id).Select(table => new AddTableViewModel
                {
                    TableId = table.Table != null ? table.Table.TableId : 0,
                    TableName = table.Table != null ? table.Table.TableName : string.Empty,
                    SectionName = table.Table != null && table.Table.Section != null ? table.Table.Section.SectionName : string.Empty,
                    Capacity = table.Table != null ? table.Table.Capacity : 0
                }).ToList();
                return tableQuery!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public List<AddTableViewModel> TableMapppingAfterorder(int id)
        {
            try
            {
                var tableQuery = _context.OrderTableMappings.Where(x => x.OrderId == id).Select(table => new AddTableViewModel
                {
                    TableId = table.Table != null ? table.Table.TableId : 0,
                    TableName = table.Table != null ? table.Table.TableName : string.Empty,
                    SectionName = table.Table != null && table.Table.Section != null ? table.Table.Section.SectionName : string.Empty,
                    Capacity = table.Table != null ? table.Table.Capacity : 0
                }).ToList();
                return tableQuery!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }

        public List<AddTaxViewModel> TaxFind(int id)
        {
            try
            {
                var taxQuery = _context.OrderTaxMappings.Where(c => !c.IsDeleted).Where(x => x.OrderId == id).Select(tax => new AddTaxViewModel
                {
                    TaxName = tax.Tax != null ? tax.Tax.TaxName : string.Empty,
                    TotalTax = tax.TaxAmount,
                    TaxType = tax.Tax.TaxType,
                    TaxId = tax.TaxId
                }).ToList();

                return taxQuery;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }

        public List<TaxesAndFee> AllTaxForOrder()
        {
            try
            {
                return _context.TaxesAndFees.Where(x => x.IsDelete == false && x.IsEnabled == true).ToList();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }



        public IQueryable<Order> GetAllOrderExport()
        {
            try
            {
                var query = _context.Orders.AsNoTracking().Include(o => o.Payments).Include(e => e.Customer).AsQueryable();
                return query;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }



        public Order GetOrderDetails(int id)
        {
            try
            {
                var order = _context.Orders.Where(c => !c.IsDelete).Include(c => c.Payments).Include(c => c.Feedbacks).Include(a => a.Invoices).Include(c => c.OrderDetails).ThenInclude(o => o.Item)
                .Include(c => c.OrderDetails).ThenInclude(o => o.OrderedItemModifiers)
                .Include(c => c.Customer).Include(o => o.OrderTableMappings).ThenInclude(c => c.Table).ThenInclude(s => s.Section)
                .FirstOrDefault(c => c.OrderId == id);
                return order;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public List<OrderDetailsVM> GetOrderDetailsVM(int id)
        {
            try
            {
                var result = _context.Set<OrderDetailsVM>()
                     .FromSqlRaw("SELECT * FROM public.get_order_details({0})", id)
                     .ToList();


                return result;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }

        public string GetModifierName(int OrderModifierId)
        {
            try
            {
                var modifierName = _context.OrderedItemModifiers.Include(x => x.Modifier).FirstOrDefault(c => c.OrderedItemModifierId == OrderModifierId).Modifier.ModifierName;
                return modifierName!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public int GetModifierQuntity(int OrderModifierId)
        {
            try
            {
                var quantity = _context.OrderedItemModifiers.Include(x => x.Modifier).FirstOrDefault(c => c.OrderedItemModifierId == OrderModifierId).Modifier.Quantity;
                return quantity;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return 0;
            }
        }
        public int GetModifierPrice(int OrderModifierId)
        {
            try
            {
                var quantity = _context.OrderedItemModifiers.Include(x => x.Modifier).FirstOrDefault(c => c.OrderedItemModifierId == OrderModifierId).Modifier.Rate;
                return (int)(quantity);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return 0;
            }
        }





        public List<Order> OrderDetailsForDashboard(DateTime fromdate)
        {
            try
            {
                List<Order> orders = _context.Orders.Where(o => o.CreatedAt >= fromdate)
                                        .Include(o => o.OrderDetails).ThenInclude(oi => oi.Item).ToList();

                if (orders == null)
                {
                    return null!;
                }

                return orders;
            }
            catch (Exception ex)
            {
                return new List<Order>();
            }
        }
    }
}