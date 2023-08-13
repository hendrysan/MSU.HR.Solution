using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class EducationPagination
    {
        public List<Education> Educations { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
