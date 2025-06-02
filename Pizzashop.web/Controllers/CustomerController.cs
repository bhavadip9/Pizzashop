namespace Pizzashop.web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PizzaShop.service.Interfaces;
using ClosedXML.Excel;
using PizzaShop.service.Implementation;

[ServiceFilter(typeof(PermissionFilter))]
[CustomAuthorize("Admin", "Manager")]
public class CustomerController : Controller
{


    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }



    #region Customer


    [HttpGet]
    public IActionResult Customer()
    {
        return View("Customer");
    }

   public async Task<IActionResult> CustomerTable(int page, int pageSize, string search, string Date, string SortbyName, string SortbyDate)
    {
        var customer = await _customerService.GetAllCutomer(page, pageSize, search, Date, SortbyName, SortbyDate);
        ViewBag.SortByName = SortbyName;
        ViewBag.SortbyDate = SortbyDate;
        return PartialView("CustomerTable", customer);
    }

    public IActionResult ExportAllCustomer(string time, string search)
    {
        // int timeint = int.Parse(time);
        var customerdata = _customerService.GetExportCustomer(search: search, time: time);

        using var wb = new XLWorkbook();

        var ws = wb.AddWorksheet();
        ws.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Range("A2", "B3").Merge().Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Cell("A2").Value = "Account";
        ws.Cell("A2").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);
        ws.Range("C2", "F3").Merge();
        //  ws.Cell("C2").Value = ordersdata.status;
        ws.Cell("C2").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);

        ws.Range("H2", "I3").Merge().Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Cell("H2").Value = "Search Text";
        ws.Cell("H2").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);
        ws.Range("J2", "M3").Merge();
        ws.Cell("J2").Value = customerdata.search;
        ws.Cell("J2").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);

        ws.Range("A5", "B6").Merge().Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Cell("A5").Value = "DATE";
        ws.Cell("A5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);
        ws.Range("C5", "F6").Merge();
        ws.Cell("C5").Value = customerdata.Date;
        ws.Cell("C5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);

        ws.Range("H5", "I6").Merge().Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Cell("H5").Value = "No of Record";
        ws.Cell("H5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);
        ws.Range("J5", "M6").Merge();
        ws.Cell("J5").Value = customerdata.record;
        ws.Cell("J5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
        .Border.SetTopBorder(XLBorderStyleValues.Thin)
        .Border.SetRightBorder(XLBorderStyleValues.Thin)
        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
        .Border.SetLeftBorder(XLBorderStyleValues.Thin);
         var img = "../Pizzashop.web\\wwwroot\\images\\logos\\pizzashop_logo.png";
        
        ws.Range("O2", "P6").Merge();
        ws.AddPicture(img).MoveTo(ws.Cell("O2")).Scale(.3);


        ws.Cell("A9").Value = "Id";
        ws.Cell("A9").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Range("B9", "D9").Merge();
        ws.Cell("B9").Value = "Name";
        ws.Cell("B9").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Range("E9", "G9").Merge();
        ws.Cell("E9").Value = "Email";
        ws.Cell("E9").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Range("H9", "J9").Merge();
        ws.Cell("H9").Value = "Date";
        ws.Cell("H9").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Range("K9", "L9").Merge();
        ws.Cell("K9").Value = "Phone";
        ws.Cell("K9").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Range("M9", "N9").Merge();
        ws.Cell("M9").Value = "TotalOrder";
        ws.Cell("M9").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0066A8"));
        ws.Range("O9", "P9").Merge();


        for (var j = 0; j < customerdata.CustomerData.Count(); j++)
        {
            var i = j + 10;
            // dataTable.Rows.Add(row.OrderID,row.OrderDate,row.Customer,row.Status,row.PaymentMod,row.Rating,row.Total);
            ws.Cell("A" + i).Value = customerdata.CustomerData[j].CustomerId;
            ws.Range("B" + i, "D" + i).Merge();
            ws.Cell("B" + i).Value = customerdata.CustomerData[j].CustomerName;
            ws.Range("E" + i, "G" + i).Merge();
            ws.Cell("E" + i).Value = customerdata.CustomerData[j].CustomerEmail;
            ws.Range("H" + i, "J" + i).Merge();
            ws.Cell("H" + i).Value = customerdata.CustomerData[j].Date + "";
            ws.Range("K" + i, "L" + i).Merge();
            ws.Cell("K" + i).Value = customerdata.CustomerData[j].CustomerPhone;
            ws.Range("M" + i, "N" + i).Merge();
            ws.Cell("M" + i).Value = customerdata.CustomerData[j].TotalOrder;

        }


        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");


    }


    public IActionResult GetDetailCustomer(int id)
    {

        var model = _customerService.GetCustomer(id);
        return PartialView("CustomerDetail", model);
    }

    #endregion

}