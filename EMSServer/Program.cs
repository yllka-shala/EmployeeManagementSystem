using EMSBaseLibrary.Entities;
using EMSServer.Data;
using EMSServer.Helpers;
using EMSServer.Repositories.Implementations;
using EMSServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
var jwtSection = builder.Configuration.GetSection(nameof(JwtSection)).Get<JwtSection>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultCoonnection") ?? 
        throw new InvalidOperationException("Connection not found!"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection!.Issuer,
        ValidAudience = jwtSection!.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Key))
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token!",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },

            new string[]{}
        }
    });
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<PerformanceReview>, PerformanceReviewRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Department>, DepartmentRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<EmployeePosition>, PositionRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<LeaveRequest>, LeaveRequestRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Attendance>, AttendanceRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Deduction>, DeductionRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Employee>, EmployeeRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<LeaveType>, LeaveTypeRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<EmployeeDocument>, EmployeeDocumentRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Notification>, NotificationRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Asset>, AssetRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<EmployeeAsset>, EmployeeAssetRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<EmployeeNotification>, EmployeeNotificationRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Bonus>, BonusRepository>();
builder.Services.AddScoped<IGenericRepositoryInterface<Roles>, RolesRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", builder => builder
    .WithOrigins("https://localhost:7281", "http://localhost:5236")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
});

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorWasm");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
