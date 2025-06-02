

using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;

namespace PizzaShop.repository.Interfaces
{
  public interface IKotRepository
  {



    public List<Section> GetAllSection();
    public List<Table> WaitingTable();
    public bool UpdateOrderDetail(OrderDetail data);
    public bool UpdateOrderDetail(OrderDetailsVM data);
    public WaitingTokenCode AddWaitingUser(WaitingTokenCode user);
    public List<WaitingTokenCode> GetNewWaitnigUserAllDetail(int id);
    public WaitingTokenCode GetWaitnigUserAllDetail(int id);
    public WaitingTokenCode EditWaitingUser(WaitingTokenCode user);

    // public List<WaitingTokenCode> GetAllWaitingUser();
    public List<WaitingTokenCode> GetAllWaitingUser();

    public List<Table> GetAllTableBySectionId(int sectionId);

    public Table GetTableDetail(int tableid);

    public WaitingTokenCode GetWaitnigUserById(int id);

    public Customer GetWaitingUser(string email);

    public Customer GetAllCustomer(string Email1);
    public Customer AddUserAsCustomer(Customer user);

    public Order AddOrder(Order order);
    public Payment AddPayment(Payment payment);

    public Payment UpdatePayment(Payment payment);
    public Payment CheckPayment(long orderid);

    public int AddAssignTable(WaitingUserDetails waitingUser);

    public int AddAssignTable(TableAndSectionViewModel waitingUser);

    public Table UpdateTable(Table table);
    public string GetTableName(int tableId);

    public WaitingTokenCode UpdateWaitingUser(WaitingTokenCode table);

    public OrderTableMapping AddTableOrder(OrderTableMapping user);

    public List<MenuItem> GetAllItem();

    public bool DeleteWatingToken(int id);

    public List<Section> GetAllSectionAnSelected();

    public List<Table> GetAllTable(int sectionid);

    public List<MenuItem> GetAllItemForMenuApp(int categoryId);

    public Task<List<Order>> GetAllItem(int orderId);

    public List<MenuItem> GetAllModifier(int id);

    public Task<List<FlatOrderDto>> GetAllOrder();
    public Task<List<Order>> GetAllProgressOrder();

    public Customer GetCustomerDetail(int id);
    public Order GetOrderDetail(int id);

    public Order GetOrderUpdate(int id);
    public bool UpdateCustomer(Customer customer);
    public bool UpdateOrder(Order order);

    public OrderDetail GetOrder(int DetailId);

    public OrderDetail GetDetail(int detailid);

    public List<OrderDetail> GetOrderDetailList(int id);

    public OrderTableMapping GetTableMapping(int id);

    public bool UpdatetableMapping(OrderTableMapping mapping);
    public bool AddNewOrderitem(OrderDetail order);

    public List<OrderTaxMapping> GetTaxMpping(int orderid);

    public OrderTaxMapping GetOrderTaxMapping(int orderId, int taxId);
    public bool AddOrderTaxMapping(OrderTaxMapping tax);
    public bool UpdateOrderTaxMapping(OrderTaxMapping tax);

    public bool AddFeedback(Feedback review);

    public List<WaitingTokenCode> GetWaitingtokensListByDate(DateTime fromdate);
    public int ConfirmationCancel(int orderId);

      public bool ConfirmationOrderStatus(ConfirmationOrder model);

       public int SaveOrder(int orderId, float TotalAmount, List<AddOrderViewModel> model);
  }
}