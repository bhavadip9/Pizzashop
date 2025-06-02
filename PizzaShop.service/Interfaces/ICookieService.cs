
using Microsoft.AspNetCore.Http;
using PizzaShop.entity.Models;

namespace PizzaShop.service.Interfaces
{
    public interface ICookieService
    {
        void SaveJWTToken(HttpResponse response, string token);

        string? GetJWTToken(HttpRequest request);

        void SaveUserData(HttpResponse response, User user);

        void ClearCookies(HttpContext httpContext);
    }
}