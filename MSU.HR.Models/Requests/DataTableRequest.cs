namespace MSU.HR.Models.Requests
{
    public class DataTableRequest
    {
        public string? draw { get; set; }
        public string? sortColumn { get; set; }
        public string? sortColumnDirection { get; set; }
        public string? searchValue { get; set; }
        public int pageSize { get; set; } = 10;
        public int skip { get; set; } = 0;
    }
}
