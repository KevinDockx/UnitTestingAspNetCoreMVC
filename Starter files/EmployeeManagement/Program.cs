using EmployeeManagement;
using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.DbContexts;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



// add HttpClient support
builder.Services.AddHttpClient("TopLevelManagementAPIClient");

// add AutoMapper for mapping between entities and viewmodels
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// add support for Sessions (requires a store)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// add other services
builder.Services.RegisterBusinessServices();
builder.Services.RegisterDataServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

// custom middleware
app.UseMiddleware<EmployeeManagementSecurityHeadersMiddleware>();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EmployeeOverview}/{action=Index}/{id?}");

app.Run();
