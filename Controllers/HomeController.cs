using IBMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using IBMS.Data;
using Microsoft.AspNetCore.Authorization;

namespace IBMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //dasboard
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.TotalProducts = _context.Products.Count();

            ViewBag.TotalSales = _context.Sales.Any()? _context.Sales.Sum(s => s.NetAmount): 0;

            ViewBag.LowStock = _context.Stocks.Count(s => s.CurrentStock <= s.ReorderLevel);

            ViewBag.TotalCustomers = _context.Customers.Count();

            return View();
        }

    }
}
