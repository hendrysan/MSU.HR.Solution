using MSU.HR.Contexts;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
});

builder.Services.AddAuthentication()
         .AddCookie(options =>
         {
             options.LoginPath = "/Auth/Login";
             options.LogoutPath = "/Auth/Logout";
         });
//.ConfigureApplicationCookie(options => options.LoginPath = "/Auth/Login");


builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<IUser, UserRepository>();

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
