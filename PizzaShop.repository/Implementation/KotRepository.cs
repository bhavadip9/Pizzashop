using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class KotRepository : IKotRepository
    {
        private readonly NewPizzashopContext _context;

        public KotRepository(NewPizzashopContext context)
        {
            _context = context;
        }


        // public List<Section> GetAllSection()
        // {
        //     try
        //     {
        //         return _context.Sections.Where(c => !c.IsDeleted).Include(o => o.Tables).ThenInclude(o => o.OrderTableMappings).ThenInclude(o => o.Order).ToList();
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
        //         return null;
        //     }
        // }
        public List<Section> GetAllSection()
        {
            try
            {
                // Call the PostgreSQL function via raw SQL
                var jsonResult = _context.Database.SqlQueryRaw<string>("SELECT get_all_sections_with_tables_and_orders()").AsEnumerable().FirstOrDefault();
                //  .ExecuteSqlInterpolated($"SELECT get_all_sections_with_tables_and_orders();");

                if (jsonResult != null)
                {
                    // Deserialize the JSON into a list of sections
                    var sections = JsonSerializer.Deserialize<List<Section>>(jsonResult);
                    return sections;
                }

                return new List<Section>(); // Return an empty list if no data found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        public List<Table> WaitingTable()
        {
            try
            {
                return _context.Tables.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        // public bool UpdateOrderDetail(OrderDetail data)
        // {
        //     try
        //     {
        //         _context.OrderDetails.Update(data);
        //         _context.SaveChanges();
        //         return true;
        //     }
        //     catch (DbUpdateException ex)
        //     {
        //         Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
        //         // throw;
        //         return false;
        //     }
        // }

        public bool UpdateOrderDetail(OrderDetail data)
        {
            try
            {
                var sql = "CALL update_order_detail(" +
                          "@p_order_detail_id, @p_item_id, @p_order_id, @p_quntity, @p_prepared, @p_item_comment, @p_is_deleted);";

                var parameters = new[]
                {
                     new NpgsqlParameter("@p_order_detail_id",    data.OrderDetailId),
                     new NpgsqlParameter("@p_item_id",            data.ItemId),
                     new NpgsqlParameter("@p_order_id",           data.OrderId),
                     new NpgsqlParameter("@p_quntity",            data.Quntity),
                     new NpgsqlParameter("@p_prepared",           data.Prepared),
                     new NpgsqlParameter("@p_item_comment",       (object?)data.ItemComment ?? DBNull.Value),
                     new NpgsqlParameter("@p_is_deleted",         data.IsDeleted),
                };

                _context.Database.ExecuteSqlRaw(sql, parameters);
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }
        public bool UpdateOrderDetail(OrderDetailsVM data)
        {
            try
            {
                var sql = "CALL update_order_detail(" +
                          "@p_order_detail_id, @p_item_id, @p_order_id, @p_quntity, @p_prepared, @p_item_comment, @p_is_deleted);";

                var parameters = new[]
                {
                     new NpgsqlParameter("@p_order_detail_id",    data.orderdetail_id),
                     new NpgsqlParameter("@p_item_id",            data.item_id),
                     new NpgsqlParameter("@p_order_id",           data.order_id),
                     new NpgsqlParameter("@p_quntity",            data.quantity),
                     new NpgsqlParameter("@p_prepared",           data.prepared),
                     new NpgsqlParameter("@p_item_comment",       (object?)data.item_comment ?? DBNull.Value),
                     new NpgsqlParameter("@p_is_deleted",         data.orderitem_isdelete ?? false),
                };

                _context.Database.ExecuteSqlRaw(sql, parameters);
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }


        // public WaitingTokenCode AddWaitingUser(WaitingTokenCode user)
        // {
        //     try
        //     {
        //         _context.WaitingTokenCodes.Add(user);
        //         _context.SaveChanges();
        //         return null!;
        //     }
        //     catch (DbUpdateException ex)
        //     {
        //         Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
        //         return null!;
        //     }
        // }
        public WaitingTokenCode AddWaitingUser(WaitingTokenCode user)
        {
            try
            {
                var sql = "CALL add_waiting_token_code(" +
                          "@p_total_person, @p_user_name, @p_phone_no, @p_email, @p_section_id, @p_created_at);";

                var parameters = new[]
                {
                     new NpgsqlParameter("@p_total_person",    user.TotalPerson),
                     new NpgsqlParameter("@p_user_name",           user.UserName),
                     new NpgsqlParameter("@p_phone_no",           user.PhoneNo),
                     new NpgsqlParameter("@p_email",            user.Email),
                     new NpgsqlParameter("@p_section_id",          user.SectionId),
                     new NpgsqlParameter("@p_created_at",       user.CreatedAt),
                };

                _context.Database.ExecuteSqlRaw(sql, parameters);
                return null!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }

        // public bool DeleteWatingToken(int id)
        // {
        //     var token = _context.WaitingTokenCodes.Find(id);
        //     token!.IsDelete = true;
        //     _context.WaitingTokenCodes.Update(token);
        //     _context.SaveChanges();
        //     return true;
        // }
        public bool DeleteWatingToken(int id)
        {
            var sql = "CALL deletewaitingtoken(@token_id)";
            var parameters = new[]
            {
              new NpgsqlParameter("token_id", id)
            };
            _context.Database.ExecuteSqlRaw(sql, parameters);
            return true;
        }
        // public WaitingTokenCode EditWaitingUser(WaitingTokenCode user)
        // {
        //     try
        //     {
        //         _context.WaitingTokenCodes.Update(user);
        //         _context.SaveChanges();
        //         return null!;
        //     }
        //     catch (DbUpdateException ex)
        //     {
        //         Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
        //         // throw;
        //         return null!;
        //     }
        // }
        // public WaitingTokenCode EditWaitingUser(WaitingTokenCode user)
        // {
        //     _context.Database.ExecuteSqlRaw(
        //          "CALL edit_waiting_user({0}, {1}, {2}, {3}, {4}, {5}, {6})",
        //          user.TokenId,
        //          user.TotalPerson,
        //          user.UserName,
        //          user.PhoneNo,
        //          user.Email,
        //          user.SectionId,
        //          user.ModifiedAt
        //      );

        //     return user;
        // }

        public WaitingTokenCode EditWaitingUser(WaitingTokenCode user)
        {
           
            _context.Database.ExecuteSqlRaw(
                "Select * from edit_waiting_user({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                user.TokenId,        
                user.TotalPerson,    
                user.UserName,     
                user.PhoneNo,        
                user.Email,       
                user.SectionId,    
                user.ModifiedAt      
            );

            // Returns the updated user (as in, what you just sent in)
            return user;
        }
        public List<WaitingTokenCode> GetNewWaitnigUserAllDetail(int id)
        {
            try
            {
                var result = _context.Set<WaitingTokenCode>()
                                                  .FromSqlRaw("SELECT * FROM public.get_waiting_token_code_by_id({0})", id)
                                                  .ToList();

                // var result = _context.Set<WaitingTokenCode>()
                //                   .FromSqlRaw("SELECT * FROM public.get_waiting_token_code_by_id({0})", id)
                //                   .FirstOrDefault();


                return result;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public WaitingTokenCode GetWaitnigUserAllDetail(int id)
        {
            try
            {
                return _context.WaitingTokenCodes.Where(c => c.TokenId == id && !c.IsDelete).Include(o => o.Section).ThenInclude(o => o.Tables).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        // public List<WaitingTokenCode> GetAllWaitingUser()
        // {
        //     try
        //     {
        //         return _context.WaitingTokenCodes.Where(o => !o.IsDelete).Include(o => o.Section).ThenInclude(o => o.Tables).ToList();
        //     }
        //     catch (DbUpdateException ex)
        //     {
        //         Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
        //         // throw;
        //         return null!;
        //     }
        // }
        public List<WaitingTokenCode> GetAllWaitingUser()
        {
            try
            {
                var query = "SELECT * FROM get_all_waiting_users()";

                return _context.WaitingTokenCodes.FromSqlRaw(query).AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null!;
            }
        }


        public int AddAssignTable(WaitingUserDetails waitingUser)
        {
            try
            {
                var orderId = _context.Database.SqlQueryRaw<int>(
                               "SELECT assign_table1(@p_waiting_user_id, @p_section_id,@p_user_name,@p_email,@p_total_person,@p_phone, @p_selected_tablelist)",
                               new NpgsqlParameter("@p_waiting_user_id", waitingUser.WaitingUserId),
                               new NpgsqlParameter("@p_section_id", waitingUser.SectionId),
                               new NpgsqlParameter("@p_user_name", waitingUser.UserName),
                               new NpgsqlParameter("@p_email", waitingUser.Email),
                               new NpgsqlParameter("@p_total_person", waitingUser.No_of_Person),
                               new NpgsqlParameter("@p_phone", waitingUser.Phone),

                               new NpgsqlParameter
                               {
                                   ParameterName = "@p_selected_tablelist",
                                   NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer,
                                   Value = waitingUser.SelectedTable.ToArray()
                               }
                           ).AsEnumerable().FirstOrDefault();

                return orderId;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return 0; // or throw an exception, or handle it as needed
            }

            // var orderId = _context.Database.SqlQueryRaw<int>(
            //     "SELECT assign_table(@p_waiting_user_id, @p_section_id, @p_selected_tablelist)",
            //     new NpgsqlParameter("@p_waiting_user_id", waitingUser.WaitingUserId),
            //     new NpgsqlParameter("@p_section_id", waitingUser.SectionId),
            //     new NpgsqlParameter("@p_selected_tablelist", waitingUser.SelectedTable.ToArray())
            // ).FirstOrDefault();

            // return orderId;

        }
        public int AddAssignTable(TableAndSectionViewModel waitingUser)
        {
            var orderId = _context.Database.SqlQueryRaw<int>(
                "SELECT assign_table(@p_waiting_user_id, @p_section_id, @p_selected_tablelist)",
                new NpgsqlParameter("@p_waiting_user_id", waitingUser.WaitingUserId),
                new NpgsqlParameter("@p_section_id", waitingUser.sectionId),
                new NpgsqlParameter
                {
                    ParameterName = "@p_selected_tablelist",
                    NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer,
                    Value = waitingUser.SelectedTablelist.ToArray()
                }
            ).AsEnumerable().FirstOrDefault();

            return orderId;

        }
        public List<Table> GetAllTableBySectionId(int sectionId)
        {
            try
            {
                return _context.Tables.Where(c => c.SectionId == sectionId && !c.IsDelete).ToList();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public Table GetTableDetail(int tableid)
        {
            try
            {
                return _context.Tables.Where(c => c.TableId == tableid && !c.IsDelete).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public WaitingTokenCode GetWaitnigUserById(int id)
        {
            try
            {
                return _context.WaitingTokenCodes.Where(c => c.TokenId == id && !c.IsDelete).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }

        public Customer GetWaitingUser(string email)
        {
            try
            {
                return _context.Customers.Where(c => c.Email!.ToLower() == email.ToLower()).Include(o => o.Orders.Where(c => c.OrderStatus != "Complete")).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }

        public Customer GetAllCustomer(string Email1)
        {
            try
            {
                return _context.Customers.Where(c => !c.IsDelete).FirstOrDefault(o => o.Email == Email1)!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public Customer AddUserAsCustomer(Customer user)
        {
            try
            {
                var userEmail = _context.Customers.Where(c => !c.IsDelete).FirstOrDefault(o => o.Email == user.Email);
                if (userEmail == null)
                {
                    _context.Customers.Add(user);
                    _context.SaveChanges();
                    return user;
                }
                else
                {
                    _context.Customers.Update(user);
                    _context.SaveChanges();
                    return user;
                }


            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public Order AddOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                return order;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public Payment AddPayment(Payment payment)
        {
            try
            {
                _context.Payments.Add(payment);
                _context.SaveChanges();
                return payment;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public Payment UpdatePayment(Payment payment)
        {
            try
            {
                _context.Payments.Update(payment);
                _context.SaveChanges();
                return payment;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public Payment CheckPayment(long orderid)
        {
            try
            {
                var result = _context.Payments.FirstOrDefault(o => o.OrderId == orderid);

                if (result != null)
                {
                    return result;
                }
                return null!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }


        public Table UpdateTable(Table table)
        {
            try
            {
                _context.Tables.Update(table);
                _context.SaveChanges();
                return table;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                // throw;
                return null!;
            }

        }

        public string GetTableName(int tableId)
        {
            try
            {
                return _context.Tables.Where(c => c.TableId == tableId).FirstOrDefault()!.TableName;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }
        public WaitingTokenCode UpdateWaitingUser(WaitingTokenCode table)
        {
            try
            {
                _context.WaitingTokenCodes.Update(table);
                _context.SaveChanges();
                return table;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }

        }

        public OrderTableMapping AddTableOrder(OrderTableMapping user)
        {
            try
            {
                _context.OrderTableMappings.Add(user);
                _context.SaveChanges();
                return null!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null!;
            }
        }


        // public List<MenuItem> GetAllItem()
        // {
        //     try
        //     {
        //         var result = _context.MenuItems.Where(c => !c.IsDeleted).Where(o => o.IsAvailable).ToList();
        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.Message);
        //         return null!;
        //     }
        // }

        public List<MenuItem> GetAllItem()
{
    return _context.MenuItems
        .FromSqlRaw("SELECT * FROM get_all_menu_items()")
        .ToList();
}



        public List<Section> GetAllSectionAnSelected()
        {
            try
            {
                return _context.Sections
                       .Where(c => !c.IsDeleted).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;

            }
        }
        public List<Table> GetAllTable(int sectionid)
        {
            try
            {
                //return _context.Tables.Where(o => o.SectionId == sectionid).Where(c => !c.IsDelete).Where(o => o.Status == "Available").ToList();
                return _context.Tables.Where(o => o.SectionId == sectionid && !o.IsDelete && o.Status == "Available").ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }



        public List<MenuItem> GetAllItemForMenuApp(int categoryId)
        {
            try
            {
                var result = _context.MenuItems.Where(c => c.CategoryId == categoryId).Where(c => !c.IsDeleted).Where(c => c.IsAvailable).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }



        public async Task<List<Order>> GetAllItem(int orderId)
        {
            try
            {
                var result = await _context.Orders.Where(c => c.OrderId == orderId).Where(c => (bool)!c.IsDelete).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }


        public List<MenuItem> GetAllModifier(int id)
        {
            var Value = _context.MenuItems
                .Include(m => m.MappingItemModifierGroups)
                .ThenInclude(m => m.ModifierGroup)
                .ThenInclude(m => m.MappingModifierModifiergroups.Where(mg => !mg.IsDeleted))
                .ThenInclude(m => m.Modifier)
                .Where(m => m.ItemId == id && !m.IsDeleted).ToList();

            return Value;
        }
        // public async Task<List<Order>> GetAllOrderOld()
        // {
        //     return await _context.Orders.Where(c => !c.IsDelete).Include(c => c.OrderDetails).ThenInclude(o => o.Item).ThenInclude(o => o.Category)
        //         .Include(c => c.OrderDetails).ThenInclude(o => o.OrderedItemModifiers).Include(c => c.OrderTableMappings).ThenInclude(o => o.Table).ThenInclude(o => o.Section).ToListAsync();
        // }
        public async Task<List<FlatOrderDto>> GetAllOrder()
        {
            var orders = await _context.FlatOrderDtos
        .FromSqlRaw("SELECT * FROM get_all_orders()")
        .ToListAsync();

            return orders;
        }
        public async Task<List<Order>> GetAllProgressOrder()
        {
            return await _context.Orders.Where(c => !c.IsDelete).Where(c => c.OrderStatus != "Completed").Include(c => c.OrderDetails).ThenInclude(o => o.Item).ThenInclude(o => o.Category)
                .Include(c => c.OrderDetails).ThenInclude(o => o.OrderedItemModifiers).Include(c => c.OrderTableMappings).ThenInclude(o => o.Table).ThenInclude(o => o.Section).ToListAsync();
        }
        public Customer GetCustomerDetail(int id)
        {
            try
            {
                return _context.Customers.Where(c => c.CustomerId == id && !c.IsDelete).Include(o => o.Orders).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }

        public Order GetOrderDetail(int id)
        {
            try
            {
                return _context.Orders.Where(c => c.OrderId == id && !c.IsDelete).Include(o => o.Payments).Include(o => o.OrderTableMappings).ThenInclude(o => o.Table).Include(c => c.OrderDetails).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }
        public Order NewGetOrderDetail(int id)
        {
            try
            {
                return _context.Orders.Where(c => c.OrderId == id && !c.IsDelete).Include(o => o.Payments).Include(o => o.OrderTableMappings).ThenInclude(o => o.Table).Include(c => c.OrderDetails).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }
        
        public Order GetOrderUpdate(int id)
        {
            try
            {
                return _context.Orders.Where(c => c.OrderId == id && !c.IsDelete).Include(o => o.OrderTableMappings).ThenInclude(o => o.Table).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }

        public bool UpdateCustomer(Customer customer)
        {
            if (customer.CustomerId == null)
            {
                return false;
            }
            var user = _context.Customers.Find(customer.CustomerId);
            _context.Customers.Update(user);
            _context.SaveChanges();
            return true;
        }
        public bool UpdateOrder(Order order)
        {
            if (order.OrderId == null)
            {
                return false;
            }
            var user = _context.Orders.Find(order.OrderId);
            _context.Orders.Update(user);
            _context.SaveChanges();
            return true;
        }

        public OrderDetail GetOrder(int DetailId)
        {
            try
            {
                return _context.OrderDetails.Where(c => c.OrderDetailId == DetailId).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }
        public OrderDetail GetDetail(int detailid)
        {
            try
            {
                return _context.OrderDetails.Where(c => c.OrderDetailId == detailid).FirstOrDefault()!;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }
        // public List<OrderDetail> GetOrderDetailList(int id)
        // {
        //     try
        //     {
        //         return _context.OrderDetails.Where(c => c.OrderId == id).Where(c => !c.IsDeleted).Include(o => o.OrderedItemModifiers).ToList()!;
        //     }
        //     catch (DbUpdateException ex)
        //     {
        //         Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
        //         return null;
        //     }
        // }
        public List<OrderDetail> GetOrderDetailList(int id)
        {
            try
            {

                var result = _context.Database.SqlQueryRaw<string>("SELECT get_order_detail_list({0})", id).AsEnumerable().FirstOrDefault();
                if (result != null)
                {
                    return JsonSerializer.Deserialize<List<OrderDetail>>(result);
                }
                return new List<OrderDetail>();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }



        public OrderTableMapping GetTableMapping(int id)
        {
            return _context.OrderTableMappings.FirstOrDefault(m => m.TableId == id);
        }

        public bool UpdatetableMapping(OrderTableMapping mapping)
        {
            try
            {
                _context.OrderTableMappings.Update(mapping);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }

        }


        public bool AddNewOrderitem(OrderDetail order)
        {
            try
            {
                _context.OrderDetails.Add(order);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }

        public List<OrderTaxMapping> GetTaxMpping(int orderid)
        {
            try
            {
                return _context.OrderTaxMappings.Where(c => c.OrderId == orderid).ToList();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }

        public OrderTaxMapping GetOrderTaxMapping(int orderId, int taxId)
        {
            return _context.OrderTaxMappings
                           .FirstOrDefault(x => x.OrderId == orderId && x.TaxId == taxId);
        }
        public bool AddOrderTaxMapping(OrderTaxMapping tax)
        {
            try
            {
                _context.OrderTaxMappings.Add(tax);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }
        public bool UpdateOrderTaxMapping(OrderTaxMapping tax)
        {
            try
            {
                _context.OrderTaxMappings.Update(tax);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }

        public bool AddFeedback(Feedback review)
        {
            try
            {
                _context.Feedbacks.Add(review);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }

        public List<WaitingTokenCode> GetWaitingtokensListByDate(DateTime fromdate)
        {
            try
            {
                List<WaitingTokenCode> waitingTokens = _context.WaitingTokenCodes.Where(o => o.CreatedAt >= fromdate)
                                       .ToList();
                if (waitingTokens == null)
                {
                    return null;
                }

                return waitingTokens;
            }
            catch (Exception ex)
            {
                return new List<WaitingTokenCode>();
            }
        }


        public int ConfirmationCancel(int orderId)
        {
            var result = _context.Database.SqlQueryRaw<int>("SELECT confirmation_cancel({0})", orderId).AsEnumerable().FirstOrDefault();
            return result;
        }

        public bool ConfirmationOrderStatus(ConfirmationOrder model)
        {

            var selectedTaxesParam = new NpgsqlParameter
            {
                ParameterName = "@p_selected_taxes",
                NpgsqlDbType = NpgsqlDbType.Jsonb,
                Value = Newtonsoft.Json.JsonConvert.SerializeObject(model.selectedTaxes)
            };
            var result = _context.Database.SqlQueryRaw<bool>(
                            "SELECT confirmation_order_status(@p_orderid, @p_customerid, @p_totalbill,@p_subtotal,@p_othertax,@p_selected_taxes)",
                            new NpgsqlParameter("@p_orderid", model.orderid),
                            new NpgsqlParameter("@p_customerid", model.customerid),
                            new NpgsqlParameter("@p_totalbill", model.totalbill),
                            new NpgsqlParameter("@p_subtotal", model.subtotal),
                            new NpgsqlParameter("@p_othertax", model.othertax),
                            selectedTaxesParam

                        ).AsEnumerable().FirstOrDefault();

            if (result = true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // public int SaveOrder(int orderId, float TotalAmount, List<AddOrderViewModel> model)
        // {
        //     var SelectModel = new NpgsqlParameter
        //     {
        //         ParameterName = "@p_model",
        //         NpgsqlDbType = NpgsqlDbType.Json,
        //         Value = JsonSerializer.Serialize(model)
        //     };
        //     var result = _context.Database.SqlQueryRaw<int>(
        //         "SELECT AddOrderItem(@p_order_id, @p_total_amount, @p_model)",
        //         new NpgsqlParameter("@p_order_id", orderId),
        //         new NpgsqlParameter("@p_total_amount", TotalAmount),
        //         SelectModel
        //     ).AsEnumerable().FirstOrDefault();


        //     return result;
        // }

        public int SaveOrder(int orderId, float TotalAmount, List<AddOrderViewModel> model)
        {
            var SelectModel = new NpgsqlParameter
            {
                ParameterName = "@p_model",
                NpgsqlDbType = NpgsqlDbType.Json,
                Value = JsonSerializer.Serialize(model)
            };
            var result = _context.Database.SqlQueryRaw<int>(
    "SELECT AddOrderItem(@p_order_id, @p_total_amount::numeric, @p_model)",
    new NpgsqlParameter("@p_order_id", orderId),
    new NpgsqlParameter("@p_total_amount", TotalAmount),
    SelectModel
).AsEnumerable().FirstOrDefault();
            // var result = _context.Database.SqlQueryRaw<int>(
            //     "SELECT AddOrderItem(@p_order_id, @p_total_amount, @p_model)",
            //     new NpgsqlParameter("@p_order_id", orderId),
            //     new NpgsqlParameter("@p_total_amount", TotalAmount),
            //     SelectModel
            // ).AsEnumerable().FirstOrDefault();

            return result;
        }

    }
}