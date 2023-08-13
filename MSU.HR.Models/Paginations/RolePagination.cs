using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class RolePagination
    {
        public List<Role> Roles { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
