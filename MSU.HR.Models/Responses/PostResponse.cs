namespace MSU.HR.Models.Responses
{
    public class PostResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; }
    }
}
