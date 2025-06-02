using System.Security.Claims;


namespace PizzaShop.service.Interfaces
{
  public interface IJwtService
  {
    public string GenerateJwtToken(string name, string email, string role, int roleid);
    ClaimsPrincipal? ValidateToken(string token);
  }
}