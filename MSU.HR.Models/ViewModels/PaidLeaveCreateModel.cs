using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.ViewModels
{
    public class PaidLeaveCreateModel
    {
        public int RemainingAllowance { get; set; }
        public IEnumerable<DropdownModel> ReasonDropdown { get; set; }

        [Required]
        [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required]
        public Guid ReasonId { get; set; }
        public string? Notes { get; set; }
        [Required]
        public int TemporaryAllowance { get; set; }
        [Required]
        public int Taken { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string UserCode { get; set; }
        [Required]
        public string UserFullName { get; set; }
        public Employee EmployeeInfo { get; set; }
        public string SuperiorityInfo { get; set; }

    }
}
