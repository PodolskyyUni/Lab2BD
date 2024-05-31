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
    public class FactoryProductsController : Controller
    {
        private readonly Isttp1FvContext _context;

        public FactoryProductsController(Isttp1FvContext context)
        {
            _context = context;
        }

        // GET: FactoryProducts
        public async Task<IActionResult> Index()
        {
            var isttp1FvContext = _context.FactoryProducts.Include(f => f.Factory).Include(f => f.Product);
            return View(await isttp1FvContext.ToListAsync());
        }

        // GET: FactoryProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factoryProduct = await _context.FactoryProducts
                .Include(f => f.Factory)
                .Include(f => f.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (factoryProduct == null)
            {
                return NotFound();
            }

            return View(factoryProduct);
        }

        // GET: FactoryProducts/Create
        public IActionResult Create()
        {
            ViewData["FactoryId"] = new SelectList(_context.Factories, "Id", "Adress");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription");
            return View();
        }

        // POST: FactoryProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,FactoryId,ProductionCost,AmountProduced")] FactoryProduct factoryProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(factoryProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FactoryId"] = new SelectList(_context.Factories, "Id", "Adress", factoryProduct.FactoryId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription", factoryProduct.ProductId);
            return View(factoryProduct);
        }

        // GET: FactoryProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factoryProduct = await _context.FactoryProducts.FindAsync(id);
            if (factoryProduct == null)
            {
                return NotFound();
            }
            ViewData["FactoryId"] = new SelectList(_context.Factories, "Id", "Adress", factoryProduct.FactoryId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription", factoryProduct.ProductId);
            return View(factoryProduct);
        }

        // POST: FactoryProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,FactoryId,ProductionCost,AmountProduced")] FactoryProduct factoryProduct)
        {
            if (id != factoryProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factoryProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FactoryProductExists(factoryProduct.Id))
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
            ViewData["FactoryId"] = new SelectList(_context.Factories, "Id", "Adress", factoryProduct.FactoryId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "ProductDescription", factoryProduct.ProductId);
            return View(factoryProduct);
        }

        // GET: FactoryProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factoryProduct = await _context.FactoryProducts
                .Include(f => f.Factory)
                .Include(f => f.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (factoryProduct == null)
            {
                return NotFound();
            }

            return View(factoryProduct);
        }

        // POST: FactoryProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factoryProduct = await _context.FactoryProducts.FindAsync(id);
            if (factoryProduct != null)
            {
                _context.FactoryProducts.Remove(factoryProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FactoryProductExists(int id)
        {
            return _context.FactoryProducts.Any(e => e.Id == id);
        }
    }
}
