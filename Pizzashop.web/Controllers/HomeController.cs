using Microsoft.AspNetCore.Mvc;
using Pizzashop.entity.ViewModels;
using PizzaShop.service.Implementation;
using PizzaShop.service.Interfaces;

namespace Pizzashop.web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISessionService _sessionService;

    private readonly IHomeService _homeService;

    private readonly IJwtService _jwtService;

    private readonly ICookieService _cookieService;

    private readonly IUploadService _uploadService;

    public HomeController(ILogger<HomeController> logger, ISessionService sessionService, IJwtService iwtService, ICookieService cookieService, IHomeService homeService, IUploadService uploadService)
    {
        _logger = logger;
        _sessionService = sessionService;
        _jwtService = iwtService;
        _cookieService = cookieService;
        _homeService = homeService;
        _uploadService = uploadService;
    }


    public IActionResult Index()
    {

        if (!string.IsNullOrEmpty(Request.Cookies["UserData"]))
        {
            var user_ = _sessionService.GetUser(HttpContext);
            _sessionService.SetUser(HttpContext, user_);
            var token = _jwtService.GenerateJwtToken(user_.UserName, user_.Email, user_.RolesNavigation.RoleName, user_.RolesNavigation.RoleId);
            _cookieService.SaveJWTToken(Response, token);
            return user_?.RolesNavigation.RoleName switch
            {
                "Admin" => RedirectToAction("AdminHome", "Home"),
                "Chef" => RedirectToAction("Index", "OrderApp"),
                "Manager" => RedirectToAction("ManagerHome", "Home"),

            };
        }
        var user = _sessionService.GetUser(HttpContext);
        if (user == null)
        {
            return RedirectToAction("Login", "Login");
        }

        return user?.RolesNavigation.RoleName switch
        {
            "Admin" => RedirectToAction("AdminHome", "Home"),
            "Chef" => RedirectToAction("Index", "OrderApp"),
            "Manager" => RedirectToAction("ManagerHome", "Home"),

        };
    }

    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpPost]
    public async Task<IActionResult> UpdateImage(ProfileViewModel profile)
    {
        try
        {
            if (profile.FormFile != null)
            {
                var uploadPath = _uploadService.Upload(profile.FormFile, "Upload");
                profile.Image = $"{Request.Scheme}://{Request.Host}/{uploadPath}";
            }
            var profile1 = _homeService.UpdateImage(profile);
            if (profile1 == null)
            {
                TempData["success"] = "Profile update completed.";
                return NotFound();
            }
            else
            {
                TempData["success"] = "Profile update completed.";
                return View("Profile", profile1);
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the profile.";
            return View("Error", ex);
        }
    }


    public IActionResult GetDashboard(int dateRange)
    {
        var model = _homeService.GetDashboard(dateRange);
        var user = _sessionService.GetUser(HttpContext);
        model.Role = user.RolesNavigation.RoleName;
        

        return PartialView("_DashBoardPV", model);
    }

    public IActionResult GetDashboardGraph(int dateRange)
    {
        var model = _homeService.GetDashboardGraph(dateRange);
        return Json(model);
    }

    public IActionResult UserProfile()
    {
        var user = _sessionService.GetUser(httpContext: HttpContext);

        var email = user.Email;

        var userProfile = _homeService.GetOneByEmail(email);
        if (user != null)
        {
            return Ok(userProfile);
        }
        return NotFound();
    }

    [CustomAuthorize("Admin")]
    public IActionResult AdminHome()
    {
        return View();
    }

    [CustomAuthorize("Users")]
    public IActionResult UserHome()
    {
        return View();
    }
    [CustomAuthorize("Manager")]
    public IActionResult ManagerHome()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    #region Profile 


    /// <summary>
    ///  Show Profile Page
    /// </summary>
    /// <returns></returns>


    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        try
        {
            var profile = _homeService.ProfileGet();
            return View("Profile", profile);
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while Profile Loading.";
            return View("Error", ex);
        }

    }





    /// <summary>
    ///  Update Profile
    /// </summary>
    /// <param name="profile"></param>
    /// <returns> Profile Data </returns>
    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel profile)
    {
        try
        {

            // if(!ModelState.IsValid)
            // {
            //     TempData["error"] = "An error occurred while updating the profile.";
            //     return View("Profile", profile);
            // }
            if (profile.FormFile != null)
            {
                var uploadPath = _uploadService.Upload(profile.FormFile, "Upload");
                profile.Image = $"{Request.Scheme}://{Request.Host}/{uploadPath}";
            }
            else
            {
                profile.Image = profile.Image;
            }
            var profile1 = _homeService.ProfileUpdate(profile);
            if (profile1 == null)
            {
                TempData["error"] = "An error occurred while updating the profile.";
                return NotFound();
            }
            else
            {
                TempData["success"] = "Profile update completed.";
                return View("Profile", profile1);
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the profile.";
            return View("Error", ex);
        }
    }


    #endregion

    #region  Change Password

    /// <summary>
    ///  Show Change Password Page
    /// </summary>
    /// <returns></returns>

    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        try
        {
            return View("ChangePassword");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while Change Password Loading.";
            return View("Error", ex);
        }
    }

    /// <summary>
    ///  Change Password
    /// </summary>
    /// <param name="changePassword"></param>
    /// <returns>Change Password Data</returns>


    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangepasswordViewModel changePassword)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool Change = _homeService.ChangePass(changePassword);
                if (!Change)
                {
                    ModelState.AddModelError("ConfirmPassword", "New Password and Confirm Password do not match");
                    ModelState.AddModelError("Password", "Old Password is incorrect");
                    return View("ChangePassword");
                }
                TempData["success"] = "Successfully,password Update";
                return RedirectToAction("Logout", "Login");
            }
            return View("ChangePassword");
        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while updating the password.";
            return View("Error", ex);
        }
    }


    #endregion


    [CustomAuthorize("Admin", "Chef", "Manager")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string errorCode)
    {
        var viewModel = new ErrorViewModel();

        int.TryParse(errorCode, out int code);

        switch (code)
        {
            case 403:
                viewModel.HtmlTitleTag = "403";
                return View("Error", viewModel);
            case 404:
                viewModel.HtmlTitleTag = "404";
                return View("Error", viewModel);
            case 500:
                viewModel.HtmlTitleTag = "500";
                return View("Error", viewModel);
            default:
                viewModel.HtmlTitleTag = "Unknown Error";
                return View("Error", viewModel);
        }
    }

     [Route("ServerError")]
    public IActionResult ServerError()
    {
        // You can customize this view or return your own static HTML, etc.
        // Option 1: Return a View (requires setup of MVC views)
        // return View("ServerError");

        // Option 2: Return inline HTML for demonstration
        string html = @"
        <html>
        <head><title>Server Error</title></head>
        <body>
            <h1>Oops! Something went wrong.</h1>
            <p>Please try again later.</p>
        </body>
        </html>";
        return Content(html, "text/html");
    }
}