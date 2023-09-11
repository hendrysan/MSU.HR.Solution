using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class DocumentAttendancePagination
    {
        public List<DocumentAttendance> Data { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
