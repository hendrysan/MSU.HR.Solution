namespace MSU.HR.Models.Others
{
    public class PaginationModel
    {
        public long TotalRecord { get; set; }
        public int TotalPage { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
