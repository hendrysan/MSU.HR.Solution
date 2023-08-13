using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class ShiftPagination
    {
        public List<Shift> Shifts { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
