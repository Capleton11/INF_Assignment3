using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace LibraryManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly LibraryEntities db = new LibraryEntities();

        // Action for Current Loans Report
        public async Task<ActionResult> CurrentLoansReport()
        {
            var currentLoans = await db.borrows
                .Include(b => b.book)
                .Include(b => b.student)
                .Where(b => b.broughtDate == null)
                .Select(b => new CurrentLoanViewModel
                {
                    BookName = b.book.name,
                    StudentName = b.student.name + " " + b.student.surname,
                    TakenDate = b.takenDate
                }).ToListAsync();

            return View("CurrentLoansReport", currentLoans);
        }
        public ActionResult SaveCurrentLoansReport()
        {
            // Query the data
            var currentLoans = db.borrows
                .Where(b => b.broughtDate == null)
                .Select(b => new {
                    BookName = b.book.name,
                    StudentName = b.student.name + " " + b.student.surname,
                    TakenDate = b.takenDate
                }).ToList();

            // Set up PDF document
            using (var ms = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph("Current Loans Report"));
                document.Add(new Paragraph("\n"));

                // Table setup
                PdfPTable table = new PdfPTable(3) { WidthPercentage = 100 };
                table.AddCell("Book Name");
                table.AddCell("Student Name");
                table.AddCell("Taken Date");

                // Populate the table with data
                foreach (var loan in currentLoans)
                {
                    table.AddCell(loan.BookName);
                    table.AddCell(loan.StudentName);
                    table.AddCell(loan.TakenDate?.ToString("yyyy-MM-dd"));
                }

                // Add table to document
                document.Add(table);
                document.Close();

                // Return file as a downloadable PDF
                return File(ms.ToArray(), "application/pdf", "CurrentLoansReport.pdf");
            }
        }
    }
}