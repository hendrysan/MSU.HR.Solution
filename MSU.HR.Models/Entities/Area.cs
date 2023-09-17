using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class Area
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
