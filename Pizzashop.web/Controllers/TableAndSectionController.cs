namespace Pizzashop.web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PizzaShop.service.Interfaces;
using PizzaShop.service.Implementation;
using Pizzashop.entity.ViewModels;


[ServiceFilter(typeof(PermissionFilter))]
[CustomAuthorize("Admin", "Manager")]
public class TableAndSectionController : Controller
{


    private readonly ISectionService _sectionService;

    public TableAndSectionController(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    #region Section


    [HttpGet]
    // public IActionResult TableAndSection()
    // {
    //     return View("TableAndSection/TableAndSection");
    // }
    public async Task<IActionResult> TableAndSection()
    {
        try
        {
            var section = await _sectionService.GetAllSection();



            if (section == null || !section.Any())
            {
                var emptyViewModel = new SectionListViewModel
                {
                    Section = new List<AddSectionViewModel>(),
                };

                return View("TableAndSection", emptyViewModel);
            }
            var viewModel = new SectionListViewModel
            {
                Section = section.Select(p => new AddSectionViewModel
                {
                    SectionId = p.SectionId,
                    SectionName = p.SectionName,
                    SectionDescription = p.SectionDescription
                }).ToList(),

            };
            return View("TableAndSection", viewModel);

        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error in Section page: {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }



    [HttpGet]
    public IActionResult AddSection()
    {
        try
        {
            return PartialView("_AddSectionPV");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new section.";
            return View("Error", ex);
        }
    }



    [HttpPost]
    public IActionResult AddSection(AddSectionViewModel model)
    {
        try
        { ModelState.Remove("SectionId");
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Model is not valid !";
                return Json(new { success = false , message = "Model is not valid !" });

            }
           
            var sectionlist = _sectionService.GetAllSectionList();
            bool isSectionExists = sectionlist.Select(c => c.SectionName.ToLower()).Contains(model.SectionName.ToLower());

            if (isSectionExists == true)
            { TempData["error"] = "Already Exist Section Name";
                return Json(new { success = false, message = "Same Name Of Section Not Valid" });
            }
            
            var result = _sectionService.AddSectionAsync(model);
            TempData["success"] = "Successfully, Add Section";
            return Json(new { success = true, result.SectionId, message = "New section added successfully." });
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new section.";
             return Json(new { success = false, message = "Exception occurred while adding new section." });
        }
    }


    public async Task<IActionResult> UpdateSection(int sectionId)
    {
        try
        {
            if (sectionId == null)
            {
                return NotFound();
            }

            var section = _sectionService.GetEditSection(sectionId);
            return PartialView("_UpdateSectionPV", section);
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while editing the section.";
            return View("Error", ex);
        }

    }

    [HttpPost]
    public async Task<IActionResult> UpdateSection(AddSectionViewModel model)
    {
        try
        {
            if (model == null)
            {
                return NotFound();
            }
            var sectionlist = _sectionService.GetAllSectionList();
            bool isSectionExists = sectionlist.Any(c => c.SectionName.ToLower() == model.SectionName.ToLower() && c.SectionId != model.SectionId);
            if (isSectionExists == true)
            {
                TempData["error"] = "Already Exist Section Name";
                return Json(new { success = false });
            }
            var section = _sectionService.UpdateSection(model);

            if (section)
            {
                TempData["success"] = "Successfully, Update Section";
                return Json(new { success = true, meassge = "Successfully, Update Section" });
            }
            else
            {
                TempData["error"] = "Something with wrong !";
                return Json(new { success = false, meassge = "Something with wrong !" });
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the section.";
            return View("Error", ex);
        }
    }


    [HttpPost]
    public IActionResult DeleteSection(int SectionId)
    {
        try
        {
            if (SectionId <= 0)
            {
                return Json(new { success = true, meassge = "Invalid section ID." });
            }
            _sectionService.DeleteSection(SectionId);

            TempData["success"] = "Successfully, Delete Section";

            return RedirectToAction("TableAndSection");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the section.";
            return View("Error", ex);
        }
    }
    #endregion

    #region Table

    public IActionResult GetAllSection()
    {
        var section = _sectionService.GetAllSectionList();
        return Json(section);
    }


    [HttpGet]
    public async Task<IActionResult> GetTableBySection(int sectionId, int page, int pageSize, string search)
    {
        try
        {
            var table = await _sectionService.GetAllTable(sectionId, page, pageSize, search);
            return PartialView("_TablePV", table);
        }
        catch (Exception ex)
        {

            Console.Error.WriteLine($"Error in GetItemsByCategory: {ex.Message}");
            return StatusCode(500, "Internal server error. Please check the logs for details.");
        }
    }

    [HttpGet]
    public IActionResult AddTable()
    {
        try
        {
            return PartialView("_AddTable");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new section.";
            return View("Error", ex);
        }
    }

    [HttpPost]
    public IActionResult AddTable(AddTableViewModel model)
    {
        try
        {
            var GetTableList = _sectionService.GetTableList(model.SectionId);

            var isTableExist = GetTableList.Any(c => c.TableName.ToLower() == model.TableName.ToLower());

            if (isTableExist == true)
            {
                // TempData["error"] = "";
                return Json(new { success = false, message = "In this Section Already table Exist !" });
            }


            if (model == null)
            {
                TempData["error"] = "Exception occurred while adding new table.";
                return Json(new { success = false });
                // return RedirectToAction("TableAndSection");
            }
            _sectionService.AddTable(model);
            // TempData["succses"] = "Add new Table";
            return Json(new { success = true, message = "Add new Table" });
            //return RedirectToAction("TableAndSection");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new Table.";
            return View("Error", ex);
        }

    }

    [HttpGet]
    public async Task<IActionResult> EditTable(int id)
    {
        try
        {
            if (id == null)
            {
                return NotFound();
            }
            var table = _sectionService.GetEditTable(id);
            return Json(table);
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while editing the item.";
            return View("Error", ex);
        }


    }

    [HttpPost]
    public async Task<IActionResult> EditTable(AddTableViewModel model)
    {
        try
        {
            if (model == null)
            {
                return NotFound();
            }
            var GetTableList = _sectionService.GetTableList(model.SectionId);
            var isTableExist = GetTableList.Any(c => c.TableName.ToLower() == model.TableName.ToLower() && c.TableId != model.TableId);
            if (isTableExist == true)
            {
                // TempData["error"] = "";
                // return Json(new { success = false, message = "In this Section Already table Exist !" });
                TempData["error"] = "In this Section Already table Exist !";
                return RedirectToAction("TableAndSection");
            }

            var item = _sectionService.EditTable(model);

            TempData["success"] = "Successfully, Update Table";
            return RedirectToAction("TableAndSection");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the item.";
            return View("Error", ex);
        }


    }

    [HttpPost]
    public IActionResult DeleteTable(int id)
    {
        try
        {


            _sectionService.DeleteTable(id);
            TempData["success"] = "Successfully, Delete Table";
            return RedirectToAction("TableAndSection");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the Table.";
            return View("Error", ex);
        }

    }
    [HttpGet]
    public IActionResult DeleteManyTable(List<int> Ids)
    {
        if (Ids == null)
        {
            TempData["error"] = "An error occurred while deleting the Table.";
        }
        @ViewBag.ids = Ids;
        return PartialView("_ManyTableDeletePV");
    }

    [HttpPost]
    public IActionResult DeleteManyTablepost(List<int> Ids)
    {
        try
        {

            _sectionService.DeleteManyTable(Ids);
            TempData["success"] = "Successfully, Delete Many Table";
            return RedirectToAction("TableAndSection");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while deleting the Table.";
            return View("Error", ex);
        }

    }

    #endregion



}