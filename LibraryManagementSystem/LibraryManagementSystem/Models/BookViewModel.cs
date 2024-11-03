using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public int? PageCount { get; set; }
        public string Status { get; set; }
    }
}