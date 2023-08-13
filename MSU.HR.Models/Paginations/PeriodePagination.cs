using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class PeriodPagination
    {
        public List<Period> Periods { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
