using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Services;

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

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//We set Time here 
                //options.Cookie.HttpOnly = true;
                //options.Cookie.IsEssential = true;
            });


            services.AddMvc();
            services.AddHttpContextAccessor();
            services.AddInterfaceServices(configuration);

            return services;
        }
    }
}
