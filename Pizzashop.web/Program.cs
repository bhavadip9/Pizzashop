using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using pizzashop.services.Implementations;
using Pizzashop.Services;
using PizzaShop.entity.Models;
using PizzaShop.repository.Implementation;
using PizzaShop.repository.Interfaces;
using PizzaShop.service.Implementation;
using PizzaShop.service.Interfaces;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var conn = builder.Configuration.GetConnectionString("PizzashopDatabase");
builder.Services.AddDbContext<NewPizzashopContext>(q => q.UseNpgsql(conn));


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IloginRepository, LogicRepository>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IRoleAndPermissionRepo, RoleAndPermissionRepo>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IKotRepository, KotRepository>();
builder.Services.AddScoped<IKotService, KotService>();
builder.Services.AddScoped<PermissionFilter>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/login";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login/AccessDenied";
    });

builder.Services.AddAuthorization();
// builder.Services.AddHttpContextAccessor();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.MapHub<OrderHub>("/orderhub");
app.UseStatusCodePagesWithReExecute("/Home/Error", "?errorCode={0}");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();