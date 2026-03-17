using IBMS.Data;
using IBMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //get products
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        //get product by id
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
        }

        //add product get
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //add product post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _context.Stocks.Add(new Stock
            {
                ProductId = product.ProductId,
                CurrentStock = product.Quantity,
                ReorderLevel = 5
            });

            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Product added successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        //edit product get
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        //edit product post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Product updated successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        //delete product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
                _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Product removed successfully.";
            TempData["AlertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    }
}
