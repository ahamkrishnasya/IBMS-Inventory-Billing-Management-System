using IBMS.Data;
using IBMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // ===================== CREATE (GET) =====================
        public IActionResult Create()
        {
            return View();
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Customer created successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT (GET) =====================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // ===================== EDIT (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();

                TempData["AlertMessage"] = "Customer updated successfully.";
                TempData["AlertType"] = "success";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerId))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE (GLOBAL MODAL) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                TempData["AlertMessage"] = "Customer deleted successfully.";
                TempData["AlertType"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== EXISTS =====================
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
