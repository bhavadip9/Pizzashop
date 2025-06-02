namespace Pizzashop.web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PizzaShop.service.Interfaces;
using Pizzashop.entity.ViewModels;
using PizzaShop.service.Implementation;

[ServiceFilter(typeof(PermissionFilter))]
[CustomAuthorize("Admin", "Manager")]
public class TaxAndFeeController : Controller
{

    private readonly ITaxService _taxService;
    public TaxAndFeeController(ITaxService taxService)
    {
        _taxService = taxService;
    }




    #region Tax And Fee

    [HttpGet]
    public IActionResult TaxAndFee()
    {
        return View();
    }


    public async Task<IActionResult> TaxAndFeeList(string search)
    {
        try
        {
            var tax = await _taxService.GetAllTax(search);
            return PartialView("_TaxTable", tax);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error in Section page: {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }

    public IActionResult AddTax()
    {
        try
        {
            return PartialView("_AddTax");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new Tax.";
            return View("Error", ex);
        }
    }

    [HttpPost]
    public IActionResult AddTax(AddTaxViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {

                if (model == null)
                {
                    return Json(new { success = false, message = "Invalid model data" });
                }
                var taxdata = _taxService.GetAllTaxList();
                var isExist = taxdata.Any(c => c.TaxName.ToLower() == model.TaxName.ToLower());
                if (isExist)
                {
                    return Json(new { success = false, message = "Tax Already Exist !" });
                }

                var result = _taxService.AddTax(model);
                if (result == true)
                {
                    return Json(new { success = true, message = "Add new Tax" });
                }
                else
                {
                    return Json(new { success = false, message = "Something with wrong try again!" });
                }
            }
            else
            {
                return Json(new { success = false, message = "Something with wrong try again!" });
            }

        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new Tax.";
            return View("Error", ex);
        }

    }



    [HttpGet]
    public async Task<IActionResult> EditTax(int id)
    {
        try
        {
            if (id == null)
            {
                return NotFound();
            }
            var tax = _taxService.GetEditTax(id);
            return PartialView("_UpdateTax", tax);
            // return Json(tax);
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while editing the tax.";
            return View("Error", ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditTax(AddTaxViewModel model)
    {
        try
        {
            if (model == null)
            {
                return NotFound();
            }
            var IsExist= _taxService.GetAllTaxList();
            var isExist = IsExist.Any(c => c.TaxName.ToLower() == model.TaxName.ToLower() && c.TaxId != model.TaxId);
            if (isExist)
            {
                return Json(new { success = false, message = "Tax Already Exist !" });
            }

            var item = _taxService.EditTax(model);

            if (item == true)
            {
                TempData["success"] = "Successfully, Update Tax";
                return Json(new { success = true, message = "Successfully, Update Tax" });
            }
            else
            {
                return Json(new { success = false, message = "Something with wrong try again!" });
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the tax.";
            return View("Error", ex);
        }


    }
    [HttpGet]
    public IActionResult DeleteTaxGet(int id)
    {
        var tax = new AddTaxViewModel();
        tax.TaxId = id;
        return PartialView("_DeleteTax", tax);

    }
    [HttpPost]
    public IActionResult DeleteTax(int id)
    {
        try
        {
            var result = _taxService.DeleteTax(id);
            if (result == true)
            {
                return Json(new { success = true, message = "Successfully, Delete Tax" });
            }
            else
            {
                return Json(new { success = false, message = "Something with wrong try again!" });
            }


        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the Tax.";
            return View("Error", ex);
        }

    }


    #endregion

}