using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Claims;

namespace MSU.HR.Services.Repositories
{
    public class LogErrorRepository : ILogError
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly IConfiguration _configuration;
        public LogErrorRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userIdentity = new UserIdentityModel(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            _configuration = configuration;
        }

        public async Task<int> SaveAsync(Exception ex, string body)
        {
            var connectionString = _configuration.GetConnectionString("MySQLConnection");
            using IDbConnection connection = new MySqlConnection(connectionString);

            LogError entity = new LogError();
            var request = _httpContextAccessor.HttpContext.Request;
            var ip = request.Host.Value;
            var ipClient = request.HttpContext.Connection.RemoteIpAddress.ToString();
            var queryString = request.QueryString.ToString();

            var userAgent = request.Headers["User-Agent"].ToString();
            string msg = "";
            string type = "";
            string src = "";
            if (ex != null)
            {
                msg = ex.Message;
                type = ex.GetType().Name.ToString();
                src = ex.Source;
            }
            entity.Id = Guid.NewGuid();
            entity.Message = msg;
            entity.Type = type;
            entity.Source = src;
            entity.URL = request.Path;
            entity.Parameter = queryString;
            entity.ParameterBody = body;
            entity.Browser = userAgent;
            entity.IP = ip;
            entity.IPClient = ipClient;


            entity.UserAgent = userIdentity.Id.ToString();
            entity.CreatedDate = DateTime.Now;

            //_context.LogErrors.Add(entity);
            //return await _context.SaveChangesAsync();

            var count = await connection.ExecuteAsync(@"INSERT INTO LogErrors (Id,Message,Type,Source,URL,Parameter,ParameterBody,Browser,IP,IPClient,UserAgent,CreatedDate)  
                    VALUES (@Id,@Message,@Type,@Source,@URL,@Parameter,@ParameterBody,@Browser,@IP,@IPClient,@UserAgent,@CreatedDate)",
                    new
                    {
                        Id = entity.Id,
                        Message = entity.Message,
                        Type = entity.Type,
                        Source = entity.Source,
                        URL = entity.URL,
                        Parameter = entity.Parameter,
                        ParameterBody = entity.ParameterBody,
                        Browser = entity.Browser,
                        IP = entity.IP,
                        IPClient = entity.IPClient,
                        UserAgent = entity.UserAgent,
                        CreatedDate = entity.CreatedDate
                    });
            return count;

        }
    }
}
