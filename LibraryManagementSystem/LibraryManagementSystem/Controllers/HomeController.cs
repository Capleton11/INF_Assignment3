using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private LibraryEntities db = new LibraryEntities(); // Assuming LibraryEntities is your DbContext

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            ViewBag.Authors =  db.authors.ToList();
            ViewBag.Types = db.types.ToList();
            // Fetch books and students from the database
            var books = db.books.Include(b => b.author).Include(b => b.type).ToList();
            var students = db.students.ToList();

            // Paginate books and students
            var pagedBooks = books.Skip((page - 1) * pageSize).Take(pageSize);
            var pagedStudents = students.Skip((page - 1) * pageSize).Take(pageSize);

            // Create a tuple to pass to the view
            var model = new Tuple<IEnumerable<LibraryManagementSystem.Models.book>, IEnumerable<LibraryManagementSystem.Models.student>>(pagedBooks, pagedStudents);

            ViewBag.TotalBooks = books.Count;
            ViewBag.TotalStudents = students.Count;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(model);
        }
        public ActionResult CreateBook()
        {
            return RedirectToAction("Create", "Books");
        }

        public ActionResult CreateStudent()
        {
            return RedirectToAction("Create", "Students");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}