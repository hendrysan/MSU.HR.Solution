using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class TimeOffHistory
    {
        public Guid Id { get; set; }
        public Guid TimeOffId { get; set; }
        public Guid UserId { get; set; }
        
        [StringLength(150)]
        public string UserFullName { get; set; }

        [StringLength(150)]
        public string StatusId { get; set; }
        
        [StringLength(150)]
        public string Remarks { get; set; }
        
        public DateTime CreatedDate { get; set; }

    }
}
