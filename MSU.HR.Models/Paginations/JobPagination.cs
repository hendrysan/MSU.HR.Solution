using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class JobPagination
    {
        public List<Job> Jobs { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
