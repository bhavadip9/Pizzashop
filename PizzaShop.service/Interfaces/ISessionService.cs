
using Microsoft.AspNetCore.Http;
using PizzaShop.entity.Models;

namespace PizzaShop.service.Interfaces
{
  public interface ISessionService
  {
    void SetUser(HttpContext httpContext, User user);

    User? GetUser(HttpContext httpContext);

    void ClearSession(HttpContext httpContext);
  }
}