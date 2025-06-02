
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.repository.Interfaces;

namespace PizzaShop.repository.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        private readonly NewPizzashopContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public HomeRepository(NewPizzashopContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }



        public string? GetUserEmail()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
                return email;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public User? GetOneByEmail(string email)
        {
            try
            {
                return _context.Users.Where(c => !c.IsDelete).FirstOrDefault(m => m.Email.ToLower() == email.ToLower());
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public User? GetUser(int id)
        {
            try
            {
                return _context.Users.Where(c => !c.IsDelete).FirstOrDefault(m => m.UserId == id);
            }
            catch (Exception e)
            {
                return null;
            }

        }



        public User? UpdateUser(User user)
        {
            try
            {
                _context.Update(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public RolePermissionTable? UpdatePermisson(RolePermissionTable permission)
        {
            try
            {
                _context.Update(permission);
                _context.SaveChanges();
                return permission;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public User? UpdatePass(User user, string NewPassword)
        {
            try
            {
                user.Password = NewPassword;
                _context.Update(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public async Task<User> AddUser(ProfileViewModel user)
        {
            try
            {
                var newUser = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                    Roles = user.RoleId,
                    UserName = user.UserName,
                    Image = user.Image,
                    CountryName = user.CountryId,
                    StateName = user.StateId,
                    CityName = user.CityId,
                    ZipCode = user.ZipCode ?? 0,
                    Address = user.Address,
                    Phone = user.Phone
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            catch (Exception e)
            {
                return null;
            }

        }


    }



}