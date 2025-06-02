
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.service.Interfaces
{
  public interface IKotService
  {
    public WaitingViewModel GetAllSection();
 
    public Task<AddPaginationViewmodel<OrderDetailViewModel>> GetAllOrder(string status, int page, int pageSize);

    public WaitingUserDetails AddWaitingUser(WaitingUserDetails waiting);

    public List<WaitingUserDetails> GetAllWaitingUser();

    public List<TableList> GetAllTableBySectionId(int sectionId);

    public int AddAsCustomer(WaitingUserDetails waiting);
    public List<SectionList> GetAllSectionList();
    public int AssignTable(TableAndSectionViewModel waiting);
    public List<AddCategoryViewModel> GetAllCategoryList();

    public bool DeleteWatingToken(int id);

    public List<Section> GetAllAnselectSection();
    public List<Table> GetAllTable(int sectionid);

    public int GetIsExist(string email);
    public WaitingUserDetails EditWaitingUser(int waitingUserid);
    public bool EditWaitingUser(WaitingUserDetails waiting);

    public List<AddItemViewModel> GetAllItem(int categoryId, string search);

    public List<AddItemViewModel> GetAllModifier(int id);
    public List<AddItemViewModel> GetAllItemList(string search);


    public bool checkStatus(int orderid);
    public List<WaitingUserDetails> GetAllWaitingUserBySectionId(int sectionId);

    public bool Addfavorite(int itemId);
    public Task<AddPaginationViewmodel<OrderDetailViewModel>> CategoryWithOrder(int categoryid, int page, int pageSize, string status);

    public bool UpdateOrderItemStatus(int orderid, string status, List<UpdateOrder> item);
    public OrderDetailViewModel GetOrderDetails(int id, string status);

    public List<AddItemViewModel> GetAllItemFav(string search);
    public CustomerDetailViewModel EditCustomerDetail(int Customerid);

    public int EditCustomerDetail(CustomerDetailViewModel modal);

    public bool AddOrderComment(OrderDetailViewModel modal);

    public bool AddItemComment(OrderDetailViewModel modal);
    public int AddOrderItem(int orderId, float TotalAmount, List<AddOrderViewModel> model);

    public bool ConfirmationOrderStatus(ConfirmationOrder model);

    public int ConfirmationOrder(int orderid);

    public OrderDetailViewModel GetOrderDetails(int id);


    public OrderDetail OrderUpdate(int id);

    public string AddItemCheckOrderStatus(int id);

    public bool SavePaymetMethod(int id, string method);

    public int CancelOrder(int orderid);

    public bool CustomerReview(CustomerReviewViewModel model);

    public int ConfirmationCancel(int orderid);

    public WaitingUserDetails CheckWaitingUser(string email);
  }
}