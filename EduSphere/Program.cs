﻿using EduSphere.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// DI için mail servis interface ve classlarını eklemeyi unutma!
using Microsoft.AspNetCore.Http; // IHttpContextAccessor için

var builder = WebApplication.CreateBuilder(args);

// 1. Servisleri ekle
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EduSphereContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IHttpContextAccessor DI container'a ekle
builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

// 2. App'i build et
var app = builder.Build();

// 3. Middleware'leri sırayla ekle
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin için özel route ekliyoruz
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Login}/{id?}",
    defaults: new { controller = "Admin" }
);

// Account giriş ekranı ana route
app.MapControllerRoute(
    name: "login",
    pattern: "",
    defaults: new { controller = "Account", action = "Login" }
);

// Messages için özel route
app.MapControllerRoute(
    name: "messages",
    pattern: "Messages/{action=Index}/{id?}",
    defaults: new { controller = "Messages" }
);

// Diğer controller'lar için genel route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();