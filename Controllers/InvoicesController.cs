using IBMS.Data;
using IBMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IBMS.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //get invoices
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Sale)
                .ThenInclude(s => s.Customer)
                .ToListAsync();

            return View(invoices);
        }

        // invoice by id
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Sale)
                .ThenInclude(s => s.Customer)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null) return NotFound();

            return View(invoice);
        }

        // add invoice 
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var invoiceDate = new Invoice
            {
                InvoiceDate = DateTime.UtcNow.Date
            };
            await LoadSales();
            return View(invoiceDate);
        }

        // add invoice post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Invoice created successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // edit invoice get
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            await LoadSales(invoice.SaleId); 
            return View(invoice);
        }

        // edit invoice post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Invoice invoice)
        {
            _context.Update(invoice);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Invoice updated successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // delete invoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound(); 

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Invoice deleted successfully.";
            TempData["AlertType"] = "danger";

            return RedirectToAction(nameof(Index));
        }

        // dropdown
        private async Task LoadSales(int? selectedId = null)
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Select(s => new
                {
                    s.SaleId,
                    DisplayText = "#" + s.SaleId +
                                  " - " + s.Customer.CustomerName +
                                  " - ₹" + s.TotalAmount,
                    s.TotalAmount
                })
                .ToListAsync();

            ViewBag.Sales = new SelectList(sales, "SaleId", "DisplayText", selectedId);
        }
    }
}