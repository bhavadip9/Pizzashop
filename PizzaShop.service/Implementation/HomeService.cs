
using AuthenticationDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Pizzashop.entity.ViewModels;
using PizzaShop.entity.Models;
using PizzaShop.entity.ViewModel;
using PizzaShop.repository.Interfaces;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _usersrepository;
        private readonly IKotRepository _kotRepository;
        private readonly ICustomerRepository _customerRepository;

        private readonly IRoleAndPermissionRepo _roleAndPermissionRepo;

        public HomeService(IHomeRepository homeRepository, IUserRepository usersrepository, IRoleAndPermissionRepo roleAndPermissionRepo, IOrderRepository orderRepository, IKotRepository kotRepository, ICustomerRepository customerRepository)
        {
            _homeRepository = homeRepository;
            _usersrepository = usersrepository;
            _roleAndPermissionRepo = roleAndPermissionRepo;
            _orderRepository = orderRepository;
            _kotRepository = kotRepository;
            _customerRepository = customerRepository;
        }


        public User GetOneByEmail(string email)
        {
            try
            {
                return _usersrepository.GetOneByEmail(email);
            }
            catch (Exception)
            {
                // Console.WriteLine(ex);
                return null;
            }
        }

        #region Profile

        /// <summary>
        /// Profile Service Get Profile Detail
        /// </summary>
        /// <returns></returns>
        public ProfileViewModel ProfileGet()
        {
            try
            {
                var email = _homeRepository.GetUserEmail();
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email cannot be null or empty.");
                }

                var user = _homeRepository.GetOneByEmail(email);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found with the given email.");
                }

                ProfileViewModel profile = new ProfileViewModel
                {
                    Email = user.Email,
                    UserId = user.UserId,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Image = user.Image,
                    Address = user.Address,
                    Phone = user.Phone,
                    CountryId = user.CountryName,
                    StateId = user.StateName,
                    CityId = user.CityName,
                    RoleId = user.Roles,
                    ZipCode = user.ZipCode,
                    Status = user.Status
                };

                return profile;
            }
            catch (Exception ex)
            {
                return new ProfileViewModel
                {
                    ErrorMessage = "An error occurred while retrieving the profile. Please try again later."
                };
            }
        }

        /// <summary>
        /// Image Upload
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>


        public ProfileViewModel UpdateImage(ProfileViewModel profile)
        {
            try
            {

                var email = _homeRepository.GetUserEmail();
                var user = _homeRepository.GetOneByEmail(email);



                if (profile.FormFile == null)
                {
                    user.Image = user.Image;
                }
                else
                {
                    user.Image = profile.Image;
                }

                var UserData = _homeRepository.UpdateUser(user);

                return new ProfileViewModel
                {
                    Email = user.Email,
                    UserId = user.UserId,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Image = user.Image,
                    Address = user.Address,
                    Phone = user.Phone,
                    CountryId = user.CountryName,
                    StateId = user.StateName,
                    CityId = user.CityName,
                    RoleId = user.Roles,
                    ZipCode = user.ZipCode,
                    Status = user.Status
                };

            }
            catch (Exception)
            {
                return new ProfileViewModel
                {
                    ErrorMessage = "An error occurred while retrieving the profile. Please try again later."
                };
            }

        }
        /// <summary>
        /// Profile Service Edit Profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns>ProfileViewModel</returns>
        public ProfileViewModel ProfileUpdate(ProfileViewModel profile)
        {
            try
            {

                var email = _homeRepository.GetUserEmail();
                var user = _homeRepository.GetOneByEmail(email);


                user.Email = email;
                user.FirstName = profile.FirstName;
                user.LastName = profile.LastName;
                user.UserName = profile.UserName;
                if (profile.Image == null)
                {
                    user.Image = user.Image;
                }
                else
                {
                    user.Image = profile.Image;
                }
                user.Address = profile.Address;
                user.Phone = profile.Phone;
                user.CountryName = profile.CountryId;
                user.StateName = profile.StateId;
                user.CityName = profile.CityId;
                user.ZipCode = profile.ZipCode ?? 0;
                user.Status = profile.Status;
                var UserData = _homeRepository.UpdateUser(user);

                return new ProfileViewModel
                {
                    Email = user.Email,
                    UserId = user.UserId,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Image = user.Image,
                    Address = user.Address,
                    Phone = user.Phone,
                    CountryId = user.CountryName,
                    StateId = user.StateName,
                    CityId = user.CityName,
                    RoleId = user.Roles,
                    ZipCode = user.ZipCode,
                    Status = user.Status
                };

            }
            catch (Exception)
            {
                return new ProfileViewModel
                {
                    ErrorMessage = "An error occurred while retrieving the profile. Please try again later."
                };
            }

        }

        /// <summary>
        /// Profile Service Change Password
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        // public bool ChangePass(ChangepasswordViewModel changePassword)
        // {

        //     try
        //     {
        //         var email = _homeRepository.GetUserEmail();
        //         var user = _homeRepository.GetOneByEmail(email);
        //         var oldpassword = user.Password;
        //         if (oldpassword != changePassword.Password)
        //         {
        //             return false;
        //         }
        //         if (changePassword.NewPassword != changePassword.ConfirmPassword)
        //         {
        //             return false;
        //         }

        //         var change = _homeRepository.UpdatePass(user, changePassword.NewPassword);

        //         return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
        //     }

        //     return false;
        // }

        public bool ChangePass(ChangepasswordViewModel changePassword)
        {

            try
            {
                var email = _homeRepository.GetUserEmail();
                var user = _homeRepository.GetOneByEmail(email);
                var oldpassword = user.Password;

                var VerifyPassword = PasswordUtills.VerifyPassword(user.Password, changePassword.Password);
                if (!VerifyPassword)
                {
                    return false;
                }
                if (changePassword.NewPassword != changePassword.ConfirmPassword)
                {
                    return false;
                }

                var change = _homeRepository.UpdatePass(user, PasswordUtills.HashPassword(changePassword.NewPassword));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
            }

            return false;
        }

        #endregion













        #region RoleAndPermission

        /// <summary>
        /// Get Permissions For Role Service
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>

        public async Task<List<PermissionViewModel>> GetPermissionsForRoleAsynC(int roleId)
        {
            try
            {
                if (roleId <= 0)
                {
                    throw new ArgumentException("Invalid role ID.", nameof(roleId));
                }

                var permissions = await _roleAndPermissionRepo.GetPermissionsByRoleIdAsync(roleId);

                if (permissions == null)
                {
                    throw new KeyNotFoundException($"No permissions found for role ID: {roleId}");
                }

                return permissions.Select(p => new PermissionViewModel
                {
                    RolePermissionId = p.RolePermissionId,
                    PermissionName = p.PermissionName,
                    CanView = p.CanView,
                    CanAddEdit = p.CanAddEdit,
                    CanDelete = p.CanDelete
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<PermissionViewModel>();
            }
        }


        /// <summary>
        /// Save Permissions Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task SavePermissionsAsync(RoleAndPermissionViewModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var OldPermissions = await _roleAndPermissionRepo.GetPermissionsByRoleIdAsync(model.RoleId);

                if (OldPermissions == null || !OldPermissions.Any())
                {
                    throw new ArgumentNullException(nameof(OldPermissions));
                }

                foreach (var perm in model.Permissions)
                {
                    var exiPerm = OldPermissions.FirstOrDefault(p => p.RolePermissionId == perm.RolePermissionId);

                    if (exiPerm != null)
                    {
                        exiPerm.CanView = perm.CanView;
                        exiPerm.CanAddEdit = perm.CanAddEdit;
                        exiPerm.CanDelete = perm.CanDelete;
                    }
                }
                await _roleAndPermissionRepo.UpdatePermissionsAsync(OldPermissions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0} ,StackTrace: {1}", ex.Message, ex.StackTrace));
            }
        }
        #endregion


        #region Dashboard

        // public DashboardViewModel GetDashboardData()
        // {
        //     var totalPayment = _orderRepository.GetOrderTotalSale();
        //     var totalorder = _orderRepository.GetOrderTotal();
        //     var waiting = _orderRepository.WaitingUser();
        //     var Completeorder = _orderRepository.GetCompleteOrder();
        //     var avgsale = (totalPayment / Completeorder);
        //     var TotalCustomer = _orderRepository.GetNewCustomer();
        //     var SellingItems = _orderRepository.GetTopSellingItems();
        //     var leastSelling = _orderRepository.leastSellingItems();
        //     var DailyRevenu = _orderRepository.DailyRevenu();

        //     var AllData = new DashboardViewModel
        //     {
        //         TotalSale = totalPayment,
        //         TotalOrder = totalorder,
        //         AvgTotalSale = avgsale,
        //         TotalWaitingUser = waiting,
        //         TotalCustomer = TotalCustomer,
        //         SellingItems = SellingItems,
        //         leastSelling = leastSelling,
        //         RevenueGraph = DailyRevenu
        //     };

        //     return AllData;

        // }

        public DashboardViewModel GetDashboard(int dateRange)
        {
            try
            {
                DateTime fromDate = dateRange switch
                {
                    0 => DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-7), DateTimeKind.Unspecified),
                    1 => DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-30), DateTimeKind.Unspecified),
                    2 => DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), DateTimeKind.Unspecified),
                    3 => DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, 1, 1), DateTimeKind.Unspecified),
                    _ => DateTime.MinValue
                };
                List<Order> orders = _orderRepository.OrderDetailsForDashboard(fromDate);
                List<WaitingTokenCode> waitingtokens = _kotRepository.GetWaitingtokensListByDate(fromDate);
                List<Customer?> customers = _customerRepository.GetCustomersWithCompletedOrderByDate(fromDate);

                var avgTime = waitingtokens.Where(w => w.IsDelete == true).Select(w => (w.CreatedAt - w.ModifiedAt.Value).Ticks).DefaultIfEmpty(0).Average();
                var AverageWaitingTime = new TimeSpan(Convert.ToInt64(avgTime));

                var orderItem = orders.SelectMany(o => o.OrderDetails).Where(o => o.Item != null).ToList();

                var allItems = orderItem.GroupBy(oi => oi.ItemId).Select(i => new SellingItem
                {
                    ItemId = i.FirstOrDefault().Item.ItemId,
                    Name = i.FirstOrDefault()?.Item?.ItemName,
                    Image = i.FirstOrDefault().Item.Image,
                    TotalQuantity = (int)i.Sum(q => q.Quntity)
                }).OrderByDescending(i => i.TotalQuantity).ToList();

                var topSelling = allItems.Take(2).ToList();

                var leastSelling = allItems.OrderBy(i => i.TotalQuantity).Where(i => !topSelling.Any(ts => ts.ItemId == i.ItemId)).Take(2).ToList();


                return new DashboardViewModel
                {
                    TotalSale = orders.Sum(o => o.Amount),
                    TotalOrder = orders.Count,
                    AvgTotalOrder = orders.Any() ? orders.Average(o => o.Amount) : 0,
                    AverageWaitingTime = AverageWaitingTime.TotalMinutes,
                    NewCustomerCount = customers.Count,
                    WaitingListCount = waitingtokens.Count(w => (bool)!w.IsDelete),
                    TopSellingItems = topSelling,
                    LeastSellingItems = leastSelling
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DashboardViewModel();
            }
        }

        public GraphDataVM GetDashboardGraph(int dateRange)
        {
            try
            {
                DateTime startDate, endDate;
                List<string> labels = new();
                List<decimal> revenueData = new();
                List<int> customerGrowthData = new();

                static DateTime AsUnspecified(DateTime dt) => DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);

                switch (dateRange)
                {
                    case 0:
                        startDate = AsUnspecified(DateTime.UtcNow.AddDays(-7));
                        endDate = AsUnspecified(DateTime.UtcNow);
                        labels = Enumerable.Range(0, 7)
                            .Select(i => DateTime.UtcNow.AddDays(-6 + i).ToString("dd"))
                            .ToList();
                        break;

                    case 1:
                        startDate = AsUnspecified(DateTime.UtcNow.AddDays(-30));
                        endDate = AsUnspecified(DateTime.UtcNow);
                        labels = Enumerable.Range(0, 30)
                            .Select(i => DateTime.UtcNow.AddDays(-29 + i).ToString("dd"))
                            .ToList();
                        break;

                    case 2:
                        startDate = AsUnspecified(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1));
                        endDate = AsUnspecified(startDate.AddMonths(1).AddDays(-1));
                        labels = Enumerable.Range(1, DateTime.DaysInMonth(startDate.Year, startDate.Month))
                            .Select(d => d.ToString("00"))
                            .ToList();
                        break;

                    case 3:
                        startDate = AsUnspecified(new DateTime(DateTime.UtcNow.Year, 1, 1));
                        endDate = AsUnspecified(new DateTime(DateTime.UtcNow.Year, 12, 31));
                        labels = Enumerable.Range(1, 12)
                            .Select(m => new DateTime(startDate.Year, m, 1).ToString("MMM"))
                            .ToList();
                        break;

                    case 4:
                    default:
                        startDate = AsUnspecified(new DateTime(2024, 1, 1));
                        endDate = AsUnspecified(DateTime.UtcNow);
                        int startYear = 2024;
                        int currentYear = DateTime.UtcNow.Year;
                        labels = Enumerable.Range(startYear, currentYear - startYear + 1)
                            .Select(y => y.ToString())
                            .ToList();
                        break;
                }

                List<Order> completedOrders = _orderRepository.OrderDetailsForDashboard(startDate);
                List<Customer?> customersWithCompletedOrder = _customerRepository.GetCustomersWithCompletedOrderByDate(startDate);

                if (dateRange == 3)
                {
                    customerGrowthData = labels.Select(label =>
                         customersWithCompletedOrder.Count(c => c.CreatedAt.ToString("MMM") == label && c.CreatedAt.Year == DateTime.Now.Year)).ToList();

                    revenueData = labels.Select(label =>
                       (decimal)completedOrders.Where(o => o.CreatedAt.ToString("MMM") == label && o.CreatedAt.Year == DateTime.Now.Year).Sum(o => o.Amount)).ToList();
                }
                else if (dateRange == 4)
                {


                    customerGrowthData = labels.Select(label =>
                   customersWithCompletedOrder.Count(c => c.CreatedAt.Year.ToString() == label)).ToList();

                    revenueData = labels.Select(label =>
                         (decimal)completedOrders.Where(o => o.CreatedAt.Year.ToString() == label).Sum(o => o.Amount)).ToList();
                }
                else
                {
                    customerGrowthData = labels.Select(label =>
                        customersWithCompletedOrder.Count(c => c.CreatedAt.ToString("dd") == label)).ToList();

                    revenueData = labels.Select(label =>
                        (decimal)completedOrders.Where(o => o.CreatedAt.ToString("dd") == label).Sum(o => o.Amount)).ToList();
                }

                return new GraphDataVM
                {
                    Labels = labels,
                    RevenueData = revenueData,
                    CustomerGrowthData = customerGrowthData,
                    MaxRevenue = revenueData.Any() ? Math.Ceiling(revenueData.Max() / 500) * 500 : 0,
                    MaxCustomerGrowth = customerGrowthData.Any() ? ((customerGrowthData.Max() + 9) / 10) * 10 : 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GraphDataVM();
            }
        }

        #endregion 

    }
}