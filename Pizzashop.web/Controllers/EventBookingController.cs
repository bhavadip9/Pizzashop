namespace Pizzashop.web.Controllers;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.service.Implementation;


[CustomAuthorize("Admin", "Manager")]
public class EventBookingController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult EventBooking()
    {
        return View();
    }
}