
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using PizzaShop.entity.Models;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation
{
    public class SessionService : ISessionService
    {
        public void SetUser(HttpContext httpContext, User user)
        {
            // if (user != null)
            // {
            //     string userData = JsonSerializer.Serialize(user);
            //     httpContext.Session.SetString("UserData", userData);
            // }

            if (user != null)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true // Optional
                    };

                    string userData = JsonSerializer.Serialize(user, options);
                    httpContext.Session.SetString("UserData", userData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Serialization Error: {ex.Message}");
                    throw;
                }
            }
        }

        public User? GetUser(HttpContext httpContext)
        {

            string? userData = httpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userData))
            {
                httpContext.Request.Cookies.TryGetValue("UserData", out userData);
            }

            return string.IsNullOrEmpty(userData) ? null : JsonSerializer.Deserialize<User>(userData);
        }

        public void ClearSession(HttpContext httpContext) => httpContext.Session.Clear();

    }
}