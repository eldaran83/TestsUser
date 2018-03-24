using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestUsers.Data;
using TestUsers.Models.BO;

namespace TestUsers.Controllers
{
    public class AventuresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AventuresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Aventures
        public async Task<IActionResult> Index()
        {
            return View(await _context.Aventures.ToListAsync());
        }

        // GET: Aventures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aventure = await _context.Aventures
                .SingleOrDefaultAsync(m => m.AventureID == id);
            if (aventure == null)
            {
                return NotFound();
            }

            return View(aventure);
        }

        // GET: Aventures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aventures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AventureID,NomAventure,Vote,ImageUrl")] Aventure aventure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aventure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aventure);
        }

        // GET: Aventures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aventure = await _context.Aventures.SingleOrDefaultAsync(m => m.AventureID == id);
            if (aventure == null)
            {
                return NotFound();
            }
            return View(aventure);
        }

        // POST: Aventures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AventureID,NomAventure,Vote,ImageUrl")] Aventure aventure)
        {
            if (id != aventure.AventureID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aventure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AventureExists(aventure.AventureID))
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
            return View(aventure);
        }

        // GET: Aventures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aventure = await _context.Aventures
                .SingleOrDefaultAsync(m => m.AventureID == id);
            if (aventure == null)
            {
                return NotFound();
            }

            return View(aventure);
        }

        // POST: Aventures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aventure = await _context.Aventures.SingleOrDefaultAsync(m => m.AventureID == id);
            _context.Aventures.Remove(aventure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AventureExists(int id)
        {
            return _context.Aventures.Any(e => e.AventureID == id);
        }
    }
}
