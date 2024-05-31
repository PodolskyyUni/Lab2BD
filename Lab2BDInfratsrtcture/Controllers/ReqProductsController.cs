using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2BDDomain.Model;
using Lab2BDInfrastructure;

namespace Lab2BDInfratsrtcture.Controllers
{
    public class ReqProductsController : Controller
    {
        private readonly Isttp1FvContext _context;

        public ReqProductsController(Isttp1FvContext context)
        {
            _context = context;
        }

        // GET: ReqProducts
        public async Task<IActionResult> Index()
        {
            var isttp1FvContext = _context.ReqProducts.Include(r => r.Order).Include(r => r.Product);
            return View(await isttp1FvContext.ToListAsync());
        }

        // GET: ReqProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reqProduct = await _context.ReqProducts
                .Include(r => r.Order)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reqProduct == null)
            {
                return NotFound();
            }

            return View(reqProduct);
        }

        // GET: ReqProducts/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription");
            return View();
        }

        // POST: ReqProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,ReqProductAmount,OrderId")] ReqProduct reqProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reqProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", reqProduct.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription", reqProduct.ProductId);
            return View(reqProduct);
        }

        // GET: ReqProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reqProduct = await _context.ReqProducts.FindAsync(id);
            if (reqProduct == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", reqProduct.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription", reqProduct.ProductId);
            return View(reqProduct);
        }

        // POST: ReqProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ReqProductAmount,OrderId")] ReqProduct reqProduct)
        {
            if (id != reqProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reqProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReqProductExists(reqProduct.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", reqProduct.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription", reqProduct.ProductId);
            return View(reqProduct);
        }

        // GET: ReqProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reqProduct = await _context.ReqProducts
                .Include(r => r.Order)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reqProduct == null)
            {
                return NotFound();
            }

            return View(reqProduct);
        }

        // POST: ReqProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reqProduct = await _context.ReqProducts.FindAsync(id);
            if (reqProduct != null)
            {
                _context.ReqProducts.Remove(reqProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReqProductExists(int id)
        {
            return _context.ReqProducts.Any(e => e.Id == id);
        }
    }
}
