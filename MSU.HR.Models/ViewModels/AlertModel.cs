namespace MSU.HR.Models.ViewModels
{
    public class AlertModel
    {
        public AlertType Type { get; set; }
        public string Message { get; set; }
        public string StrongMessage { get; set; }

    }

    public enum AlertType
    {
        Success,
        Error,
        Warning,
        Info
    }
}
