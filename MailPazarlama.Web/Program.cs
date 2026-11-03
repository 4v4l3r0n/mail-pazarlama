using MailPazarlama.Business.Services.MailSender;
using MailPazarlama.Business.Services.MailSetting;
using MailPazarlama.Business.Services.MailTemplate;
using MailPazarlama.Business.Services.Report;
using MailPazarlama.Business.Services.Security;
using MailPazarlama.Business.Services.Subscriber;
using MailPazarlama.Business.Services.User;
using MailPazarlama.Data;
using MailPazarlama.Data.UnitOfWork;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISubscriberService, SubscriberService>();
builder.Services.AddScoped<IMailTemplateService, MailTemplateService>();
builder.Services.AddScoped<IMailSettingService, MailSettingService>();
builder.Services.AddScoped<IMailReportService, MailReportService>();
builder.Services.AddScoped<IMailSenderService, MailSenderService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "MailPazarlama.Auth";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
