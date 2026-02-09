using IBMS.Data;
using IBMS.Models;
using IBMS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IBMS.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return View(sales);
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
                return NotFound();

            return View(sale);
        }

        // ================= CREATE (GET) =================
        public IActionResult Create()
        {
            return View(new SaleDto
            {
                SaleDate = DateTime.UtcNow.Date,

                CustomerList = new SelectList(
                    _context.Customers,
                    "CustomerId",
                    "CustomerName"
                ),

                ProductList = new SelectList(
                    _context.Products,
                    "ProductId",
                    "ProductName"
                ),

                ProductPrices = _context.Products
                    .ToDictionary(p => p.ProductId, p => p.UnitPrice),

                ProductTaxRates = _context.Products
                    .ToDictionary(p => p.ProductId, p => p.TaxRate)
            });
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == dto.ProductId);

            if (product == null || stock == null || stock.CurrentStock < dto.Quantity)
            {
                ModelState.AddModelError("", "Insufficient stock.");

                dto.CustomerList = new SelectList(_context.Customers, "CustomerId", "CustomerName", dto.CustomerId);
                dto.ProductList = new SelectList(_context.Products, "ProductId", "ProductName", dto.ProductId);
                dto.ProductPrices = _context.Products.ToDictionary(p => p.ProductId, p => p.UnitPrice);
                dto.ProductTaxRates = _context.Products.ToDictionary(p => p.ProductId, p => p.TaxRate);

                return View(dto);
            }

            decimal total = product.UnitPrice * dto.Quantity;
            decimal tax = (total * product.TaxRate) / 100;

            var sale = new Sale
            {
                CustomerId = dto.CustomerId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                SaleDate = DateTime.Now,
                TotalAmount = total,
                TaxAmount = tax,
                NetAmount = total + tax
            };

            _context.Sales.Add(sale);
            stock.CurrentStock -= dto.Quantity;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT (GET) =================
        public async Task<IActionResult> Edit(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
                return NotFound();

            return View(new SaleDto
            {
                SaleId = sale.SaleId,
                CustomerId = sale.CustomerId,
                ProductId = sale.ProductId,
                Quantity = sale.Quantity,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                TaxAmount = sale.TaxAmount,
                NetAmount = sale.NetAmount,

                CustomerList = new SelectList(
                    _context.Customers,
                    "CustomerId",
                    "CustomerName",
                    sale.CustomerId
                ),

                ProductList = new SelectList(
                    _context.Products,
                    "ProductId",
                    "ProductName",
                    sale.ProductId
                ),

                ProductPrices = _context.Products
                    .ToDictionary(p => p.ProductId, p => p.UnitPrice),

                ProductTaxRates = _context.Products
                    .ToDictionary(p => p.ProductId, p => p.TaxRate)
            });
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaleDto dto)
        {
            var sale = await _context.Sales.FindAsync(dto.SaleId);
            if (sale == null)
                return NotFound();

            var product = await _context.Products.FindAsync(dto.ProductId);
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == dto.ProductId);

            if (product == null || stock == null)
                return NotFound();

            // restore old stock
            stock.CurrentStock += sale.Quantity;

            if (stock.CurrentStock < dto.Quantity)
            {
                ModelState.AddModelError("", "Insufficient stock.");

                dto.CustomerList = new SelectList(_context.Customers, "CustomerId", "CustomerName", dto.CustomerId);
                dto.ProductList = new SelectList(_context.Products, "ProductId", "ProductName", dto.ProductId);
                dto.ProductPrices = _context.Products.ToDictionary(p => p.ProductId, p => p.UnitPrice);
                dto.ProductTaxRates = _context.Products.ToDictionary(p => p.ProductId, p => p.TaxRate);

                return View(dto);
            }

            decimal total = product.UnitPrice * dto.Quantity;
            decimal tax = (total * product.TaxRate) / 100;

            sale.Quantity = dto.Quantity;
            sale.TotalAmount = total;
            sale.TaxAmount = tax;
            sale.NetAmount = total + tax;

            stock.CurrentStock -= dto.Quantity;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // ================= DELETE (POST ONLY – MODAL) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
                return NotFound();

            // restore stock
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == sale.ProductId);

            if (stock != null)
            {
                stock.CurrentStock += sale.Quantity;
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
