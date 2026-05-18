using EMS_Client;
using EMS_Client.Helpers;
using EMS_Client.Services.Implementations;
using EMS_Client.Services.Interfaces;
using EMSBaseLibrary.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7217/");
}).AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGenericService<Attendance>, GenericService<Attendance>>();
builder.Services.AddScoped<IGenericService<Department>, GenericService<Department>>();
builder.Services.AddScoped<IGenericService<EmployeePosition>, GenericService<EmployeePosition>>();
builder.Services.AddScoped<IGenericService<LeaveRequest>, GenericService<LeaveRequest>>();
builder.Services.AddScoped<IGenericService<LeaveType>, GenericService<LeaveType>>();
builder.Services.AddScoped<IGenericService<PerformanceReview>, GenericService<PerformanceReview>>();
builder.Services.AddScoped<IGenericService<Employee>, GenericService<Employee>>();
builder.Services.AddScoped<IGenericService<EmployeeDocument>, GenericService<EmployeeDocument>>();
builder.Services.AddScoped<IGenericService<Notification>, GenericService<Notification>>();
builder.Services.AddScoped<IGenericService<EmployeeNotification>, GenericService<EmployeeNotification>>();
builder.Services.AddScoped<IGenericService<Bonus>, GenericService<Bonus>>();
builder.Services.AddScoped<IGenericService<Deduction>, GenericService<Deduction>>();
builder.Services.AddScoped<IGenericService<Asset>, GenericService<Asset>>();
builder.Services.AddScoped<IGenericService<EmployeeAsset>, GenericService<EmployeeAsset>>();
builder.Services.AddScoped<IGenericService<Roles>, GenericService<Roles>>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
