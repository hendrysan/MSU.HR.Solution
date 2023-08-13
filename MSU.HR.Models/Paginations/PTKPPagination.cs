using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class PTKPPagination
    {
        public List<PTKP> PTKPs { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
