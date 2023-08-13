using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class GradePagination
    {
        public List<Grade> Grades { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
