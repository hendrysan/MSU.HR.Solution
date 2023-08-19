using MSU.HR.Models.Entities;
using System.Security.Claims;

namespace MSU.HR.Services.Interfaces
{
    public interface IToken
    {
        DateTime GetRefreshTokenExpiryTime();
        //DateTime GetRefreshTokenExpiryDay();
        string CreateToken(AspNetUser user, Corporate? corporate, Role? role, Employee? employee, DateTime expiryTime);
        List<Claim> CreateClaims(AspNetUser user, Corporate? corporate, Role? role, Employee? employee);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
        string GenerateRefreshToken();
    }
}
