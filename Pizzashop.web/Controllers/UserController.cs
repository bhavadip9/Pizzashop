namespace Pizzashop.web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using PizzaShop.service.Interfaces;
using PizzaShop.service.Implementation;
using Pizzashop.entity.ViewModels;


/// <summary>
/// Hendle Full Admin Accesee 
/// </summary>
[ServiceFilter(typeof(PermissionFilter))]

public class UserController : Controller
{

    private readonly IUploadService _uploadService;
    private readonly IUserService _userService;

    private readonly IHomeService _homeService;


    public UserController(IHomeService adminService, IUploadService uploadService, IUserService userService)
    {

        _homeService = adminService;
        _uploadService = uploadService;
        _userService = userService;

    }

    #region UserList


    [CustomAuthorize("Admin", "Manager")]
    [HttpGet]
    public async Task<IActionResult> Userlist()
    {
        return View("UserList");
    }

    [CustomAuthorize("Admin", "Manager")]
    public async Task<IActionResult> UserListTable(int page, int pageSize, string search, string sortBy, string sortbyrole)
    {
        var user = await _userService.GetAllUser(page, pageSize, search, sortBy, sortbyrole);
        ViewBag.SortByRole = sortbyrole;
        ViewBag.SortBy = sortBy;
        return PartialView("UserListTable", user);
    }

    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> GetAllRole()
    {
        var role = _userService.GetAllRole();
        return Json(role);

    }

    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> GetAllCounty()
    {
        var role = _userService.GetAllCounty();
        return Json(role);

    }
    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> GetStatesByCountry(int countryId)
    {
        var states = _userService.GetStatesByCountry(countryId);
        return Json(states);

    }
    [CustomAuthorize("Admin", "Manager", "Chef")]
    [HttpGet]
    public async Task<IActionResult> GetCitiesByState(int stateId)
    {
        var cities = _userService.GetCityByState(stateId);

        return Json(cities);
    }


    #endregion

    #region Add User

    [CustomAuthorize("Admin", "Manager")]
    [HttpGet]
    public async Task<IActionResult> AddUser()
    {
      
         return View("AddUser");
    }
    [CustomAuthorize("Admin", "Manager")]
    [HttpPost]
    public async Task<IActionResult> SendPassword(string Email, string Password)
    {
        var data = await _userService.SendPassword(Email, Password);
        return View(data);
    }
    [CustomAuthorize("Admin", "Manager")]
    [HttpPost]
    public async Task<IActionResult> AddUser(ProfileViewModel user, SendpasswordViewModel model)
    {
        try
        {
            if (user.RoleId == -1)
            {
                TempData["error"] = "Please select a role.";
                return RedirectToAction("AddUser", user);
            }
            ModelState.Remove("FormFile");
            if (!ModelState.IsValid)
            {
                return View("AddUser");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                TempData["error"] = "Email cannot be empty.";
                return RedirectToAction("AddUser");
            }
            var user1 = _homeService.GetOneByEmail(user.Email);

            if (user1 != null)
            {
                TempData["error"] = "Email already exists.";
                return View("AddUser", user);
            }
            if (user.FormFile != null)
            {
                var path = _uploadService.Upload(Image: user.FormFile, folder_name: user.UserName);
                user.Image = $"{Request.Scheme}://{Request.Host}/{path}";
            }
            else
            {
                user.Image = $"{Request.Scheme}://{Request.Host}/images/male-icon.jpg";
            }


            await _userService.AddUser(user);



            var callbackUrl = Url.Action("SendPassword", "Admin", new
            {
                Email = model.Email,
                Password = model.Password
            }, Request.Scheme);

            Console.WriteLine(callbackUrl);

            await SendEmailPassWordAsync(model.Email, model.Password, "Password Send");

            TempData["success"] = "Successfully sent mail.";
            return RedirectToAction("UserList");

        }
        catch (Exception ex)
        {
            TempData["error"] = "An error occurred while adding new user.";
            return View("Error", ex);
        }

    }
    [CustomAuthorize("Admin", "Manager")]
    public static async Task SendEmailPassWordAsync(string email, string password, string subject)
    {
        string smtpServer = "mail.etatvasoft.com";
        int smtpPort = 587;
        string smtpPassword = "P}N^{z-]7Ilp";
        string fromMail = "test.dotnet@etatvasoft.com";

        string htmlMessage = $@"
            <div >
                <header style='background-color: blue; padding: 10px; text-align: center;display: flex; justify-content: center; align-items: center;'>
                    <img src={@"{http://localhost:5189/images/icon/pizzashop_logo.png}"} alt='logo' style='width: 100px; height: 100px;'>
                    <h1>Pizzashop</h1>  
                </header>
                <main style='padding: 10px;'>
                    <p>Pizza Shop</p>                 
                    <p><span style='color: orange'>Important Note:</span> This is your password `{password}`</p>
                </main>
</div>";


        using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(fromMail, smtpPassword);
            smtpClient.EnableSsl = true;

            MailMessage message = new MailMessage
            {
                From = new MailAddress(fromMail),
                Subject = subject,
                Body = "<html><body>" + htmlMessage + "</body></html>",
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(email));

            try
            {
                await smtpClient.SendMailAsync(message);
                Console.WriteLine($"Email sent successfully to {email}");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }

    #endregion


    #region Edit User
    [CustomAuthorize("Admin", "Manager")]
    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var user = _userService.EditUser(id);


        if (user == null)
        {
            return NotFound();
        }

        return View("EditUser", user);

    }

    [CustomAuthorize("Admin", "Manager")]
    [HttpPost]
    public async Task<IActionResult> EditUser(ProfileViewModel profile)
    {
        if (!ModelState.IsValid)
        {
            if (profile.FormFile != null)
            {
                var path = _uploadService.Upload(Image: profile.FormFile, folder_name: profile.UserName);
                profile.Image = $"{Request.Scheme}://{Request.Host}/{path}";
            }
            var profile1 = _userService.EditUser(profile);

            TempData["succses"] = "Complete Update Profile";
            return RedirectToAction("Userlist");
        }
        return NotFound();
    }


    #endregion

    #region Delete User
    [CustomAuthorize("Admin", "Manager")]
    [HttpPost]
    public async Task<IActionResult> DeleteUser(int Id)
    {
        var user = _userService.DeleteAsync(Id);
        if (user == null)
        {
            return NotFound();
        }
        TempData["success"] = "Successfully,Add Delete";
        return RedirectToAction("Userlist");
    }


    #endregion
}