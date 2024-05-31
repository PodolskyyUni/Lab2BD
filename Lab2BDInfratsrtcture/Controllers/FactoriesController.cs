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
    public class FactoriesController : Controller
    {
        private readonly Isttp1FvContext _context;

        public FactoriesController(Isttp1FvContext context)
        {
            _context = context;
        }

        // GET: Factories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Factories.ToListAsync());
        }

        // GET: Factories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factory = await _context.Factories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (factory == null)
            {
                return NotFound();
            }

            return View(factory);
        }

        // GET: Factories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Factories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Adress,Maintenance,FactoryName")] Factory factory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(factory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(factory);
        }

        // GET: Factories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factory = await _context.Factories.FindAsync(id);
            if (factory == null)
            {
                return NotFound();
            }
            return View(factory);
        }

        // POST: Factories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Adress,Maintenance,FactoryName")] Factory factory)
        {
            if (id != factory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FactoryExists(factory.Id))
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
            return View(factory);
        }

        // GET: Factories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factory = await _context.Factories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (factory == null)
            {
                return NotFound();
            }

            return View(factory);
        }

        // POST: Factories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factory = await _context.Factories.FindAsync(id);
            if (factory != null)
            {
                _context.Factories.Remove(factory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FactoryExists(int id)
        {
            return _context.Factories.Any(e => e.Id == id);
        }
        public IActionResult FindFactoriesProducingClientProducts(string clientXName)
        {
            if (string.IsNullOrEmpty(clientXName))
            {
                ViewBag.Message = "Будь ласка, введіть ім'я клієнта.";
                return View("NoResults");
            }

            // Step 1: Retrieve the ClientId for the given client name
            var clientId = _context.Clients
                .Where(c => EF.Functions.Like(c.ClientName, clientXName))
                .Select(c => c.Id)
                .FirstOrDefault();

            if (clientId == 0)
            {
                ViewBag.Message = "Клієнт з таким ім'ям не знайдений.";
                return View("NoResults");
            }

            // Step 2: Use the retrieved ClientId to find factories that produce at least one product ordered by the client
            string sqlQuery = @"
        SELECT DISTINCT f.Id, 
                        CAST(f.FactoryName AS NVARCHAR(MAX)) AS FactoryName, 
                        CAST(f.Adress AS NVARCHAR(MAX)) AS Adress, 
                        CAST(f.Maintenance AS NVARCHAR(MAX)) AS Maintenance
        FROM Factories f
        JOIN FactoryProducts fp ON f.Id = fp.FactoryId
        WHERE fp.ProductId IN (
            SELECT rp.ProductId
            FROM ReqProducts rp
            JOIN Orders o ON rp.OrderId = o.Id
            WHERE o.ClientId = @ClientId
        )";

            var factories = _context.Factories
                .FromSqlRaw(sqlQuery, new SqlParameter("@ClientId", clientId))
                .ToList();

            if (!factories.Any())
            {
                ViewBag.Message = "Немає фабрик, що виробляють хоча б один товар, замовлений клієнтом " + clientXName + ".";
                return View("FindFactoriesProducingClientProducts", new List<Factory>());
            }

            return View("FindFactoriesProducingClientProducts", factories);
        }





    }
}
