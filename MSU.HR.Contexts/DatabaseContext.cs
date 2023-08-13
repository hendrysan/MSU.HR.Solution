using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MSU.HR.Models.Entities;

namespace MSU.HR.Contexts
{
    public class DatabaseContext : IdentityUserContext<AspNetUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Corporate> Corporates { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<LogError> LogErrors { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<PTKP> PTKPs { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleAccess> RoleAccesses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<TimeOff> TimeOffs { get; set; }
        public DbSet<TimeOffHistory> TimeOffHistories { get; set; }
        public DbSet<TypeEmployee> TypeEmployees { get; set; }

        private readonly IConfiguration _configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionUsed = _configuration.GetSection("ConnectionUsed").Value.ToString().ToLower();



            if (connectionUsed == "mysql")
            {
                string mySqlConnection = _configuration.GetConnectionString("MySQLConnection");
                options.UseMySQL(mySqlConnection);
            }
            else
            {
                throw new Exception("Connection Used Cannot Found OnConfiguration");
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{ }
    }
}
