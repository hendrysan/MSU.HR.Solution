using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class EmployeePagination
    {
        public List<Employee> Employees { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
