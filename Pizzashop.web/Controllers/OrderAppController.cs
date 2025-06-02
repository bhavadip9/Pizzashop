
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Pizzashop.entity.ViewModels;
using PizzaShop.service.Implementation;
using PizzaShop.service.Interfaces;

namespace Pizzashop.web.Controllers;

public class OrderAppController : Controller
{
    private readonly IKotService _kotService;

    private readonly ISectionService _sectionService;

    private readonly IHubContext<OrderHub> _hubContext;

    private readonly IOrderService _orderService;
    private readonly ICategoryService _categoryService;
    public OrderAppController(IKotService kotService, ICategoryService categoryService, ISectionService sectionService, IOrderService orderService, IHubContext<OrderHub> hubContext)
    {
        _categoryService = categoryService;
        _kotService = kotService;
        _sectionService = sectionService;
        _orderService = orderService;
        _hubContext = hubContext;
    }

    [CustomAuthorize("Manager", "Chef")]
    public IActionResult Index()
    {
        var category = _kotService.GetAllCategoryList();

        return View(category);
    }


    [HttpGet]
    public async Task<IActionResult> CategoryWithOrder(int categoryId, string status, int page, int pageSize)
    {
        if (categoryId == 0)
        {
            var order = await _kotService.GetAllOrder(status, page, pageSize);
            ViewBag.Order_id = 0;
            order.status = status;
            return PartialView("OrderDetail", order);
        }
        else
        {
            var order = await _kotService.CategoryWithOrder(categoryId, page, pageSize, status);
            order.status = status;
            return PartialView("OrderDetail", order);
        }
    }

    public IActionResult ShowOrderUpdate(int orderid, string status)
    {

        var details = _kotService.GetOrderDetails(orderid, status);
        ViewBag.status = status;
        return PartialView("_OrderUpdate", details);
    }


    [HttpPost]
    public IActionResult UpdateOrderItem(int orderId, string status, List<UpdateOrder> items)
    {
        var value = _kotService.UpdateOrderItemStatus(orderId, status, items);
        if (value)
        {

            if (status == "ready")
            {
                //  TempData["success"] = "Successfully, Mark As Progress";
                return Json(new { success = true, message = "Successfully, Mark As Prepared" });
            }
            else if (status == "progress")
            {
                //TempData["success"] = "Successfully, Mark As Prepared";
                return Json(new { success = true, message = "Successfully, Mark As Progress" });
            }


            return Json(new { success = true, message = "Successfully, Update Order" });
        }
        else
        {
            return Json(new { success = false, message = "Something went wrong, try again!" });
        }
    }

    #region  OrderApp Table

    public IActionResult GetAllSection()
    {
        var section = _sectionService.GetAllSectionList();
        return Json(section);
    }

    public IActionResult WaitingTable()
    {
        var Section = _kotService.GetAllSection();
        return View("TableAssign/WaitingTable", Section);
    }

    [HttpGet]
    public IActionResult AddWaitingUser()
    {
        try
        {
            return PartialView("TableAssign/_AddWaitingUser");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public IActionResult CheckEmail(string email)
    {
        var UserData = _kotService.CheckWaitingUser(email);
        if (UserData != null)
        {
            return Json(UserData);
        }
        else
        {
            return null;
        }
    }
    [HttpPost]
    public IActionResult AddWaitingUser(WaitingUserDetails waiting)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var isExist = _kotService.GetIsExist(waiting.Email);
                if (isExist == 0)
                {
                    TempData["error"] = "Already Exist !";
                    return RedirectToAction("WaitingTable");
                }
                else if (isExist == -1)
                {
                    TempData["error"] = "Already Order Running !";
                    return RedirectToAction("WaitingTable");
                }
                var result = _kotService.AddWaitingUser(waiting);
                if (result != null)
                {

                    TempData["success"] = "Successfully, Added Waiting User";
                    return RedirectToAction("WaitingTable");

                }
                else
                {
                    TempData["error"] = "Error, Added Waiting User";
                    return RedirectToAction("WaitingTable");

                }
            }
            else
            {
                TempData["error"] = "Error, Added Waiting User";
                return RedirectToAction("WaitingTable");
            }
            //return PartialView("_AddWaitingUser");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    [HttpPost]


    [HttpPost]
    public IActionResult AddAssignSidebar(int sectionId, List<int> tableIds)
    {
        try
        {
            var waiting = _kotService.GetAllWaitingUser();

            var model = new AddWaitingAssignViewModel
            {
                SectionId = sectionId,
                WaitingUserList = waiting,
                SelectedTable = tableIds
            };
            return PartialView("TableAssign/_AddWaitingAssign", model);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public IActionResult GetAllTableBySectionId(int sectionId)
    {
        try
        {
            var table = _kotService.GetAllTableBySectionId(sectionId);
            return Json(table);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }



    [HttpPost]
    public IActionResult AddAsCustomer(WaitingUserDetails waiting)
    {
        try
        {
            // ModelState.Remove("WaitingUserId");
            // if (ModelState.IsValid)
            // {
            var result = _kotService.AddAsCustomer(waiting);

            if (result > 0)
            {
                return Json(new { success = true, orderId = result, message = "Successfully, Assign Table" });
            }
            if (result == -1)
            {
                return Json(new { success = false, orderId = result, message = "Error, Table Capacity" });
            }
            else
            {
                return Json(new { success = false, orderId = result, message = "Error, Not Assign Table" });
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion

    #region  Waiting List
    public IActionResult WaitingListPage()
    {
        var waiting = _kotService.GetAllWaitingUser();

        var section = _kotService.GetAllSectionList();

        var model = new AddWaitingAssignViewModel
        {
            WaitingUserList = waiting,
            SectionList = section

        };
        return View("WaitingList/WaitingListPage", model);
    }

    public IActionResult GetAllWaitingUser()
    {
        var waiting = _kotService.GetAllWaitingUser();
        return PartialView("WaitingList/_WaitingListTable", waiting);

    }

    public IActionResult GetSectionByWaitingList(int sectionid)
    {
        var waitingUser = _kotService.GetAllWaitingUserBySectionId(sectionid);
        return PartialView("WaitingList/_WaitingListTable", waitingUser);

    }
    public IActionResult _AddUserInWaitingList(WaitingUserDetails waiting)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var isExist = _kotService.GetIsExist(waiting.Email);
                if (isExist == 0)
                {
                    TempData["error"] = "Already Exist !";
                    return RedirectToAction("WaitingListPage");
                }
                else if (isExist == -1)
                {
                    TempData["error"] = "Already Order Running";
                    return RedirectToAction("WaitingListPage");
                }
                else
                {
                    var result = _kotService.AddWaitingUser(waiting);
                    if (result != null)
                    {
                        TempData["success"] = "Successfully, Added Waiting User";
                        return RedirectToAction("WaitingListPage");

                    }
                    else
                    {
                        TempData["error"] = "Error, Added Waiting User";
                        return RedirectToAction("WaitingListPage");

                    }
                }
            }
            else
            {
                TempData["error"] = "Error, Added Waiting User";
                return RedirectToAction("WaitingList/_AddUserInWaitingList");
            }
            //return PartialView("_AddWaitingUser");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public IActionResult DeleteWatingUser(int WaitingUserId)
    {
        var value = _kotService.DeleteWatingToken(WaitingUserId);

        if (value == true)
        {
            TempData["success"] = "Delete Waiting User";
        }
        else
        {
            TempData["error"] = "Not Delete ,try Again";
        }
        return RedirectToAction("WaitingListPage");
    }


    // [HttpPost]
    // public IActionResult TableSectionAssignPost(TableAndSectionViewModel model)
    // {
    //     try
    //     {
    //         if (ModelState.IsValid)
    //         {
    //             var result = _kotService.AssignTable(model);
    //             if (result != 0)
    //             {
    //                 return Json(new { success = true, orderId = result, message = "Successfully, Assign Table" });
    //             }
    //             else
    //             {
    //                 return Json(new { success = false, orderId = result, message = "Error, Not Assign Table" });
    //             }
    //         }
    //         else
    //         {
    //             return Json(new { success = false, orderId = 0, message = "Something with wrong try again!" });
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         throw;
    //     }
    // }

    [HttpGet]
    public IActionResult EditWaitingUser(int WaitingUserId)
    {
        var WaitingUser = _kotService.EditWaitingUser(WaitingUserId);
        return PartialView("WaitingList/_EditWaitingUser", WaitingUser);
    }

    [HttpPost]
    public IActionResult EditWaitingUser(WaitingUserDetails userDetails)
    {


        try
        {
            if (ModelState.IsValid)
            {
                var result = _kotService.EditWaitingUser(userDetails);
                if (result == true)
                {
                    TempData["success"] = "Successfully, Edit Waiting User";
                    return RedirectToAction("WaitingListPage");

                }
                else
                {
                    TempData["error"] = "Error, Edit Waiting User";
                    return RedirectToAction("WaitingListPage");

                }
            }
            else
            {
                TempData["error"] = "Error, Edit Waiting User";
                return RedirectToAction("WaitingListPage");
            }
            //return PartialView("_AddWaitingUser");
        }
        catch (Exception ex)
        {
            throw ex;
        }
       ;
    }

    #endregion


    #region  order Menu

    public IActionResult MenuApp(int id)
    {

        var category = _kotService.GetAllCategoryList();
        ViewBag.Order_id = id;
        return View("MenuApp/MenuApp", category);
    }

    public IActionResult TableSectionAssign(int WaitingUserId, int sectionId, string Email, string UserName, int TotalPerson, string Phone)
    {

        var model = new WaitingUserDetails();
        model.WaitingUserId = WaitingUserId;
        model.SectionId = sectionId;
        model.Email = Email;
        model.UserName = UserName;
        model.No_of_Person = TotalPerson;
        model.Phone = Phone;
        return PartialView("WaitingList/_AssignTableAndSection", model);
    }



    public IActionResult GetAllAnselectSection()
    {
        var section = _kotService.GetAllAnselectSection();
        return Json(section);
    }
    [HttpPost]
    public IActionResult GetAllTableBySection(int sectionId)
    {
        var table = _kotService.GetAllTable(sectionId);
        return Json(table);
    }


    public IActionResult GetAllItemByCategory(int categoryId, string search, int orderId)
    {
        try
        {
            if (categoryId == 0)
            {
                var itemAll = _kotService.GetAllItemList(search);
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.orderId = orderId;

                return PartialView("MenuApp/ShowItem", itemAll);

            }
            else if (categoryId == -1)
            {
                var itemsFav = _kotService.GetAllItemFav(search);
                ViewBag.SelectedCategoryId = categoryId;
                ViewBag.orderId = orderId;
                return PartialView("MenuApp/ShowItem", itemsFav);
            }
            var items = _kotService.GetAllItem(categoryId, search);
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.orderId = orderId;
            return PartialView("MenuApp/ShowItem", items);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error in GetItemsByCategory: {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }

    public IActionResult ModifierByItem(int itemid, string foodtype, int TaxAmount, int orderId)
    {
        var modifier = _kotService.GetAllModifier(itemid);
        @ViewBag.TaxAmount = TaxAmount;
        @ViewBag.foodtype = foodtype;
        @ViewBag.orderId = orderId;
        if (modifier != null && modifier.Any())
        {
            var ModifierItems = modifier.Any(mg => mg.ModifierGroups != null && mg.ModifierGroups.Any(g => g.ModifierItem != null && g.ModifierItem.Any()));

            if (ModifierItems)
            {
                return PartialView("MenuApp/ShowModifier", modifier);
            }
        }
        return Json(new { success = false, message = "Add item in Order" });
    }


    public IActionResult OrderBillPage(int id)
    {
        var Model = _kotService.GetOrderDetails(id);
        return PartialView("MenuApp/_ShowBill", Model);

    }


    public IActionResult AddFavorito(int itemId)
    {
        var result = _kotService.Addfavorite(itemId);
        if (result == true)
        {
            return Json(new { success = true, message = "Add Successfully in favorite item." });
        }
        else
        {
            return Json(new { success = false, message = "Remove Successfully from favorite item" });
        }
    }

    [HttpGet]
    public IActionResult CustomerDetail(int customerid)
    {
        var Customer = _kotService.EditCustomerDetail(customerid);
        return PartialView("MenuApp/_CustomerDetail", Customer);
    }

    [HttpPost]
    public IActionResult CustomerDetail(CustomerDetailViewModel model)
    {
        var Customer = _kotService.EditCustomerDetail(model);
        if (Customer == 1)
        {
            return Json(new { success = true, orderId = model.Orderid, message = "Successfully, Edit Customer" });

        }
        else if (Customer == -1)
        {
            return Json(new { success = false, orderId = model.Orderid, message = "Table Capacity Full" });
        }
        else
        {
            return Json(new { success = false, model.Orderid, message = "Something with wrong !" });
        }
        // return RedirectToAction("MenuPage");
    }

    [HttpGet]
    public IActionResult OrderWiseComment(int orderid)
    {
        var OrderDetail = new OrderDetailViewModel();
        OrderDetail.OrderId = orderid;
        return PartialView("MenuApp/_OrderWiseComment", OrderDetail);
    }




    [HttpPost]
    public IActionResult OrderWiseComment(OrderDetailViewModel model)
    {
        var order = _kotService.AddOrderComment(model);
        if (order == true)
        {
            return Json(new { success = true, orderId = model.OrderId, message = "Successfully, Add Comment" });

        }
        else
        {
            return Json(new { success = false, model.OrderId, message = "Something with wrong !" });
        }
    }

    [HttpGet]
    public IActionResult ItemWiseComment(int detailsid)
    {
        var OrderDetail = new OrderDetailViewModel();
        OrderDetail.DetailId = detailsid;
        return PartialView("MenuApp/_ItemWiseComment", OrderDetail);
    }

    [HttpPost]
    public IActionResult ItemWiseComment(OrderDetailViewModel model)
    {
        var order = _kotService.AddItemComment(model);
        if (order == true)
        {
            return Json(new { success = true, orderId = model.OrderId, message = "Add item Instruction" });

        }
        else
        {
            return Json(new { success = false, model.OrderId, message = "Something with wrong !" });
        }
    }

    [HttpPost]
    public IActionResult ConfirmationOrder(int orderid)
    {

        var ordercomplete = _kotService.ConfirmationOrder(orderid);

        if (ordercomplete == 0)
        {
            var OrderDetail = new OrderDetailViewModel();
            OrderDetail.OrderId = orderid;
            //  OrderDetail.CustomerId = model.customerid;
            return PartialView("MenuApp/_CompleteConfirmation", OrderDetail);
        }
        else if (ordercomplete == 1)
        {
            TempData["error"] = "Your Order is Not Complete";
            return Json(new { success = false, message = "Your Order is Not Complete" });

        }
        else
        {
            TempData["error"] = "Your Order is Not Complete";
            return Json(new { success = false, message = "Your Order is Not Complete" });


        }

    }



    public IActionResult SaveOrder(string dataList, int id, float TotalAmount)
    {
        //  var order = JsonConvert.DeserializeObject(dataList);

        List<AddOrderViewModel> addOrderViews = JsonConvert.DeserializeObject<List<AddOrderViewModel>>(dataList);

        if (addOrderViews.Count == 0)
        {
            return Json(new { success = false, message = "Add Item in Order" });
        }

        var result = _kotService.AddOrderItem(id, TotalAmount, addOrderViews);

        if (result == 0)
        {
            return Json(new { success = true, message = "Successfully, Add Order" });

        }
        else if (result == -1)
        {
            return Json(new { success = false, message = "Your Order Is Completed" });
        }
        else
        {
            return Json(new { success = false, message = "Something with wrong !" });
        }
    }



    public int ReduceQuntity(int id)
    {
        // var Prepared = _kotService.OrderUpdate(id);

        // return Prepared;
        var orderItem = _kotService.OrderUpdate(id);
        if (orderItem != null)
        {
            return orderItem.Prepared; // return how much is saved/prepared
        }
        return 0;
    }
    public string AddItemCheckOrderStatus(int id)
    {
        var status = _kotService.AddItemCheckOrderStatus(id);
        return status;
    }

    [HttpPost]
    public IActionResult ConfirmationOrderStatus(ConfirmationOrder model)
    {

        if (model.selectedTaxesstring != null)
        {
            List<TaxList> addTax = JsonConvert.DeserializeObject<List<TaxList>>(model.selectedTaxesstring);


            model.selectedTaxes = addTax;
            addTax.ForEach(t => t.orderid = model.orderid);

        }

        var result = _kotService.ConfirmationOrderStatus(model);
        if (result)
        {
            var OrderDetail = new CustomerReviewViewModel();
            OrderDetail.orderid = model.orderid;
            OrderDetail.CustomerId = model.customerid;

            return PartialView("MenuApp/_CustomerReview", OrderDetail);
        }
        else if (result == false)
        {
            return Json(new { success = false, message = "Your Order is Not Complete" });
        }

        else
        {
            return Json(new { success = false, message = "Something with wrong !" });
        }

    }


    public IActionResult SavePaymetMethod(int orderid, string method)
    {
        var result = _kotService.SavePaymetMethod(orderid, method);
        if (result != null)
        {
            return Json(new { success = true, message = "Successfully, Save Payment Method." });
        }
        else
        {
            return Json(new { success = false, message = "Something with wrong !" });

        }
    }


    public IActionResult CancelOrder(int orderid)
    {
        var result = _kotService.CancelOrder(orderid);

        if (result == 0)
        {
            return PartialView("MenuApp/_CancelOrder");
            // return Json(new { success = true, message = "Successfully,Cancle Order." });
        }
        else if (result == 1)
        {
            return Json(new { success = false, message = "The order item is ready, cannot cancel the order" });
        }
        else
        {
            return Json(new { success = false, message = "Something with wrong !" });
        }

    }
    public IActionResult ConfirmCancel(int orderid)
    {
        var result = _kotService.ConfirmationCancel(orderid);

        if (result == 0)
        {
            //return PartialView("MenuApp/_CancelOrder");
            return Json(new { success = true, message = "Successfully,Cancle Order." });
        }
        else if (result == 1)
        {
            return Json(new { success = false, message = "The order item is ready, cannot cancel the order" });
        }
        else
        {
            return Json(new { success = false, message = "Something with wrong !" });
        }
    }

    [HttpPost]
    public IActionResult CustomerReview(CustomerReviewViewModel model)
    {
        var result = _kotService.CustomerReview(model);
        if (result == true)
        {
            TempData["sucsses"] = "Add Review.";
            return RedirectToAction("WaitingTable");
            //  return Ok(Json(new { success = true, message = "Add Review." }));
            //return ;
        }
        else
        {
            TempData["error"] = "Something with wrong !";
            // return Ok("Something with wrong !");
            return RedirectToAction("WaitingTable");
            //return Json(new { success = false, message = "Something with wrong !" });
        }
    }
    #endregion
}