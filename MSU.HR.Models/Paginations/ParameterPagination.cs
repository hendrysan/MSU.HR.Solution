using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class ParameterPagination
    {
        public List<Parameter> Parameters { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
