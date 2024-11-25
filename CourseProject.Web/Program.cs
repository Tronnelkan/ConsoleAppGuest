using CourseProject.DAL.Data;
using CourseProject.BLL.Services;
using CourseProject.DAL.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CourseProject.Web.Mappings;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation.AspNetCore;
using CourseProject.Web.ViewModels.Validators;

var builder = WebApplication.CreateBuilder(args);

// Додавання служб до контейнера DI
//builder.Services.AddControllersWithViews()
//    .AddFluentValidation(config =>
//        config.RegisterValidatorsFromAssemblyContaining<RegisterViewModelValidator>());
builder.Services.AddControllersWithViews();


// Налаштування DbContext
builder.Services.AddDbContext<CourseProjectContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Реєстрація репозиторіїв
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

// Реєстрація сервісів
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAddressService, AddressService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Аутентифікація та авторизація
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    // Додайте інші політики за потребою
});

var app = builder.Build();

// Налаштування конвеєра HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<CourseProject.Web.Middleware.AuthorizationLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
