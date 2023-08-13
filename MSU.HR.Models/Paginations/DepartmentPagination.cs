using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class DepartmentPagination
    {
        public List<Department> Departments { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
