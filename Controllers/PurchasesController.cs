using IBMS.Data;
using IBMS.Models;
using IBMS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IBMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            return View(await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .ToListAsync());
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseId == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            var data = new PurchaseDto
            {
                PurchaseDate = DateTime.UtcNow.Date,
                SupplierList = _context.Suppliers
                    .Select(s => new SelectListItem
                    {
                        Value = s.SupplierId.ToString(),
                        Text = s.SupplierName
                    }).ToList(),

                ProductList = _context.Products
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = p.ProductName
                    }).ToList(),

                ProductPrices = _context.Products
                    .ToDictionary(p => p.ProductId, p => p.UnitPrice)
            };
            return View(data);
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                PurchaseDate = DateTime.Now,
                TotalAmount = product.UnitPrice * dto.Quantity 
            };

            _context.Purchases.Add(purchase);

            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == dto.ProductId);

            if (stock == null)
            {
                _context.Stocks.Add(new Stock
                {
                    ProductId = dto.ProductId,
                    CurrentStock = dto.Quantity,
                    ReorderLevel = 10
                });
            }
            else
            {
                stock.CurrentStock += dto.Quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Purchases/Edit/Id
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null) return NotFound();

            var data = new PurchaseDto
            {
                PurchaseId = purchase.PurchaseId,
                SupplierId = purchase.SupplierId,
                ProductId = purchase.ProductId,
                Quantity = purchase.Quantity,
                PurchaseDate = purchase.PurchaseDate.Date,
                TotalAmount = purchase.TotalAmount,

                SupplierList = _context.Suppliers
                    .Select(s => new SelectListItem
                    {
                        Value = s.SupplierId.ToString(),
                        Text = s.SupplierName
                    }).ToList(),

                ProductList = _context.Products
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = p.ProductName
                    }).ToList(),

                ProductPrices = _context.Products
                    .ToDictionary(p => p.ProductId, p => p.UnitPrice)
            };
            return View(data);
        }


        // POST: Purchases/Edit/Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PurchaseDto dto)
        {
            var purchase = await _context.Purchases.FindAsync(dto.PurchaseId);
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (purchase == null || product == null)
                return NotFound();

            purchase.SupplierId = dto.SupplierId;
            purchase.ProductId = dto.ProductId;
            purchase.Quantity = dto.Quantity;
            purchase.PurchaseDate = dto.PurchaseDate;
            purchase.TotalAmount = product.UnitPrice * dto.Quantity;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseId == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.PurchaseId == id);
        }
    }
}
