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

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//We set Time here 
                //options.Cookie.HttpOnly = true;
                //options.Cookie.IsEssential = true;
            });


            services.AddMvc();
            services.AddHttpContextAccessor();
                        
            services.AddScoped<ILogError, LogErrorRepository>();
            services.AddScoped<IToken, TokenRepository>();
            services.AddScoped<IUser, UserRepository>();
            services.AddScoped<ICorporate, CorporateRepository>();
            services.AddScoped<IBank, BankRepository>();
            services.AddScoped<IDepartment, DepartmentRepository>();
            services.AddScoped<IEducation, EducationRepository>();
            services.AddScoped<IGrade, GradeRepository>();
            services.AddScoped<IJob, JobRepository>();
            services.AddScoped<IReason, ReasonRepository>();
            services.AddScoped<ISection, SectionRepository>();
            services.AddScoped<ITypeEmployee, TypeEmployeeRepository>();
            services.AddScoped<IPTKP, PTKPRepository>();
            services.AddScoped<IRole, RoleRepository>();
            services.AddScoped<IEmployee, EmployeeRepository>();

            services.AddScoped<ITimeOff, TimeOffRepository>();

            return services;
        }
    }
}
