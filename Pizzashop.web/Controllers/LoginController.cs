using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PizzaShop.service.Interfaces;
using PizzaShop.service.Implementation;
using Pizzashop.entity.ViewModels;




namespace Pizzashop.Controllers
{
   
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly ISessionService _sessionService;
        private readonly ICookieService _cookieService;
        private readonly IRoleService _roleService;
        private readonly ILoginService _loginService;

        public LoginController(IJwtService jwtService, IAuthService authService, ISessionService sessionService, ICookieService cookieService, IRoleService roleService, ILoginService loginService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _sessionService = sessionService;
            _cookieService = cookieService;
            _roleService = roleService;
            _loginService = loginService;
        }


        #region  Login
        [AllowAnonymous]
        public IActionResult Login()
        {


            if (!string.IsNullOrEmpty(Request.Cookies["UserData"]))
            {
                return RedirectToAction("Index", "Home");

                //  return RedirectToAction("Index", "Home");
            }
            else
            {
                _sessionService.ClearSession(HttpContext);
                _cookieService.ClearCookies(HttpContext);
            }

            var user = _sessionService.GetUser(HttpContext);
            //   _sessionService.SetUser(HttpContext, user);

            if (user == null)
            {
                return View();
            }
            else
            {
                var token = _jwtService.GenerateJwtToken(user.UserName, user.Email, user.RolesNavigation.RoleName, user.RolesNavigation.RoleId);
                _cookieService.SaveJWTToken(Response, token);
                return RedirectToAction("Index", "Home");
            }

        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {


            try
            {
                var user = await _authService.AuthenticateUser(model.Email, model.Password);
                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        TempData["error"] = "Email is Invalid";
                        return View();
                    }

                    if (string.IsNullOrEmpty(user.Password))
                    {
                        TempData["error"] = "Password is wrong";
                        return View(); ;
                    }
                    if (user.Status != true)
                    {
                        TempData["error"] = "Not Accsee From admin";
                        return View(); ;
                    }
                }
                else
                {
                    TempData["error"] = "Please register user.";
                    return View();
                }


                var role = _roleService.GetOneByIdAsync(user.Roles);

                // Generate JWT Token
                var token = _jwtService.GenerateJwtToken(user.FirstName, user.Email, role.RoleName, user.RolesNavigation.RoleId);

                // Store token in cookie
                _cookieService.SaveJWTToken(Response, token);

                // Save User Data to Cookie for Remember Me functionality.
                if (model.RememberMe)
                {
                    _cookieService.SaveUserData(Response, user);
                }

                // Store User details in Session
                _sessionService.SetUser(HttpContext, user);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new InvalidOperationException("This operation is not allowed");
            }

        }
        public IActionResult Logout()
        {
            //     Clear out all the Cookie data
            _cookieService.ClearCookies(HttpContext);

            //     Clear out all the Session data
            _sessionService.ClearSession(HttpContext);
            return RedirectToAction("Login");
        }

        #endregion
        [CustomAuthorize("Admin", "Manager", "Chef")]
        public IActionResult AccessDenied()
        {
            return View();
        }



        #region  Password Forget

        public IActionResult Passwordforget()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Passwordforget(string email)
        {
            try
            {
                bool user = _loginService.PasswordForget(email);
                if (user == true)
                {
                    TempData["error"] = "Email cannot Found";
                }
                else
                {
                    string resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                    var callbackUrl = Url.Action("ResetPassword", "Login", new { token = resetToken, Email = email }, Request.Scheme);
                    Console.WriteLine(callbackUrl);

                    await SendEmailAsync(email, "Password Reset", callbackUrl);

                    TempData["success"] = "Successfully,Send Reset link sent.";
                    return RedirectToAction("Login", "login");
                }
                return View();
            }
            catch
            {
                throw new Exception("Not Found");
            }

        }


        public static async Task SendEmailAsync(string email, string subject, string resetLink)
        {
            string smtpServer = "mail.etatvasoft.com";
            int smtpPort = 587;
            string smtpPassword = "P}N^{z-]7Ilp";
            string fromMail = "test.dotnet@etatvasoft.com";
            string htmlMessage = $@"
            <div >
                <header style='background-color: blue; padding: 10px; text-align: center;display: flex; justify-content: center; align-items: center;'>
                    <img src={@"{http://localhost:5092/images/icon}"} alt='logo' style='width: 100px; height: 100px;'>
                    <h1>Pizzashop</h1>  
                </header>
                <main style='padding: 10px;'>
                    <p>Pizza Shop</p>
                    
                    <p>Please Click the <a href='{resetLink}' style='color: blue;'><u>link</u></a> below to reset your password</p>
                    <br>
                    <p>If you encounter any issues, or have any question, please do not hesitate to contact our support team.</p>
                    <br>
                    <p><span style='color: orange'>Important Note:</span> For security reasons, this link will expire in 24 hours. if you did not
                    request a password reset, please ignore this email or support our contact team immediatly</p>
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


        #region ResetPassword

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var change = _loginService.resetPassword(token, email);
            if (token == null)
            {
                TempData["error"] = "Invalid password reset request.";
                return BadRequest("Invalid password reset request.");
            }
            return View(change);

        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _loginService.Updatepass(model);
                    TempData["success"] = "Password reset successfully.";
                    return RedirectToAction("Login", "login");
                }
                return View(model);
            }
            catch
            {
                throw new Exception();
            }
        }

    }
    #endregion
}