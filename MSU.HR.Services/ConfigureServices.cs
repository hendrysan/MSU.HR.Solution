using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;

namespace MSU.HR.Services
{
    public static class ConfigureServices
    {
        public static IConfiguration? Configuration { get; }
        public static IServiceCollection AddInterfaceServices(this IServiceCollection services, IConfiguration configuration)
        {
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

            services.AddScoped<IAttendance, AttendanceRepository>();

            return services;
        }
    }
}
