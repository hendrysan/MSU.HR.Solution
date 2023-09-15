using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MSU.HR.Contexts;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;
using System.Security.Claims;

namespace MSU.HR.UnitTests
{
    public abstract class BaseContextInjection
    {
        public readonly DatabaseContext _context;
        public readonly IConfigurationRoot _configuration;
        public readonly ILogError _logError;
        public readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IToken _token;

        public BaseContextInjection()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<DatabaseContext>().Options;

            _context = new DatabaseContext(dbOption, _configuration);
            this._httpContextAccessor = SetupHttpContextAccessor(this._httpContextAccessor, this._token);

            _logError = new LogErrorRepository(_context, _httpContextAccessor, _configuration);

        }

        IHttpContextAccessor SetupHttpContextAccessor(IHttpContextAccessor httpContextAccessor, IToken _token)
        {
            _token = new TokenRepository(_configuration);

            var claims = _token.CreateClaims(new Models.Entities.AspNetUser()
            {
                Id = Guid.Empty.ToString(),
                UserName = "UT",
                FullName = "Unit Test",
                Email = "ut@test.com",
            }, null, null, null);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);


            Mock<IHttpContextAccessor> mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.User = principal;

            mockHttpContextAccessor
                .SetupGet(accessor => accessor.HttpContext)
                .Returns(httpContext);

            return mockHttpContextAccessor.Object;

        }

        Mock<IHttpContextAccessor> SetupHttpContextAccessorWithUrl(string currentUrl)
        {
            Mock<IHttpContextAccessor> mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            SetRequestUrl(httpContext.Request, currentUrl);

            mockHttpContextAccessor
                .SetupGet(accessor => accessor.HttpContext)
                .Returns(httpContext);

            static void SetRequestUrl(HttpRequest httpRequest, string url)
            {
                UriHelper
                    .FromAbsolute(url, out var scheme, out var host, out var path, out var query,
                        fragment: out var _);

                httpRequest.Scheme = scheme;
                httpRequest.Host = host;
                httpRequest.Path = path;
                httpRequest.QueryString = query;
            }

            return mockHttpContextAccessor;
        }

        async void AuthAccessor()
        {





            //await HttpContext.(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
