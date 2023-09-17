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
using System.Text.Json;

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

        public async Task<int> SaveAsync(Exception ex, object body)
        {
            var connectionString = _configuration.GetConnectionString("MySQLConnection");
            using IDbConnection connection = new MySqlConnection(connectionString);

            LogError entity = new LogError();
            var httpContext = _httpContextAccessor.HttpContext;
            string _body = JsonSerializer.Serialize(body);

            if (httpContext != null)
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var ip = request.Host.Value;
                var ipClient = request.HttpContext.Connection.RemoteIpAddress.ToString();
                var queryString = request.QueryString.ToString();
                var userAgent = request.Headers["User-Agent"].ToString();

                entity.URL = request.Path;
                entity.Parameter = queryString;
                entity.Browser = userAgent;
                entity.IP = ip;
                entity.IPClient = ipClient;
            }




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

            entity.ParameterBody = _body;


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
