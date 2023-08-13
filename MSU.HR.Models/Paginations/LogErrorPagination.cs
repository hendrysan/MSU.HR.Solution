using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class LogErrorPagination
    {
        public List<LogError> logErrors { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
