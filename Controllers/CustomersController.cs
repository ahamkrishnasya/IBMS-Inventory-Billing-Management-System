using IBMS.Data;
using IBMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBMS.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //get customers
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        //get customers by id
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        //add customers get
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //add customer post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Customer added successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        //edit customer get
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        //edit customer post
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

        //delete customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                TempData["AlertMessage"] = "Customer removed successfully.";
                TempData["AlertType"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        //customer exit
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
