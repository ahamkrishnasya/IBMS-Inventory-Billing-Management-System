using IBMS.Data;
using IBMS.Models;
using IBMS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IBMS.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //get sales
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return View(sales);
        }

        //get sale by id
        [HttpGet]
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

        //add sale get
        [HttpGet]
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

        //add sale post
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

            TempData["AlertMessage"] = "Sales added successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        //edit sale get
        [HttpGet]
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

        //edit sale post
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

            TempData["AlertMessage"] = "Sales updated successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        //delete sale
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

            var invoice = await _context.Invoices.Where(x => x.SaleId == id).FirstOrDefaultAsync();

            if(invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Sales removed successfully.";
            TempData["AlertType"] = "danger";
            return RedirectToAction(nameof(Index));
        }

    }
}
