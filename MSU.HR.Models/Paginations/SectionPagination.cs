using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class SectionPagination
    {
        public List<Section> Sections { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
