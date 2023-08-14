using System.Security.Claims;

namespace MSU.HR.Models.Others
{
    public class UserIdentityModel
    {
        public Guid Id { get; }
        public string UserName { get; }
        public string FullName { get; }
        public string Email { get; }
        public string Code { get; }
        public string CorporateId { get; }
        public string CorporateName { get; }
        public string RoleId { get; }
        public string RoleName { get; }
        public DateTime LastLogin { get; }

        public UserIdentityModel(ClaimsIdentity? identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
            if (identity.IsAuthenticated)
            {
                this.Id = Guid.Parse(identity.FindFirst("Id")?.Value.ToString());
                this.UserName = identity.Name ?? string.Empty;
                this.FullName = identity.FindFirst("FullName")?.Value ?? string.Empty;
                this.Email = identity.FindFirst("Email")?.Value ?? string.Empty;
                this.Code = identity.FindFirst("Code")?.Value ?? string.Empty;
                this.CorporateId = identity.FindFirst("CorporateId")?.Value;
                this.CorporateName = identity.FindFirst("CorporateName")?.Value ?? string.Empty;
                this.RoleId = identity.FindFirst("RoleId")?.Value;
                this.RoleName = identity.FindFirst("RoleName")?.Value ?? string.Empty;
                this.LastLogin = Convert.ToDateTime(identity.FindFirst("LastLogin")?.Value);
            }
        }
    }
}
