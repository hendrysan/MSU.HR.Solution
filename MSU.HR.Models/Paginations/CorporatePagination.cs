using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class CorporatePagination
    {
        public List<Corporate> Corporates { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
