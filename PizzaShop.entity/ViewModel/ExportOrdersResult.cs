using Pizzashop.entity.ViewModels;

public class ExportOrdersResult
{
    public string status { get; set; }
    public string search { get; set; }
    public string Date { get; set; }
    public string record { get; set; }
    public List<OrderViewModel> orderData { get; set; }
    public List<CustomerDetailViewModel> CustomerData { get; set; }
}