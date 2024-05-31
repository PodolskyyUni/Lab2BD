using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2BDDomain.Model;
using Lab2BDInfrastructure;
using Microsoft.Data.SqlClient;

namespace Lab2BDInfratsrtcture.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Isttp1FvContext _context;

        public OrdersController(Isttp1FvContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var isttp1FvContext = _context.Orders.Include(o => o.Client);
            return View(await isttp1FvContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ClientName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,OrderStatus,OrderDate,ApprComplDate,OrderPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ClientName", order.ClientId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ClientName", order.ClientId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,OrderStatus,OrderDate,ApprComplDate,OrderPrice")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "ClientName", order.ClientId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private string ToAscii(string input)
        {
            var asciiBytes = System.Text.Encoding.ASCII.GetBytes(input);
            return System.Text.Encoding.ASCII.GetString(asciiBytes);
        }

        public IActionResult SearchOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate == null || endDate == null)
            {
                ViewBag.Message = "Будь ласка, введіть діапазон дат.";
                return View("NoResults");
            }

            string sqlQuery = @"
                SELECT *
                FROM Orders
                WHERE OrderDate BETWEEN {0} AND {1}";

            var orders = _context.Orders
                .FromSqlRaw(sqlQuery, startDate, endDate)
                .Include(o => o.Client)
                .ToList();

            if (!orders.Any())
            {
                ViewBag.Message = "Немає замовлень в зазначеному діапазоні.";
                return View("NoResults");
            }

            return View("SearchOrdersByDateRange", orders);
        }


        // Method for searching factories by product price and amount produced
        public IActionResult SearchFactoriesByProductPriceAndAmountProduced(decimal minPrice, decimal maxPrice, int minAmountProduced)
        {
            if (minPrice <= 0 || maxPrice <= 0 || minAmountProduced <= 0)
            {
                ViewBag.Message = "Будь ласка, введіть діапазон цін та мінімальну кількість.";
                return View("NoResults");
            }

            string sqlQuery = $@"
        SELECT f.Id, f.Adress, f.FactoryName
        FROM Factories f
        JOIN FactoryProducts fp ON f.Id = fp.FactoryId
        JOIN Products p ON fp.ProductId = p.Id
        WHERE fp.ProductionCost BETWEEN {minPrice} AND {maxPrice} AND fp.AmountProduced >= {minAmountProduced}";

            var factories = _context.Factories
                .FromSqlRaw(sqlQuery)
                .Select(f => new { f.Id, f.Adress, f.FactoryName })
                .ToList();

            var distinctAddresses = factories.Select(f => f.Adress).Distinct().ToList();

            if (!distinctAddresses.Any())
            {
                ViewBag.Message = "Немає фабрик з зазначеним діапазоном цін та мінімальною кількістю.";
                return View("NoResults");
            }

            return View("SearchFactoriesByProductPriceAndAmountProduced", distinctAddresses);
        }




    }
}
