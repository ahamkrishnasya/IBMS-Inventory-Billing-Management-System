using IBMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IBMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult SalesReport()
        {
            var sales = _context.Sales
                .Include(s => s.Product)
                .Include(s => s.Customer)
                .ToList();

            return View(sales);
        }

        public IActionResult PurchaseReport()
        {
            var purchases = _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .ToList();

            return View(purchases);
        }

        public IActionResult InventoryReport()
        {
            var stock = _context.Stocks
                .Include(s => s.Product)
                .ToList();

            return View(stock);
        }
    }
}
