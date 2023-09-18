namespace MSU.HR.Models.Entities
{
    public class Inbox
    {
        public Guid Id { get; set; }
        public string? Sender { get; set; }
        public string? Receiver { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsDraft { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsImportant { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

    }
}
