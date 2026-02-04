using Dental_Clinic.Data;
using Dental_Clinic.Filters;
using Dental_Clinic.Models;
using Dental_Clinic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Clinic.Controllers
{
    [AuthorizeRole("Admin", "Receptionist")]
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;

            var query = _context.Invoices
                .OrderByDescending(x => x.InvoiceDate);

            var totalRecords = query.Count();

            var invoices = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<Invoice>
            {
                Items = invoices,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return View(model);
        }

        // GET
        public IActionResult Create()
        {
            var model = new Invoice
            {
                InvoiceNumber = GenerateInvoiceNumber()
            };
            model.Items.Add(new InvoiceItem()); 
            return View(model);
        }

        // POST — Save + Print
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Invoice model)
        {
            try
            {
                model.SubTotal = model.Items.Sum(x => x.Amount);
                model.TotalAmount = model.SubTotal - model.Discount;
                model.RemainingAmount = model.TotalAmount - model.PaidAmount;

                _context.Invoices.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Print", new { id = model.Id });
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }       
        }

        private string GenerateInvoiceNumber()
        {
            var lastInvoice = _context.Invoices
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            int nextNumber = lastInvoice == null ? 1 : lastInvoice.Id + 1;
            return $"INV-{DateTime.Now:yyyy}-{nextNumber:D4}";
        }


        // PRINT
        public IActionResult Print(int id)
        {
            var invoice = _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefault(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var invoice = _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefault(x => x.Id == id);

            if (invoice == null)
                return Json(new { success = false, message = "Invoice not found" });

            _context.InvoicesItem.RemoveRange(invoice.Items);
            _context.Invoices.Remove(invoice);
            _context.SaveChanges();

            return Json(new { success = true, message = "Invoice deleted successfully" });
        }

        public IActionResult Edit(int id)
        {
            var invoice = _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefault(x => x.Id == id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Invoice model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            var invoice = _context.Invoices
                .Include(x => x.Items)
                .FirstOrDefault(x => x.Id == model.Id);

            if (invoice == null)
                return NotFound();

            // Update header
            invoice.PatientName = model.PatientName;
            invoice.Phone = model.Phone;
            invoice.Discount = model.Discount;
            invoice.PaidAmount = model.PaidAmount;

            // Remove old items
            _context.InvoicesItem.RemoveRange(invoice.Items);

            // Add updated items
            invoice.Items = model.Items;

            // Recalculate
            invoice.SubTotal = model.Items.Sum(x => x.Amount);
            invoice.TotalAmount = invoice.SubTotal - invoice.Discount;
            invoice.RemainingAmount = invoice.TotalAmount - invoice.PaidAmount;

            _context.SaveChanges();

            TempData["success"] = "Invoice updated successfully!";
            return RedirectToAction("Print", new { id = invoice.Id });
        }



    }

}
