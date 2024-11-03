using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models
{
    public class HomeViewModel
    {
        public List<student> Students { get; set; }
        public List<BookViewModel> Books { get; set; }
    }
}