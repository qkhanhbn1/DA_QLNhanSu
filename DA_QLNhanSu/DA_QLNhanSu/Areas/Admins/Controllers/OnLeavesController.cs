using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_QLNhanSu.Models;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class OnLeavesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public OnLeavesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/OnLeaves
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.OnLeaves.Include(o => o.IdeNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/OnLeaves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var onLeave = await _context.OnLeaves
                .Include(o => o.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (onLeave == null)
            {
                return NotFound();
            }

            return View(onLeave);
        }

        // GET: Admins/OnLeaves/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide");
            return View();
        }

        // POST: Admins/OnLeaves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,ReleaseDate,ExpirationDate,Content")] OnLeave onLeave)
        {
            if (ModelState.IsValid)
            {
                _context.Add(onLeave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", onLeave.Ide);
            return View(onLeave);
        }

        // GET: Admins/OnLeaves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var onLeave = await _context.OnLeaves.FindAsync(id);
            if (onLeave == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", onLeave.Ide);
            return View(onLeave);
        }

        // POST: Admins/OnLeaves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,ReleaseDate,ExpirationDate,Content")] OnLeave onLeave)
        {
            if (id != onLeave.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(onLeave);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OnLeaveExists(onLeave.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", onLeave.Ide);
            return View(onLeave);
        }

        // GET: Admins/OnLeaves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var onLeave = await _context.OnLeaves
                .Include(o => o.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (onLeave == null)
            {
                return NotFound();
            }

            return View(onLeave);
        }

        // POST: Admins/OnLeaves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var onLeave = await _context.OnLeaves.FindAsync(id);
            if (onLeave != null)
            {
                _context.OnLeaves.Remove(onLeave);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OnLeaveExists(int id)
        {
            return _context.OnLeaves.Any(e => e.Id == id);
        }
    }
}
