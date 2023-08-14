namespace MSU.HR.Models.Others
{
    public class ErrorModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public object Data { get; set; }

        //public static ErrorModel Error(string message)
        //{
        //     return new ErrorModel() { Message = message, IsSuccess = false };
        //}

        //public static ErrorModel BadRequest(string message, int errorCode = 400)
        //{
        //    return new ErrorModel() { IsSuccess = false, Message = message, ErrorCode = errorCode };
        //}
    }
}
