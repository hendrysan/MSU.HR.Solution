namespace MSU.HR.Models.Responses
{
    public class DataTableResponse
    {
        public string? draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }        
        public object data { get; set; }
    }
}
