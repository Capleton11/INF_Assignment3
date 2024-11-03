using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models
{
    public class CurrentLoanViewModel
    {
        public string BookName { get; set; }
        public string StudentName { get; set; }
        public DateTime? TakenDate { get; set; }
    }
}