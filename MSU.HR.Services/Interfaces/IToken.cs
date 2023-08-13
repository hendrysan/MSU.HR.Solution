using MSU.HR.Models.Entities;
using System.Security.Claims;

namespace MSU.HR.Services.Interfaces
{
    public interface IToken
    {
        string CreateToken(AspNetUser user, Corporate? corporate, Role? role, Employee? employee);
        List<Claim> CreateClaims(AspNetUser user, Corporate? corporate, Role? role, Employee? employee);
    }
}
