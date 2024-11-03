using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class MaintainController : Controller
    {
        private LibraryEntities db = new LibraryEntities();

        // GET: Maintain
        public async Task<ActionResult> Index(int pageType = 1, int pageAuthor = 1, int pageBorrow = 1, int pageSize = 10)
        {
            // Calculate the total count of each item type
            ViewBag.TotalTypes = db.types.Count();
            ViewBag.TotalAuthors = db.authors.Count();
            ViewBag.TotalBorrows = db.borrows.Count();

            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPageType = pageType;
            ViewBag.CurrentPageAuthor = pageAuthor;
            ViewBag.CurrentPageBorrow = pageBorrow;

            // Fetch paginated lists
            var types = db.types.OrderBy(t => t.typeId).Skip((pageType - 1) * pageSize).Take(pageSize).ToList();
            var authors = db.authors.OrderBy(a => a.authorId).Skip((pageAuthor - 1) * pageSize).Take(pageSize).ToList();
            var borrows = db.borrows
                            .Include(b => b.book)
                            .Include(b => b.student)
                            .OrderBy(b => b.borrowId)
                            .Skip((pageBorrow - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            // Create a tuple to pass to the view
            var model = new Tuple<IEnumerable<LibraryManagementSystem.Models.type>,
                                  IEnumerable<LibraryManagementSystem.Models.author>,
                                  IEnumerable<LibraryManagementSystem.Models.borrow>>(types, authors, borrows);

            return View(model);
        }

        // Author CRUD Actions
        public async Task<ActionResult> AuthorIndex()
        {
            return View(await db.authors.ToListAsync());
        }

        public async Task<ActionResult> AuthorDetails(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var author = await db.authors.FindAsync(id);
            if (author == null) return HttpNotFound();
            return View(author);
        }

        public ActionResult AuthorCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorCreate(author author)
        {
            if (ModelState.IsValid)
            {
                db.authors.Add(author);
                await db.SaveChangesAsync();
                return RedirectToAction("AuthorIndex");
            }
            return View(author);
        }

        public async Task<ActionResult> AuthorEdit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var author = await db.authors.FindAsync(id);
            if (author == null) return HttpNotFound();
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorEdit(author author)
        {
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("AuthorIndex");
            }
            return View(author);
        }

        public async Task<ActionResult> AuthorDelete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var author = await db.authors.FindAsync(id);
            if (author == null) return HttpNotFound();
            return View(author);
        }

        [HttpPost, ActionName("AuthorDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorDeleteConfirmed(int id)
        {
            var author = await db.authors.FindAsync(id);
            db.authors.Remove(author);
            await db.SaveChangesAsync();
            return RedirectToAction("AuthorIndex");
        }

        // Similar CRUD actions for Types
        // GET, POST, EDIT, DELETE actions for `types` can be added here
        // ...
        public async Task<ActionResult> TypesIndex()
        {
            return View(await db.types.ToListAsync());
        }

        // GET: types/Details/5
        public async Task<ActionResult> TypesDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = await db.types.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // GET: types/Create
        public ActionResult TypesCreate()
        {
            return View();
        }

        // POST: types/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TypesCreate([Bind(Include = "typeId,name")] type type)
        {
            if (ModelState.IsValid)
            {
                db.types.Add(type);
                await db.SaveChangesAsync();
                return RedirectToAction("TypesIndex");
            }

            return View(type);
        }

        // GET: types/Edit/5
        public async Task<ActionResult> TypesEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = await db.types.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // POST: types/Edit/5
        [HttpPost]
        public async Task<ActionResult> TypesEdit(type type)
        {
            if (ModelState.IsValid)
            {
                db.Entry(type).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(type);
        }
        // GET: types/Delete/5
        public async Task<ActionResult> TypesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            type type = await db.types.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }
        [HttpPost]
        public async Task<ActionResult> TypesDelete(int typeId)
        {
            var type = await db.types.FindAsync(typeId);
            if (type != null)
            {
                db.types.Remove(type);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        // Borrow CRUD Actions
        public async Task<ActionResult> BorrowIndex()
        {
            var borrows = db.borrows.Include(b => b.book).Include(b => b.student);
            return View(await borrows.ToListAsync());
        }

        public async Task<ActionResult> BorrowDetails(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var borrow = await db.borrows.FindAsync(id);
            if (borrow == null) return HttpNotFound();
            return View(borrow);
        }

        public ActionResult BorrowCreate()


        {
            ViewBag.Books = db.books.ToList();
            ViewBag.Students = db.students.ToList();
            ViewBag.bookId = new SelectList(db.books, "bookId", "name");
            ViewBag.studentId = new SelectList(db.students, "studentId", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowCreate(borrow borrow)
        {
            if (ModelState.IsValid)
            {
                db.borrows.Add(borrow);
                await db.SaveChangesAsync();
                return RedirectToAction("BorrowIndex");
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrow.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrow.studentId);
            return View(borrow);
        }

        public async Task<ActionResult> BorrowEdit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var borrow = await db.borrows.FindAsync(id);
            if (borrow == null) return HttpNotFound();
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrow.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrow.studentId);
            return View(borrow);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowEdit(borrow borrow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrow).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("BorrowIndex");
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrow.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrow.studentId);
            return View(borrow);
        }

        public async Task<ActionResult> BorrowDelete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var borrow = await db.borrows.FindAsync(id);
            if (borrow == null) return HttpNotFound();
            return View(borrow);
        }

        [HttpPost, ActionName("BorrowDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowDeleteConfirmed(int id)
        {
            var borrow = await db.borrows.FindAsync(id);
            db.borrows.Remove(borrow);
            await db.SaveChangesAsync();
            return RedirectToAction("BorrowIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
