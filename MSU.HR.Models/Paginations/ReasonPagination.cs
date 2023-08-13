using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class ReasonPagination
    {
        public List<Reason> Reasons { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
