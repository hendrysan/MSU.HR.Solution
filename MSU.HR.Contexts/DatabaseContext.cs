using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MSU.HR.Models.Entities;

namespace MSU.HR.Contexts
{
    public class DatabaseContext : IdentityUserContext<AspNetUser>
    {
        private readonly IConfiguration _configuration;
        public DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Area> Areas { get; set; }
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
        public DbSet<DocumentAttendance> DocumentAttendances { get; set; }
        public DbSet<DocumentAttendanceDetail> DocumentAttendanceDetails { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionUsed = _configuration.GetSection("ConnectionUsed").Value.ToString().ToLower();
            string connectionString = string.Empty;// "server=localhost; port=3306; database=dbpayroll; user=root; password=abcd.1234; Persist Security Info=False; Connect Timeout=300";
            //string connectionString = "server=localhost; port=3306; database=dbpayroll; user=root; password=abcd.1234; Persist Security Info=False; Connect Timeout=300";
            if (connectionUsed == "mysql")
            {
                connectionString = _configuration.GetConnectionString("MySQLConnection");
                string mySqlConnection = connectionString;
                options.UseMySQL(mySqlConnection);
            }
            else
            {
                throw new Exception("Connection Used Cannot Found OnConfiguration. ["+ connectionUsed +"] = "+ connectionString);
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{ }
    }
}
