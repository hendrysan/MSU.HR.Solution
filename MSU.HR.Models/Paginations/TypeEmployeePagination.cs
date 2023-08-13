using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class TypeEmployeePagination
    {
        public List<TypeEmployee>  TypeEmployees{ get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
