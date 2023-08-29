using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.ViewModels
{
    public class PaidLeaveDetailModel
    {
        public TimeOff? TimeOff { get; set; }
        public Employee? UserRequest { get; set; }
        public Employee? UserSuperior { get; set; }
        public Employee? CurrentUser { get; set; }
        public IEnumerable<TimeOffHistory>? TimeOffHistories { get; set; }

        public Guid Id { get; set; }
        public string ActionSubmit { get; set; }
        public string Remarks { get; set; }

    }
}
