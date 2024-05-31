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
    public class ClientsController : Controller
    {
        private readonly Isttp1FvContext _context;

        public ClientsController(Isttp1FvContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientName,Contacts,Password")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientName,Contacts,Password")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        public IActionResult FindClientsOrderingSameProducts(string clientXName)
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

            // Step 2: Find other clients who ordered the same products
            string sqlQuery = @"
        SELECT c2.Id, 
               CAST(c2.ClientName AS NVARCHAR(MAX)) AS ClientName, 
               CAST(c2.Contacts AS NVARCHAR(MAX)) AS Contacts, 
               CAST(c2.Password AS NVARCHAR(MAX)) AS Password
        FROM Clients c2
        WHERE c2.Id != @ClientId
        AND NOT EXISTS (
            SELECT p.Id
            FROM Products p
            JOIN ReqProducts rp1 ON p.Id = rp1.ProductId
            JOIN Orders o1 ON rp1.OrderId = o1.Id
            WHERE o1.ClientId = @ClientId
            AND NOT EXISTS (
                SELECT 1
                FROM ReqProducts rp2
                JOIN Orders o2 ON rp2.OrderId = o2.Id
                WHERE rp2.ProductId = p.Id
                AND o2.ClientId = c2.Id
            )
        )
        AND NOT EXISTS (
            SELECT p.Id
            FROM Products p
            JOIN ReqProducts rp2 ON p.Id = rp2.ProductId
            JOIN Orders o2 ON rp2.OrderId = o2.Id
            WHERE o2.ClientId = c2.Id
            AND NOT EXISTS (
                SELECT 1
                FROM ReqProducts rp1
                JOIN Orders o1 ON rp1.OrderId = o1.Id
                WHERE rp1.ProductId = p.Id
                AND o1.ClientId = @ClientId
            )
        )";



            var clients = _context.Clients
                .FromSqlRaw(sqlQuery, new SqlParameter("@ClientId", clientId))
                .ToList();

            if (!clients.Any())
            {
                ViewBag.Message = "Немає клієнтів, які замовляли ті самі товари, що і клієнт " + clientXName + ".";
                return View("FindClientsOrderingSameProducts", new List<Client>());
            }

            return View("FindClientsOrderingSameProducts", clients);
        }


        public IActionResult FindClientsNotOrderingSameProducts(string clientXName)
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

            // Step 2: Find clients who did not order any products ordered by the client
            string sqlQuery = @"
         SELECT c2.Id, 
               CAST(c2.ClientName AS NVARCHAR(MAX)) AS ClientName, 
               CAST(c2.Contacts AS NVARCHAR(MAX)) AS Contacts, 
               CAST(c2.Password AS NVARCHAR(MAX)) AS Password
        FROM Clients c2
        WHERE c2.Id != @ClientId
        AND NOT EXISTS (
            SELECT 1
            FROM Orders o1
            JOIN ReqProducts rp1 ON o1.Id = rp1.OrderId
            JOIN Products p ON rp1.ProductId = p.Id
            JOIN ReqProducts rp2 ON p.Id = rp2.ProductId
            JOIN Orders o2 ON rp2.OrderId = o2.Id
            WHERE o1.ClientId = @ClientId
            AND o2.ClientId = c2.Id
        )";

            var clients = _context.Clients
                .FromSqlRaw(sqlQuery, new SqlParameter("@ClientId", clientId))
                .ToList();

            if (!clients.Any())
            {
                ViewBag.Message = "Немає клієнтів, які не замовляли жодного товару, замовленого клієнтом " + clientXName + ".";
                return View("FindClientsNotOrderingSameProducts", new List<Client>());
            }

            return View("FindClientsNotOrderingSameProducts", clients);
        }


    }
}

