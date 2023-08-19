using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
});

builder.Services
    .AddIdentityCore<AspNetUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<DatabaseContext>();

builder.Services.AddAuthentication()
         .AddCookie(options =>
         {
             options.LoginPath = "/Auth/Login";
             options.LogoutPath = "/Auth/Logout";
         });
//.ConfigureApplicationCookie(options => options.LoginPath = "/Auth/Login");

builder.Services.AddMvc();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IUser, UserRepository>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
