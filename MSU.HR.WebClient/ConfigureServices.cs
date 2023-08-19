using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;

namespace MSU.HR.WebClient
{
    public static class ConfigureServices
    {
        public static IConfiguration? Configuration { get; }
        public static IServiceCollection AddWebClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
            });

            services
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

            services.AddAuthentication()
                     .AddCookie(options =>
                     {
                         options.LoginPath = "/Auth/Login";
                         options.LogoutPath = "/Auth/Logout";
                     });


            services.AddMvc();
            //services.AddHttpContextAccessor();
            //builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddScoped<IUser, UserRepository>();


            return services;
        }
    }
}
