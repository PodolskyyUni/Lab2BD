using Lab2BDInfrastructure;
using Lab2BDInfratsrtcture.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lab2BDInfratsrtcture.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Isttp1FvContext _context;
        public HomeController(ILogger<HomeController> logger, Isttp1FvContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            var clients = _context.Clients.Select(a => a.ClientName).ToList().Distinct().ToList();
            var factories = _context.Factories.Select(p => p.FactoryName).ToList().Distinct().ToList();
            var orders = _context.Orders.Select(s => s.Id).Distinct().ToList();
            var products = _context.Products.Select(d => new { d.Id, d.ProductName }).ToList().Distinct().ToList();
            var reqproducts = _context.ReqProducts.Select(q => q.Id).Distinct().ToList();
            var facproducts = _context.FactoryProducts.Select(l => l.Id).ToList().Distinct().ToList();

            ViewBag.Clients = clients;
            ViewBag.Factories = factories;
            ViewBag.Orders = orders;
            ViewBag.Products = products;
            ViewBag.ReqProducts = reqproducts;
            ViewBag.FactoryProducts = facproducts;

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
