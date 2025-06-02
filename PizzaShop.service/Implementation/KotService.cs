
using PizzaShop.service.Interfaces;
using PizzaShop.entity.Models;
using Pizzashop.entity.ViewModels;
using PizzaShop.repository.Interfaces;
using Table = PizzaShop.entity.Models.Table;


namespace PizzaShop.service.Implementation
{
    public class KotService : IKotService

    {
        private readonly IKotRepository _repository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ICategoryRepository _categoryrepository;

        private readonly IOrderRepository _orderrepository;


        public KotService(IKotRepository repository, ICategoryRepository categoryrepository, ISectionRepository sectionRepository, IOrderRepository orderrepository)
        {
            _repository = repository;
            _categoryrepository = categoryrepository;
            _sectionRepository = sectionRepository;
            _orderrepository = orderrepository;
        }



        //Get All Category
        public List<AddCategoryViewModel> GetAllCategoryList()
        {
            try
            {
                var categories = _categoryrepository.GetAllCategoryLIst();
                var categorieslist = categories.Select(p => new AddCategoryViewModel
                {
                    CategoryName = p.CategoryName,
                    CategoryId = p.CategoryId
                }).ToList();

                return categorieslist;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        //     public async Task<AddPaginationViewmodel<OrderDetailViewModel>> GetAllOrder(string status, int page, int pageSize)
        //     {

        //         var detail = await _repository.GetAllOrder();
        //         var tableCount = 0;
        //         var WaitingDetails = detail
        // .Where(c => c.order_status != "Complete")
        // .GroupBy(c => c.order_id)
        // .Select(orderGroup =>
        //     new OrderDetailViewModel
        //     {
        //         OrderId = orderGroup.Key,
        //         StartDate = orderGroup.FirstOrDefault()?.createdat ?? DateTime.Now,
        //         OrderComment = orderGroup.FirstOrDefault()?.order_comment ?? "",
        //         section = orderGroup.FirstOrDefault()?.section_name ?? "",
        //         TableList = orderGroup
        //             .Select(x => x.table_name ?? "")
        //             .Distinct()
        //             .ToList(),
        //         Assigntime = orderGroup.FirstOrDefault()?.table_modifiedat ?? DateTime.Now,
        //         OrderItem = orderGroup
        //             .Where(x => !(x.orderitem_isdelete ?? false)) // only items not marked deleted
        //             .GroupBy(x => x.orderdetail_id)
        //             .Select(itemGroup =>
        //             {
        //                 var first = itemGroup.FirstOrDefault();
        //                 return new OrderManyItem
        //                 {
        //                     ItemName = first?.item_name ?? "",
        //                     ItemId = first?.item_id ?? 0,
        //                     ItemByComment = first?.item_comment ?? "",
        //                     categoryid = first?.categoryid ?? 0,
        //                     // Calculate total pending quantity:
        //                     Quantity =(first.quantity??0) - (first?.prepared??0), // Assuming quantity is the total ordered and prepared is the total prepared
        //                    // Quantity = (itemGroup.Sum(x => x.quantity ?? 0) - itemGroup.Sum(x => x.prepared ?? 0)),
        //                    // Prepared = itemGroup.Sum(x => x.prepared ?? 0),
        //                    Prepared = first?.prepared ?? 0,
        //                     modifier = itemGroup
        //                         .Where(x => x.ordered_item_modifier_id != null)
        //                         .GroupBy(x => x.ordered_item_modifier_id)
        //                         .Select(mg =>
        //                         {
        //                             var modifierRow = mg.FirstOrDefault();
        //                             return new ModifierViewModel
        //                             {
        //                                 ModifierId = mg.Key.Value,
        //                                 ModifierName = _orderrepository.GetModifierName(mg.Key.Value), 
        //                             };
        //                         })

        //                         .ToList()

        //                 };
        //             })
        //               // .Where(oi => oi.Quantity > 0)
        //             .ToList()
        //     }
        // )
        // .Where(o => o.OrderItem.Count > 0)   // Only include orders with at least one pending item
        // .ToList();

        //         // var WaitingDetails = detail.Where(c => c.OrderStatus != "Complete").Select(c => new OrderDetailViewModel
        //         // {
        //         //     OrderId = c.OrderId,
        //         //     OrderComment = c.OrderComment!,
        //         //     OrderStatus = c.OrderStatus!,
        //         //     ManyTableList = c.OrderTableMappings.Select(o => new AddTableViewModel
        //         //     {
        //         //         TableName = o.Table.TableName,
        //         //         TableId = (int)o.TableId!,
        //         //     }).ToList(),

        //         //     SectionName = c.OrderTableMappings.Select(o => o.Table.Section.SectionName).First(),

        //         //     Assigntime = (DateTime)c.OrderTableMappings.Select(c => c.Table.ModifiedAt).First()!,

        //         //     OrderItem = c.OrderDetails.Where(c => !c.IsDeleted).Select(m => new OrderManyItem
        //         //     {
        //         //         ItemName = m.ItemId != null ? m.Item.ItemName : string.Empty,
        //         //         ItemId = m.ItemId,
        //         //         Quantity = (int)(m.Quntity - m.Prepared)!,
        //         //         ItemByComment = m.ItemComment!,
        //         //         Prepared = m.Prepared,

        //         //         modifier = m.OrderedItemModifiers.Select(m => new ModifierViewModel
        //         //         {
        //         //             ModifierId = m.OrderedItemModifierId,
        //         //             ModifierName = _orderrepository.GetModifierName(m.OrderedItemModifierId),
        //         //             ModifierPrice = (int)m.Modifier.Rate,
        //         //             Quantity = m.Quantity
        //         //         }).ToList(),
        //         //     }).ToList(),
        //         // }).Where(o => o.OrderItem.Count > 0).ToList();


        //         var count = 0;
        //         if (status == "ready")
        //         {
        //             foreach (var order in WaitingDetails)
        //             {
        //                 order.OrderItem = order.OrderItem.Where(i => i.Prepared > 0).ToList();
        //                 count++;
        //             }
        //             tableCount = count;
        //             WaitingDetails = WaitingDetails.Where(o => o.OrderItem.Count > 0).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //             return new AddPaginationViewmodel<OrderDetailViewModel>
        //             {
        //                 Items = WaitingDetails,
        //                 TotalCount = tableCount,
        //                 CurrentPage = page,
        //                 PageSize = pageSize,
        //             };
        //         }
        //         else
        //         {
        //             var newCount = 0;
        //             foreach (var order in WaitingDetails)
        //             {
        //                 order.OrderItem = order.OrderItem.Where(item => item.Quantity > 0 ).ToList();
        //                 newCount++;

        //             }
        //             tableCount = newCount;
        //             WaitingDetails = WaitingDetails.Where(o => o.OrderItem.Count > 0).Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //             return new AddPaginationViewmodel<OrderDetailViewModel>
        //             {
        //                 Items = WaitingDetails,
        //                 TotalCount = tableCount,
        //                 CurrentPage = page,
        //                 PageSize = pageSize,
        //             };
        //         }
        //     }

        public async Task<AddPaginationViewmodel<OrderDetailViewModel>> GetAllOrder(string status, int page, int pageSize)
        {

            var detail = await _repository.GetAllOrder();


            var orders = detail
                .Where(c => c.order_status == "Running")
                .GroupBy(c => c.order_id)
                .Select(orderGroup =>
                    new OrderDetailViewModel
                    {
                        OrderId = orderGroup.Key,
                        StartDate = orderGroup.FirstOrDefault()?.createdat ?? DateTime.Now,
                        OrderComment = orderGroup.FirstOrDefault()?.order_comment ?? "",
                        section = orderGroup.FirstOrDefault()?.section_name ?? "",
                        TableList = orderGroup
                            .Select(x => x.table_name ?? "")
                            .Distinct()
                            .ToList(),
                        Assigntime = orderGroup.FirstOrDefault()?.table_modifiedat ?? DateTime.Now,
                        OrderItem = orderGroup
                            .Where(x => !(x.orderitem_isdelete ?? false))
                            .GroupBy(x => x.orderdetail_id)
                            .Select(itemGroup =>
                            {
                                var first = itemGroup.FirstOrDefault();
                                return new OrderManyItem
                                {
                                    ItemName = first?.item_name ?? "",
                                    ItemId = first?.item_id ?? 0,
                                    ItemByComment = first?.item_comment ?? "",
                                    categoryid = first?.categoryid ?? 0,
                                    Quantity = (first?.quantity ?? 0) - (first?.prepared ?? 0),
                                    Prepared = first?.prepared ?? 0,
                                    modifier = itemGroup
                                        .Where(x => x.ordered_item_modifier_id != null)
                                        .GroupBy(x => x.ordered_item_modifier_id)
                                        .Select(mg =>
                                        {
                                            var modifierRow = mg.FirstOrDefault();
                                            return new ModifierViewModel
                                            {
                                                ModifierId = mg.Key.Value,
                                                ModifierName = _orderrepository.GetModifierName(mg.Key.Value)
                                            };
                                        })
                                        .ToList()
                                };
                            })
                            .ToList()
                    }
                )
                .ToList();


            foreach (var order in orders)
            {
                if (status == "ready")
                {
                    order.OrderItem = order.OrderItem
                        .Where(i => i.Prepared > 0)
                        .ToList();
                }
                else
                {
                    order.OrderItem = order.OrderItem
                        .Where(i => i.Quantity > 0)
                        .ToList();
                }
            }


            var filteredOrders = orders
                .Where(o => o.OrderItem.Count > 0)
                .ToList();


            int totalCount = filteredOrders.Count;
            var pagedOrders = filteredOrders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return new AddPaginationViewmodel<OrderDetailViewModel>
            {
                Items = pagedOrders,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }


        public async Task<AddPaginationViewmodel<OrderDetailViewModel>> CategoryWithOrder(int categoryid, int page, int pageSize, string status)
        {
            var ordercard = await _repository.GetAllOrder();
            var category = _categoryrepository.GetOneByIdAsync(categoryid).CategoryName;

            int tableCount;
            var itemViewModels = ordercard
    .Where(c => c.order_status != "Complete")
    .GroupBy(c => c.order_id)
    .Select(orderGroup =>
        new OrderDetailViewModel
        {
            OrderId = orderGroup.Key,
            StartDate = orderGroup.First().createdat ?? DateTime.Now,
            OrderComment = orderGroup.First().order_comment!,
            section = orderGroup.First().section_name!,
            TableList = orderGroup.Select(x => x.table_name).Distinct().ToList()!,
            Assigntime = orderGroup.Select(x => x.table_modifiedat).First() ?? DateTime.Now,
            OrderItem = orderGroup
                .Where(x => !x.orderitem_isdelete ?? false)
                .GroupBy(x => x.orderdetail_id)
                .Select(itemGroup => new OrderManyItem
                {
                    ItemName = itemGroup.First().item_name ?? string.Empty,
                    ItemId = itemGroup.First().item_id ?? 0,
                    ItemByComment = itemGroup.First().item_comment!,
                    categoryid = itemGroup.First().categoryid ?? 0,
                    Quantity = itemGroup.First().quantity - itemGroup.First().prepared ?? 0,
                    Prepared = itemGroup.First().prepared ?? 0,
                    modifier = itemGroup
                        .Where(x => x.ordered_item_modifier_id != null)
                        .GroupBy(x => x.ordered_item_modifier_id)
                        .Select(mg => new ModifierViewModel
                        {
                            ModifierId = mg.Key!.Value,
                            ModifierName = _orderrepository.GetModifierName(mg.Key.Value),

                        })
                        .ToList()

                })
                .Where(oi => oi.Quantity > 0)
                .ToList()
        }
    )
    .Where(o => o.OrderItem.Count > 0)
    .ToList();

            // var itemViewModels = ordercard.Where(c => c.OrderStatus != "Complete").Select(c => new OrderDetailViewModel
            // {
            //     OrderId = c.OrderId,
            //     StartDate = (DateTime)c.CreatedAt,
            //     OrderComment = c.OrderComment,
            //     section = c.OrderTableMappings.First().Table.Section.SectionName,
            //     TableList = c.OrderTableMappings.Select(t => t.Table.TableName).ToList(),
            //     Assigntime = (DateTime)c.OrderTableMappings.Select(c => c.Table.ModifiedAt).First()!,
            //     OrderItem = c.OrderDetails.Where(c => !c.IsDeleted).Select(m => new OrderManyItem
            //     {
            //         ItemName = m.ItemId != null ? m.Item.ItemName : string.Empty,
            //         ItemId = m.ItemId,
            //         ItemByComment = m.ItemComment!,
            //         categoryid = (int)m.Item.CategoryId!,
            //         Quantity = (m.Quntity - m.Prepared),
            //         Prepared = m.Prepared,
            //         modifier = m.OrderedItemModifiers.Select(m => new ModifierViewModel
            //         {
            //             ModifierId = m.OrderedItemModifierId,
            //             ModifierName = _orderrepository.GetModifierName(m.OrderedItemModifierId),
            //             ModifierPrice = (int)m.Modifier.Rate,
            //             Quantity = m.Quantity
            //         }).ToList()
            //     }).ToList()
            // }).Where(o => o.OrderItem.Count > 0).ToList();


            if (categoryid != 0)
            {
                itemViewModels = itemViewModels.Where(o => o.OrderItem.Where(i => i.categoryid == categoryid).Count() > 0).ToList();
                foreach (var order in itemViewModels)
                {
                    order.OrderItem = order.OrderItem.Where(item => item.categoryid == categoryid).ToList();
                }

            }
            if (status == "ready")
            {
                int count = 0;
                foreach (var order in itemViewModels)
                {
                    order.OrderItem = order.OrderItem.Where(i => i.Prepared > 0).ToList();
                    count++;
                }
                tableCount = count;
                itemViewModels = itemViewModels.Where(o => o.OrderItem.Count > 0).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new AddPaginationViewmodel<OrderDetailViewModel>
                {
                    Items = itemViewModels,
                    TotalCount = tableCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Category = category,
                    categoryid = categoryid,
                    status = status
                };
            }
            else
            {
                int count = 0;
                foreach (var order in itemViewModels)
                {
                    order.OrderItem = order.OrderItem.Where(item => item.Quantity > 0).ToList();
                    count++;
                }

                tableCount = count;
                itemViewModels = itemViewModels.Where(o => o.OrderItem.Count > 0).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new AddPaginationViewmodel<OrderDetailViewModel>
                {
                    Items = itemViewModels,
                    TotalCount = tableCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Category = category,
                    categoryid = categoryid,
                    status = status
                };
            }
        }



        // public OrderDetailViewModel GetOrderDetails(int id, string status)
        // {
        //     try
        //     {
        //         var ordered = _orderrepository.GetOrderDetails(id);
        //         if (status == "progress")
        //         {
        //             var result = new OrderDetailViewModel
        //             {
        //                 OrderId = ordered.OrderId,
        //                 OrderStatus = ordered.OrderStatus!,

        //                 OrderItem = ordered.OrderDetails.Where(c => !c.IsDeleted).Select(m => new OrderManyItem
        //                 {
        //                     OrderDetailId = m.OrderDetailId,
        //                     ItemName = m.ItemId != null ? m.Item.ItemName : string.Empty,
        //                     ItemId = m.ItemId,
        //                     Quantity = (int)(m.Quntity - m.Prepared)!,
        //                     Prepared = m.Prepared,
        //                     modifier = m.OrderedItemModifiers.Select(m => new ModifierViewModel
        //                     {
        //                         ModifierId = m.OrderedItemModifierId,
        //                         ModifierName = _orderrepository.GetModifierName(m.OrderedItemModifierId),
        //                         ModifierPrice = (int)m.Modifier.Rate!,
        //                     }).ToList(),
        //                 }).Where(c => c.Quantity > 0).ToList(),
        //             };
        //             return result;
        //         }
        //         else
        //         {
        //             var result = new OrderDetailViewModel
        //             {
        //                 OrderId = ordered.OrderId,
        //                 OrderStatus = ordered.OrderStatus!,

        //                 OrderItem = ordered.OrderDetails.Where(c => !c.IsDeleted).Select(m => new OrderManyItem
        //                 {
        //                     OrderDetailId = m.OrderDetailId,
        //                     ItemName = m.ItemId != null ? m.Item.ItemName : string.Empty,
        //                     ItemId = m.ItemId,
        //                     Quantity = (int)(m.Quntity - m.Prepared)!,
        //                     Prepared = m.Prepared,
        //                     modifier = m.OrderedItemModifiers.Select(m => new ModifierViewModel
        //                     {
        //                         ModifierId = m.OrderedItemModifierId,
        //                         ModifierName = _orderrepository.GetModifierName(m.OrderedItemModifierId),
        //                         ModifierPrice = (int)m.Modifier.Rate!,
        //                     }).ToList(),
        //                 }).Where(c => c.Prepared > 0).ToList(),
        //             };
        //             return result;
        //         }


        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception("Error in GetOrderDetails: " + ex.Message);
        //     }
        // }
        public OrderDetailViewModel GetOrderDetails(int id, string status)
        {
            try
            {
                var ordered = _orderrepository.GetOrderDetailsVM(id);
                if (status == "progress")
                {
                    var orders = ordered
                                   .Where(c => c.order_status == "Running")
                                   .GroupBy(c => c.order_id)
                                   .Select(orderGroup =>
                                       new OrderDetailViewModel
                                       {
                                           OrderId = orderGroup.Key,
                                           StartDate = orderGroup.FirstOrDefault()?.createdat ?? DateTime.Now,
                                           OrderComment = orderGroup.FirstOrDefault()?.order_comment ?? "",
                                           section = orderGroup.FirstOrDefault()?.section_name ?? "",
                                           TableList = orderGroup
                                               .Select(x => x.table_name ?? "")
                                               .Distinct()
                                               .ToList(),
                                           Assigntime = orderGroup.FirstOrDefault()?.table_modifiedat ?? DateTime.Now,
                                           OrderItem = orderGroup
                                               .Where(x => !(x.orderitem_isdelete ?? false))
                                               .GroupBy(x => x.orderdetail_id)
                                               .Select(itemGroup =>
                                               {
                                                   var first = itemGroup.FirstOrDefault();
                                                   return new OrderManyItem
                                                   {
                                                       OrderDetailId = first?.orderdetail_id ?? 0,
                                                       ItemName = first?.item_name ?? "",
                                                       ItemId = first?.item_id ?? 0,
                                                       ItemByComment = first?.item_comment ?? "",
                                                       categoryid = first?.categoryid ?? 0,
                                                       Quantity = (first?.quantity ?? 0) - (first?.prepared ?? 0),
                                                       Prepared = first?.prepared ?? 0,
                                                       modifier = itemGroup
                                                           .Where(x => x.ordered_item_modifier_id != null)
                                                           .GroupBy(x => x.ordered_item_modifier_id)
                                                           .Select(mg =>
                                                           {
                                                               var modifierRow = mg.FirstOrDefault();
                                                               return new ModifierViewModel
                                                               {
                                                                   ModifierId = mg.Key.Value,
                                                                   ModifierName = _orderrepository.GetModifierName(mg.Key.Value)
                                                               };
                                                           })
                                                           .ToList()
                                                   };
                                               }).Where(oi => oi.Quantity > 0)
                                               .ToList()
                                       }
                                   )
                                     .FirstOrDefault();

                    return orders!;

                }
                else
                {
                    var orders = ordered
                                .Where(c => c.order_status == "Running")
                                .GroupBy(c => c.order_id)
                                .Select(orderGroup =>
                                    new OrderDetailViewModel
                                    {
                                        OrderId = orderGroup.Key,
                                        StartDate = orderGroup.FirstOrDefault()?.createdat ?? DateTime.Now,
                                        OrderComment = orderGroup.FirstOrDefault()?.order_comment ?? "",
                                        section = orderGroup.FirstOrDefault()?.section_name ?? "",
                                        TableList = orderGroup
                                            .Select(x => x.table_name ?? "")
                                            .Distinct()
                                            .ToList(),
                                        Assigntime = orderGroup.FirstOrDefault()?.table_modifiedat ?? DateTime.Now,
                                        OrderItem = orderGroup
                                            .Where(x => !(x.orderitem_isdelete ?? false))
                                            .GroupBy(x => x.orderdetail_id)
                                            .Select(itemGroup =>
                                            {
                                                var first = itemGroup.FirstOrDefault();
                                                return new OrderManyItem
                                                {
                                                    OrderDetailId = first?.orderdetail_id ?? 0,
                                                    ItemName = first?.item_name ?? "",
                                                    ItemId = first?.item_id ?? 0,
                                                    ItemByComment = first?.item_comment ?? "",
                                                    categoryid = first?.categoryid ?? 0,
                                                    Quantity = (first?.quantity ?? 0) - (first?.prepared ?? 0),
                                                    Prepared = first?.prepared ?? 0,
                                                    modifier = itemGroup
                                                        .Where(x => x.ordered_item_modifier_id != null)
                                                        .GroupBy(x => x.ordered_item_modifier_id)
                                                        .Select(mg =>
                                                        {
                                                            var modifierRow = mg.FirstOrDefault();
                                                            return new ModifierViewModel
                                                            {
                                                                ModifierId = mg.Key.Value,
                                                                ModifierName = _orderrepository.GetModifierName(mg.Key.Value)
                                                            };
                                                        })
                                                        .ToList()
                                                };
                                            }).Where(oi => oi.Prepared > 0)
                                            .ToList()
                                    }
                                )
                                  .FirstOrDefault();

                    return orders!;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetOrderDetails: " + ex.Message);
            }
        }
        public bool UpdateOrderItemStatus(int orderid, string status, List<UpdateOrder> item)
        {
            //var order = _orderrepository.GetOrderDetails(orderid);
            var order = _orderrepository.GetOrderDetailsVM(orderid); ;

            foreach (var value in item)
            {
                var ordercard = order.Where(c => c.orderdetail_id == value.detailsid).First();
                if (status == "ready")
                {
                    ordercard.prepared += value.quantity;
                }
                else
                {
                    ordercard.prepared -= value.quantity;
                }
                _repository.UpdateOrderDetail(ordercard);
            }

            return true;
        }

        public WaitingViewModel GetAllSection()
        {
            try
            {
                var SectionList = _repository.GetAllSection();
                if (SectionList == null || !SectionList.Any())
                {
                    return new WaitingViewModel { SectionList = new List<SectionList>() };
                }
                var waitingViewModel = new WaitingViewModel
                {
                    SectionList = SectionList.Select(section => new SectionList
                    {
                        SectionId = section.SectionId,
                        SectionName = section.SectionName,
                        TableList = section.Tables.Where(c => !c.IsDelete).Select(table => new TableList
                        {
                            TableId = table.TableId,
                            TableName = table.TableName,
                            TableStatus = table.Status,
                            Capacity = table.Capacity,
                            Time = (DateTime)table.ModifiedAt,
                            Assigntime = (DateTime)table.ModifiedAt,
                            OrderId = table.OrderTableMappings.Where(c => !c.IsDelete).FirstOrDefault(o => o.TableId == table.TableId)?.OrderId,
                            orderAmount = table.OrderTableMappings.Where(c => !c.IsDelete).FirstOrDefault(o => o.TableId == table.TableId)?.Order.Amount
                        }).ToList()
                    }).ToList()
                };

                return waitingViewModel;

                // return new WaitingViewModel
                // {

                //        SectionList.Select(section => new SectionList
                //         {

                //             SectionId = section.SectionId,
                //             SectionName = section.SectionName,
                //             TableList = section.Tables.Where(c => !c.IsDelete).Select(table => new TableList
                //             {
                //                 TableId = table.TableId,
                //                 TableName = table.TableName,
                //                 TableStatus = table.Status,
                //                 Capacity = table.Capacity,
                //                 Time = (DateTime)table.ModifiedAt,
                //                 Assigntime = (DateTime)table.ModifiedAt,
                //                 OrderId = table.OrderTableMappings.Where(c => !c.IsDelete).FirstOrDefault(o => o.TableId == table.TableId)?.OrderId,
                //                 orderAmount = table.OrderTableMappings.Where(c => !c.IsDelete).FirstOrDefault(o => o.TableId == table.TableId)?.Order.Amount
                //             }).ToList()
                //         }).ToList()
                // };
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public int GetIsExist(string email)
        {

            var result = _repository.GetAllWaitingUser();
            var isExist = result.Any(c => c.Email.ToLower() == email.ToLower());
            if (isExist == true)
            {

                return 0;
            }
            else
            {
                var customerExist = _repository.GetWaitingUser(email);
                if (customerExist != null)
                {
                    if (customerExist.Orders.Count > 0)
                    {
                        return -1;
                    }
                }

                return 1;
            }
        }

        public WaitingUserDetails AddWaitingUser(WaitingUserDetails waiting)
        {
            try
            {
                var waitingUser = new WaitingTokenCode
                {
                    TotalPerson = waiting.No_of_Person,
                    UserName = waiting.UserName,
                    PhoneNo = waiting.Phone,
                    Email = waiting.Email,
                    SectionId = waiting.SectionId,
                    CreatedAt = DateTime.Now
                };
                _repository.AddWaitingUser(waitingUser);
                return waiting;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<WaitingUserDetails> GetAllWaitingUserBySectionId(int sectionId)
        {
            try
            {

                var Waiting1 = _repository.GetAllWaitingUser().Where(c => c.SectionId == sectionId && !c.IsDelete).ToList();
                if (Waiting1 == null || !Waiting1.Any())
                {
                    return new List<WaitingUserDetails>();
                }
                return Waiting1.Select(static waiting => new WaitingUserDetails
                {
                    WaitingUserId = waiting.TokenId,
                    No_of_Person = waiting.TotalPerson,
                    UserName = waiting.UserName,
                    Phone = waiting.PhoneNo,
                    Email = waiting.Email,
                    SectionId = waiting.SectionId,
                    CreateTime = waiting.CreatedAt,
                    WaitingTime = DateTime.Now - waiting.CreatedAt,

                }).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<WaitingUserDetails> GetAllWaitingUser()
        {
            try
            {
                var Waiting1 = _repository.GetAllWaitingUser();
                if (Waiting1 == null || !Waiting1.Any())
                {
                    return new List<WaitingUserDetails>();
                }
                return Waiting1.Select(static waiting => new WaitingUserDetails
                {
                    WaitingUserId = waiting.TokenId,
                    No_of_Person = waiting.TotalPerson,
                    UserName = waiting.UserName,
                    Phone = waiting.PhoneNo,
                    Email = waiting.Email,
                    SectionId = waiting.SectionId,
                    CreateTime = waiting.CreatedAt,

                }).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }

        public List<TableList> GetAllTableBySectionId(int sectionId)
        {
            try
            {
                var table = _repository.GetAllTableBySectionId(sectionId);
                return table.Select(table => new TableList
                {
                    TableId = table.TableId,
                    TableName = table.TableName,
                    TableStatus = table.Status,
                    Capacity = table.Capacity,
                    // Time=table.ModifiedAt

                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        public WaitingUserDetails CheckWaitingUser(string email)
        {
            var oldUser = _repository.GetWaitingUser(email);
            var waitingUser = new WaitingUserDetails();
            if (oldUser != null)
            {
                waitingUser.Email = oldUser.Email;
                waitingUser.UserName = oldUser.Name;
                waitingUser.Phone = oldUser.Phone;

                return waitingUser;
            }
            else
            {
                return waitingUser;
            }


        }

        public int AddAsCustomer(WaitingUserDetails waiting)
        {
            try
            {
                return _repository.AddAssignTable(waiting);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return 0; // or throw an exception, or handle it as needed
            }

        }

        // public int AddAsCustomer(WaitingUserDetails waiting)
        // {
        //     try
        //     {
        //         //Table Capacity
        //         var tablecapacity = 0;
        //         foreach (var table in waiting.SelectedTable)
        //         {
        //             var tableOrder = _repository.GetTableDetail(table);
        //             if (tableOrder.Status != "Available")
        //             {
        //                 return 0;
        //             }
        //             tablecapacity += (int)tableOrder.Capacity!;
        //         }

        //         if (tablecapacity < waiting.No_of_Person)
        //         {
        //             return -1;
        //         }

        //         // var email = waiting.Email;
        //         var OlderCutomer = _repository.GetAllCustomer(waiting.Email);
        //         if (OlderCutomer != null)
        //         {
        //             OlderCutomer.Name = waiting.UserName;
        //             OlderCutomer.Phone = waiting.Phone;
        //             OlderCutomer.Email = waiting.Email;
        //             OlderCutomer = _repository.AddUserAsCustomer(OlderCutomer);
        //         }
        //         else
        //         {
        //             var waitingUser = new Customer
        //             {

        //                 Name = waiting.UserName,
        //                 Phone = waiting.Phone,
        //                 Email = waiting.Email,

        //             };
        //             OlderCutomer = _repository.AddUserAsCustomer(waitingUser);
        //         }



        //         var order = new Order
        //         {
        //             CustomerId = OlderCutomer.CustomerId,
        //             OrderStatus = "Pending",
        //             TotalPerson = waiting.No_of_Person,

        //         };
        //         var orderId = _repository.AddOrder(order);

        //         var payment = new Payment
        //         {
        //             OrderId = orderId.OrderId,
        //             PaymentMethod = "",
        //         };
        //         _repository.AddPayment(payment);

        //         if (waiting.SelectedTable != null)
        //         {
        //             foreach (var table in waiting.SelectedTable)
        //             {
        //                 var tableOrder = new OrderTableMapping
        //                 {
        //                     TableId = table,
        //                     OrderId = orderId.OrderId,

        //                 };
        //                 _repository.AddTableOrder(tableOrder);
        //             }
        //         }

        //         if (waiting.SelectedTable != null)
        //         {
        //             foreach (var table in waiting.SelectedTable)
        //             {
        //                 var tableOrder = _repository.GetTableDetail(table);
        //                 tableOrder.Status = "Occupied";
        //                 tableOrder.ModifiedAt = DateTime.Now;
        //                 _repository.UpdateTable(tableOrder);
        //             }
        //         }

        //         //Update Wainting User
        //         if (waiting.WaitingUserId != 0)
        //         {
        //             var WaintingUser1 = _repository.GetWaitnigUserById(waiting.WaitingUserId);
        //             WaintingUser1.IsDelete = true;
        //             _repository.UpdateWaitingUser(WaintingUser1);
        //         }

        //         return order.OrderId;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
        //         return 0;
        //     }
        // }

        public List<SectionList> GetAllSectionList()
        {
            try
            {
                var section = _repository.GetAllSection();
                return section.Select(section => new SectionList
                {
                    SectionId = section.SectionId,
                    SectionName = section.SectionName
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }

        }



        public bool DeleteWatingToken(int id)
        {
            _repository.DeleteWatingToken(id);
            return true;
        }

        public WaitingUserDetails EditWaitingUser(int waitingUserid)
        {
            var detail = _repository.GetNewWaitnigUserAllDetail(waitingUserid);

            if (detail == null)
            {
                return null;
            }
            var UserDetail = detail.Select(static detail => new WaitingUserDetails
            {
                UserName = detail.UserName,
                Email = detail.Email,
                Phone = detail.PhoneNo,
                No_of_Person = detail.TotalPerson,
                WaitingUserId = detail.TokenId,
                SectionId = detail.SectionId
            }).FirstOrDefault();

            // var UserDetail = new WaitingUserDetails
            // {
            //     UserName = detail.UserName,
            //     Email = detail.Email,
            //     Phone = detail.PhoneNo,
            //     No_of_Person = detail.TotalPerson,
            //     WaitingUserId = detail.TokenId,
            //     SectionId = detail.SectionId
            // };




            return UserDetail;
        }

        public bool EditWaitingUser(WaitingUserDetails waiting)
        {
            try
            {
                if (waiting == null)
                {
                    return false;
                }
                var userDetailsList = _repository.GetNewWaitnigUserAllDetail(waiting.WaitingUserId);
                var userDetail = userDetailsList.FirstOrDefault();
                if (userDetail == null)
                {
                    return false;
                }
                userDetail.TotalPerson = waiting.No_of_Person;
                userDetail.UserName = waiting.UserName;
                userDetail.PhoneNo = waiting.Phone;
                userDetail.Email = waiting.Email;
                userDetail.SectionId = waiting.SectionId;
                //userDetail.ModifiedAt = DateTime.Now;
                userDetail.ModifiedAt = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

                _repository.EditWaitingUser(userDetail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        public List<Section> GetAllAnselectSection()
        {
            try
            {
                return _repository.GetAllSectionAnSelected();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }
        public List<Table> GetAllTable(int sectionid)
        {
            try
            {
                return _repository.GetAllTable(sectionid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return null;
            }
        }


        public int AssignTable(TableAndSectionViewModel waiting)
        {
            try
            {
                return _repository.AddAssignTable(waiting);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
                return 0;
            }
        }
        // public int AssignTable(TableAndSectionViewModel waiting)
        // {
        //     try
        //     {
        //         var WaitingUserDetails = _repository.GetWaitnigUserAllDetail(waiting.WaitingUserId ?? 0);
        //         var tablecapacity = 0;
        //         if (waiting.SelectedTablelist != null)
        //         {
        //             foreach (var table in waiting.SelectedTablelist)
        //             {
        //                 var tableOrder = _repository.GetTableDetail(table);
        //                 if (tableOrder.Status != "Available")
        //                 {
        //                     return 0;
        //                 }
        //                 tablecapacity += (int)tableOrder.Capacity!;
        //             }
        //             if (tablecapacity < WaitingUserDetails.TotalPerson)
        //             {
        //                 return 0;
        //             }
        //         }

        //         if (WaitingUserDetails.SectionId != waiting.sectionId)
        //         {
        //             WaitingUserDetails.SectionId = waiting.sectionId;
        //             _repository.UpdateWaitingUser(WaitingUserDetails);
        //         }



        //         var OlderCutomer = _repository.GetAllCustomer(WaitingUserDetails.Email);
        //         if (OlderCutomer != null)
        //         {
        //             OlderCutomer.Name = WaitingUserDetails.UserName;
        //             OlderCutomer.Phone = WaitingUserDetails.PhoneNo;
        //             OlderCutomer.Email = WaitingUserDetails.Email;
        //             OlderCutomer = _repository.AddUserAsCustomer(OlderCutomer);
        //         }
        //         else
        //         {
        //             var waitingUser = new Customer
        //             {

        //                 Name = WaitingUserDetails.UserName,
        //                 Phone = WaitingUserDetails.PhoneNo,
        //                 Email = WaitingUserDetails.Email,

        //             };
        //             OlderCutomer = _repository.AddUserAsCustomer(waitingUser);
        //         }


        //         var order = new Order
        //         {
        //             CustomerId = OlderCutomer.CustomerId,
        //             OrderStatus = "Pending",
        //             TotalPerson = WaitingUserDetails.TotalPerson,
        //             Amount = 0,
        //         };
        //         var neworder = _repository.AddOrder(order);

        //         var payment = new Payment
        //         {
        //             OrderId = neworder.OrderId,
        //             PaymentMethod = "",
        //         };
        //         _repository.AddPayment(payment);

        //         if (waiting.SelectedTablelist != null)
        //         {
        //             foreach (var table in waiting.SelectedTablelist)
        //             {
        //                 var tableOrder = new OrderTableMapping
        //                 {
        //                     TableId = table,
        //                     OrderId = neworder.OrderId,

        //                 };
        //                 _repository.AddTableOrder(tableOrder);
        //             }
        //         }

        //         if (waiting.SelectedTablelist != null)
        //         {
        //             foreach (var table in waiting.SelectedTablelist)
        //             {
        //                 var tableOrder = _repository.GetTableDetail(table);
        //                 tableOrder.Status = "Occupied";
        //                 _repository.UpdateTable(tableOrder);
        //             }
        //         }

        //         // //Update Wainting User
        //         WaitingUserDetails.IsDelete = true;
        //         _repository.UpdateWaitingUser(WaitingUserDetails);
        //         return order.OrderId;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
        //         return 0;
        //     }
        // }
        public List<AddItemViewModel> GetAllItemList(string search)
        {
            var items = _repository.GetAllItem();

            var itemViewModels = items.Select(c => new AddItemViewModel
            {
                CategoryId = c.CategoryId,
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                IsAvailable = c.IsAvailable,
                FoodType = c.FoodType,
                shortcode = c.Shortcode,
                Descriptionitem = c.Description,
                Quantity = c.Quantity,
                Price = c.Price,
                Image = c.Image,
                IsFav = c.IsFav,
                Tax = c.Tax,
                IsDefault = c.IsDefault
            }).ToList();



            if (!string.IsNullOrEmpty(search))
            {
                itemViewModels = itemViewModels.Where(u => u.ItemName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return itemViewModels;
        }
        public List<AddItemViewModel> GetAllItemFav(string search)
        {
            var items = _repository.GetAllItem();

            var itemViewModels = items.Where(c => c.IsFav == true).Select(c => new AddItemViewModel
            {
                CategoryId = c.CategoryId,
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                IsAvailable = c.IsAvailable,
                FoodType = c.FoodType,
                shortcode = c.Shortcode,
                Descriptionitem = c.Description,
                Quantity = c.Quantity,
                Price = c.Price,
                Image = c.Image,
                IsFav = c.IsFav,
                IsDefault = c.IsDefault
            }).ToList();



            if (!string.IsNullOrEmpty(search))
            {
                itemViewModels = itemViewModels.Where(u => u.ItemName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return itemViewModels;
        }

        public bool Addfavorite(int itemId)
        {
            var item = _categoryrepository.GetItem(itemId);

            if (item.IsFav == true)
            {
                item.IsFav = false;

                _categoryrepository.EditItem(item);
                return false;
            }
            else
            {
                item.IsFav = true;
                _categoryrepository.EditItem(item);
                return true;
            }

        }

        public List<AddItemViewModel> GetAllItem(int categoryId, string search)
        {
            var items = _repository.GetAllItemForMenuApp(categoryId);

            var itemViewModels = items.Select(c => new AddItemViewModel
            {
                CategoryId = c.CategoryId,
                ItemId = c.ItemId,
                ItemName = c.ItemName,
                IsAvailable = c.IsAvailable,
                FoodType = c.FoodType,
                shortcode = c.Shortcode,
                Descriptionitem = c.Description,
                Quantity = c.Quantity,
                Price = c.Price,
                Image = c.Image,
                IsFav = c.IsFav,
                IsDefault = c.IsDefault
            }).ToList();



            if (!string.IsNullOrEmpty(search))
            {
                itemViewModels = itemViewModels.Where(u => u.ItemName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return itemViewModels;
        }


        public List<AddItemViewModel> GetAllModifier(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return null;
                }

                var item = _repository.GetAllModifier(id);

                if (item == null)
                {
                    return null;
                }

                var modifier = item.Select(c => c.MappingItemModifierGroups.Select(c => new DataItem
                {
                    Id = c.ModifierGroup.ModifierGroupId,
                    Min = c.MinValue ?? 0,
                    Max = c.MaxValue ?? 0,
                    Name = c.ModifierGroup.GroupName,
                    ModifierItem = c.ModifierGroup.MappingModifierModifiergroups.Where(c => c.Modifier.ModifierId == c.ModifierId).Select(a => new AddModifierViewModel
                    {
                        ModifierId = a.ModifierId,
                        ModifierName = a.Modifier.ModifierName,
                        Rate = a.Modifier.Rate

                    }).ToList()

                }).ToList()).ToList();




                var ModifierDetail = item.Select(c => new AddItemViewModel
                {
                    ItemId = c.ItemId,
                    ItemName = c.ItemName,
                    Price = c.Price,
                    ModifierGroups = modifier[0],

                }).ToList();



                return ModifierDetail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} , StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public CustomerDetailViewModel EditCustomerDetail(int Customerid)
        {
            var detail = _repository.GetCustomerDetail(Customerid);

            var UserDetail = new CustomerDetailViewModel
            {
                CustomerName = detail.Name,
                CustomerEmail = detail.Email,
                CustomerPhone = detail.Phone,
                TotalPerson = detail.Orders.Where(o => o.CustomerId == Customerid).First().TotalPerson,
                CustomerId = detail.CustomerId,
                Orderid = detail.Orders.Where(o => o.CustomerId == Customerid).First().OrderId

            };
            return UserDetail;
        }



        public int EditCustomerDetail(CustomerDetailViewModel modal)
        {
            try
            {
                var Customerdetail = _repository.GetCustomerDetail(modal.CustomerId);
                var OrderDetailData = _repository.GetOrderDetail(modal.Orderid);
                var TableCapacity = OrderDetailData.OrderTableMappings.Where(o => o.OrderId == modal.Orderid).Sum(o => o.Table.Capacity);

                if (TableCapacity < modal.TotalPerson)
                {
                    return -1;
                }
                Customerdetail.Name = modal.CustomerName;
                Customerdetail.Phone = modal.CustomerPhone;
                Customerdetail.Email = modal.CustomerEmail;
                var result = _repository.UpdateCustomer(Customerdetail);

                if (OrderDetailData.TotalPerson != modal.TotalPerson)
                {
                    OrderDetailData.TotalPerson = modal.TotalPerson ?? 0;
                    var result1 = _repository.UpdateOrder(OrderDetailData);
                }

                return 1;
            }
            catch
            {
                return 0;
            }


        }


        public bool AddOrderComment(OrderDetailViewModel modal)
        {
            var OrderDetailData = _repository.GetOrderDetail(modal.OrderId);
            OrderDetailData.OrderComment = modal.OrderComment;
            var result = _repository.UpdateOrder(OrderDetailData);
            if (result == true)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool AddItemComment(OrderDetailViewModel modal)
        {

            var OrderDetailData = _repository.GetOrder(modal.DetailId);
            if (OrderDetailData != null)
            {
                if (OrderDetailData.ItemComment == null)
                {
                    OrderDetailData.ItemComment = modal.ItemComment;
                }
            }

            // OrderDetailData.ItemComment = OrderDetailData.ItemComment;
            var result = _repository.UpdateOrderDetail(OrderDetailData);
            if (result == true)
            {
                return true;
            }
            else
            {
                return false;
            }

        }







        public int AddOrderItem(int orderId, float TotalAmount, List<AddOrderViewModel> model)
        {

            try
            {
              return _repository.SaveOrder(orderId, TotalAmount, model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
        // public int AddOrderItem(int orderId, float TotalAmount, List<AddOrderViewModel> model)
        // {

        //     try
        //     {
        //         var oldOrderDetails = _repository.GetOrderDetailList(orderId);



        //         var Order1 = _repository.GetOrderDetail(orderId);

        //         if (Order1.OrderStatus == "Complete" || Order1.OrderStatus == "Cancelled")
        //         {
        //             return -1;
        //         }
        //         Order1.OrderStatus = "Running";
        //         Order1.Amount = TotalAmount;
        //         _repository.UpdateOrder(Order1);

        //         var tableid = Order1.OrderTableMappings.Where(c => c.OrderId == orderId).Select(c => c.TableId);
        //         foreach (var table in tableid)
        //         {
        //             var gettable = _sectionRepository.GetTable(table);
        //             gettable.Status = "Running";
        //             gettable.ModifiedAt = DateTime.Now;
        //             _sectionRepository.EditTable(gettable);
        //         }


        //         if (oldOrderDetails != null && oldOrderDetails.Any())
        //         {
        //             var oldDetailDict = oldOrderDetails.ToDictionary(x => x.OrderDetailId, x => x);


        //             foreach (var item in model)
        //             {
        //                 if (oldDetailDict.ContainsKey(item.Id))
        //                 {
        //                     var detail = oldDetailDict[item.Id];
        //                     detail.Quntity = item.Quantity;
        //                     _repository.UpdateOrderDetail(detail);
        //                     oldDetailDict[item.Id].IsDeleted = false;
        //                 }
        //                 else
        //                 {

        //                     var newDetail = new OrderDetail
        //                     {
        //                         ItemId = item.Id,
        //                         Quntity = item.Quantity,
        //                         Prepared = 0,
        //                         OrderId = orderId,
        //                         OrderedItemModifiers = item.Modifiers.Select(s => new OrderedItemModifier
        //                         {
        //                             ModifierId = s.Id,
        //                         }).ToList()
        //                     }
        //                 ;
        //                     _repository.AddNewOrderitem(newDetail);
        //                 }
        //             }

        //             var newItemIds = model.Select(m => m.Id).ToHashSet();
        //             foreach (var oldDetail in oldOrderDetails)
        //             {
        //                 if (!newItemIds.Contains(oldDetail.OrderDetailId))
        //                 {
        //                     oldDetail.IsDeleted = true;
        //                     _repository.UpdateOrderDetail(oldDetail);
        //                 }
        //             }
        //             return 0;
        //         }
        //         else
        //         {

        //             foreach (var item in model)
        //             {
        //                 var newDetail = new OrderDetail
        //                 {
        //                     ItemId = item.Id,
        //                     Quntity = item.Quantity,
        //                     Prepared = 0,
        //                     OrderId = orderId,
        //                     OrderedItemModifiers = item.Modifiers.Select(s => new OrderedItemModifier
        //                     {
        //                         ModifierId = s.Id,
        //                     }).ToList()
        //                 };
        //                 _repository.AddNewOrderitem(newDetail);
        //             }
        //             return 0;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //         return 1;
        //     }
        // }


        public OrderDetailViewModel GetOrderDetails(int id)
        {
            try
            {


                var ordered = _orderrepository.GetOrderDetails(id);



                var result = new OrderDetailViewModel
                {
                    // InvoiceId = ordered.Invoices.FirstOrDefault()?.InvoiceId ?? 0,

                    OrderId = id,
                    StartDate = ordered.CreatedAt,
                    EndDate = ordered.ModifiedAt ?? DateTime.Now,
                    OrderStatus = ordered.OrderStatus ?? string.Empty,
                    CustomerId = ordered.CustomerId,
                    CustomerName = ordered.Customer.Name,
                    CustomerEmail = ordered.Customer.Email ?? string.Empty,
                    CustomerPhone = ordered.Customer.Phone,
                    TotalPerson = ordered.TotalPerson,
                    OderAmount = ordered.Amount,
                    PaymentMode = _repository.CheckPayment(id)?.PaymentMethod ?? string.Empty,

                    SectionName = ordered.OrderTableMappings.Select(o => o.Table.Section.SectionName).First(),

                    OrderItem = ordered.OrderDetails.Where(c => !c.IsDeleted).Select(m => new OrderManyItem
                    {
                        OrderDetailId = m.OrderDetailId,
                        ItemName = m.ItemId != null ? m.Item.ItemName : string.Empty,
                        ItemId = m.ItemId,
                        Quantity = m.Quntity,
                        Price = m.Item.Price,
                        OtherTax = m.Item.Tax,


                        modifier = m.OrderedItemModifiers.Select(m => new ModifierViewModel
                        {
                            // ModifierName = m.Modifier.ModifierName,
                           // ModifierId = m.OrderedItemModifierId,
                           ModifierId = m.ModifierId,
                            ModifierName = _orderrepository.GetModifierName(m.OrderedItemModifierId),
                            ModifierPrice = _orderrepository.GetModifierPrice(m.OrderedItemModifierId),
                            //Quantity = m.Quantity
                        }).ToList(),
                    }).ToList(),

                    ManyTableList = _orderrepository.TableMappping(id),

                    tax = _orderrepository.AllTaxForOrder().Select(c => new AddTaxViewModel
                    {
                        TaxName = c.TaxName,
                        TaxId = c.TaxId,
                        TaxType = c.TaxType,
                        IsDefault = c.IsDefault,
                        IsEnable = c.IsEnabled,
                        TaxAmount = c.TaxValue,
                    }).ToList()
                };



                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetOrderDetails: " + ex.Message);
            }
        }


        public int ConfirmationOrder(int orderid)
        {
            var order = _repository.GetOrderDetailList(orderid);

            if (order == null || !order.Any())
            {
                return -1;
            }

            foreach (var orderitem in order)
            {
                if (orderitem.Prepared != orderitem.Quntity)
                {
                    return 1;
                }
            }

            return 0;
        }
        public bool ConfirmationOrderStatus(ConfirmationOrder model)
        { 
            return _repository.ConfirmationOrderStatus(model);
        }



        // public bool ConfirmationOrderStatus(ConfirmationOrder model)
        // {
        //     var OrderDetailData = _repository.GetOrderDetail(model.orderid);
        //     OrderDetailData.OrderStatus = "Complete";
        //     OrderDetailData.Amount = model.totalbill;
        //     OrderDetailData.SubTotal = model.subtotal;
        //     OrderDetailData.OtherTax = model.othertax;
        //     var result = _repository.UpdateOrder(OrderDetailData);

        //     var tablelist = OrderDetailData.OrderTableMappings.Where(c => c.OrderId == model.orderid).Select(c => c.TableId);

        //     foreach (var table in tablelist)
        //     {
        //         var mappingtable = _repository.GetTableMapping(table);
        //         mappingtable.IsDelete = true;
        //         _repository.UpdatetableMapping(mappingtable);
        //     }

        //     foreach (var table in tablelist)
        //     {
        //         var tabledata = _sectionRepository.GetTable(table);
        //         tabledata.Status = "Available";
        //         _sectionRepository.EditTable(tabledata);

        //     }
        //     var taxlist = _repository.GetTaxMpping(model.orderid).Select(c => c.TaxId).ToList();
        //     if (model.selectedTaxes != null)
        //     {
        //         foreach (var item in model.selectedTaxes)
        //         {
        //             if (taxlist.Contains(item.taxid))
        //             {

        //                 var existingTax = _repository.GetOrderTaxMapping(model.orderid, item.taxid);
        //                 if (existingTax != null)
        //                 {
        //                     existingTax.TaxAmount = item.taxAmount;
        //                     _repository.UpdateOrderTaxMapping(existingTax);
        //                 }
        //             }
        //             else
        //             {
        //                 var newDetail = new OrderTaxMapping
        //                 {
        //                     TaxId = item.taxid,
        //                     TaxAmount = item.taxAmount,
        //                     OrderId = model.orderid
        //                 };
        //                 _repository.AddOrderTaxMapping(newDetail);
        //             }
        //         }
        //     }



        //     if (result == true)
        //     {
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }

        public OrderDetail OrderUpdate(int id)
        {
            var prepareitem = _repository.GetDetail(id);
            return prepareitem;
        }
        public string AddItemCheckOrderStatus(int id)
        {
            var status = _repository.GetOrderDetail(id).OrderStatus;
            if (status == null)
            {
                return "Empty";
            }
            return status;
        }
        public bool SavePaymetMethod(int id, string method)
        {
            // var status = _repository.GetOrderDetail(id).Payments.Where(c => c.OrderId == id).;
            var result = _repository.CheckPayment(id);

            if (result != null)
            {
                result.PaymentMethod = method;
                result.PaymentDate = DateTime.Now;

                _repository.UpdatePayment(result);

            }
            else
            {
                var PaymentMethod = new Payment
                {
                    PaymentMethod = method,
                    OrderId = id,
                    PaymentDate = DateTime.Now
                };
                _repository.AddPayment(PaymentMethod);
            }


            return true;
        }

        public int CancelOrder(int orderid)
        {
            var order = _repository.GetOrderDetailList(orderid);
            if (order != null && order.Any())
            {
                foreach (var item in order)
                {
                    if (item.Prepared > 0)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }


        public int ConfirmationCancel(int orderid)
        {
            return _repository.ConfirmationCancel(orderid);
        }
        // public int ConfirmationCancel(int orderid)
        // {
        //     var order = _repository.GetOrderDetailList(orderid);
        //     var OrderDetailData = _repository.GetOrderDetail(orderid);
        //     var tablelist = OrderDetailData.OrderTableMappings.Where(c => c.OrderId == orderid).Select(c => c.TableId);

        //     if (order != null)
        //     {
        //         foreach (var item in order)
        //         {
        //             if (item.Prepared > 0)
        //             {
        //                 return 1;
        //             }
        //         }

        //         foreach (var orderitem in order)
        //         {
        //             orderitem.IsDeleted = true;
        //             _repository.UpdateOrderDetail(orderitem);
        //         }

        //         foreach (var table in tablelist)
        //         {
        //             var tabledata = _sectionRepository.GetTable(table);
        //             tabledata.Status = "Available";
        //             _sectionRepository.EditTable(tabledata); ;
        //         }
        //         foreach (var table in tablelist)
        //         {
        //             var mappingtable = _repository.GetTableMapping(table);
        //             mappingtable.IsDelete = true;
        //             _repository.UpdatetableMapping(mappingtable);
        //         }

        //         OrderDetailData.OrderStatus = "Cancelled";
        //         OrderDetailData.Amount = 0;
        //         OrderDetailData.SubTotal = 0;
        //         _repository.UpdateOrder(OrderDetailData);
        //     }
        //     return 0;

        // }


        public bool checkStatus(int orderid)
        {
            var ordered = _orderrepository.GetOrderDetails(orderid);

            bool isFullyPrepared = ordered.OrderDetails.All(od => od.Prepared >= od.Quntity);

            if (isFullyPrepared)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CustomerReview(CustomerReviewViewModel model)
        {

            try
            {
                var addFeedback = new Feedback
                {
                    OrderId = model.orderid,
                    CustomerId = model.CustomerId,
                    Comments = model.OrderComment,
                    FoodRating = model.FoodRating,
                    ServiceRating = model.ServiceRating,
                    AmbienceRating = model.AmbienceRating,
                    Rating = ((model.FoodRating + model.ServiceRating + model.AmbienceRating) / 3)


                };

                _repository.AddFeedback(addFeedback);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }

}