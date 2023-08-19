using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MSU.HR.Models.Entities;
using MSU.HR.Services.Interfaces;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MSU.HR.Services.Repositories
{
    public class TokenRepository : IToken
    {
        private readonly IConfiguration _configuration;
        //private readonly int ExpirationMinutes = 60;

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DateTime GetRefreshTokenExpiryTime()
        {
            int expirationMinutes = Convert.ToInt32(_configuration.GetSection("Jwt:RefreshTokenExpirationMinutes").Value);
            return DateTime.Now.AddMinutes(expirationMinutes);
        }

        //public DateTime GetRefreshTokenExpiryDay()
        //{
        //    int expirationDays = Convert.ToInt32(_configuration.GetSection("Jwt:RefreshTokenValidityInDays").Value);
        //    return DateTime.Now.AddDays(expirationDays);
        //}

        public string CreateToken(AspNetUser user, Corporate? corporate, Role? role, Employee? employee, DateTime expiryTime)
        {
            var expiration = expiryTime;
            var token = CreateJwtToken(
                CreateClaims(user, corporate, role, employee),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenWrite = tokenHandler.WriteToken(token);
            return tokenWrite.ToString();
        }

        private static JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
         DateTime expiration) =>
         new(
             "apiWithAuthBackend",
             "apiWithAuthBackend",
             claims,
             expires: expiration,
             signingCredentials: credentials
         );

        public List<Claim> CreateClaims(AspNetUser user, Corporate? corporate, Role? role, Employee? employee)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("Id", user.Id),
                    new Claim("UserName", user.UserName),
                    new Claim("FullName", user.FullName),
                    new Claim("Email", user.Email),
                    new Claim("Code", employee == null ? "0": employee.Code),
                    new Claim("CorporateId", corporate == null ? "0": corporate.Id.ToString()),
                    new Claim("CorporateName", corporate == null ? "0" : corporate.Name),
                    new Claim("RoleId", role == null ? "0" : role.Id.ToString()),
                    new Claim("RoleName", role == null ? "0" : role.Name),
                    new Claim("LastLogin", DateTime.Now.ToString())
                };

                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
