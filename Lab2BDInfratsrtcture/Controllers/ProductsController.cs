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
using System.Data;

namespace Lab2BDInfratsrtcture.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Isttp1FvContext _context;

        public ProductsController(Isttp1FvContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,ProductDescription,MarketPrice")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,ProductDescription,MarketPrice")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        public IActionResult FindProductsNotOrderedByClientXAndNotProducedMoreThanXInFactories(string clientXName, int maxProduction)
        {
            if (string.IsNullOrEmpty(clientXName))
            {
                ViewBag.Message = "Будь ласка, введіть ім'я клієнта.";
                return View("NoResults");
            }

            // Combined SQL Query with both conditions
            string sqlQuery = @"
        WITH ClientCTE AS (
            SELECT Id AS ClientId
            FROM Clients
            WHERE CAST(ClientName AS NVARCHAR(MAX)) = @ClientXName
        )
        SELECT p.*
        FROM Products p
        WHERE NOT EXISTS (
            SELECT 1
            FROM ReqProducts rp
            JOIN Orders o ON rp.OrderId = o.Id
            JOIN ClientCTE c ON o.ClientId = c.ClientId
            WHERE rp.ProductId = p.Id
        )
        AND p.Id NOT IN (
            SELECT fp.ProductId
            FROM FactoryProducts fp
            WHERE fp.AmountProduced > @MaxProduction
        )";

            try
            {
                // Create SQL parameters with explicit types
                var clientXNameParam = new SqlParameter("@ClientXName", SqlDbType.NVarChar, 256) { Value = clientXName };
                var maxProductionParam = new SqlParameter("@MaxProduction", SqlDbType.Int) { Value = maxProduction };

                // Execute the query
                var products = _context.Products
                    .FromSqlRaw(sqlQuery, clientXNameParam, maxProductionParam)
                    .ToList();

                if (!products.Any())
                {
                    ViewBag.Message = "Немає продуктів, які не були замовлені клієнтом і не виробляються на фабриках з виробництвом більше за вказану кількість.";
                    return View("NoResults");
                }

                return View("FindProductsNotOrderedByClientXAndNotProducedMoreThanXInFactories", products);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Виникла помилка при виконанні основного запиту: " + ex.Message;
                return View("NoResults");
            }
        }


    }
}
