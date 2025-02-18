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
    public class CareerDevelopmentsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public CareerDevelopmentsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/CareerDevelopments
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.CareerDevelopments.Include(c => c.IdeNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/CareerDevelopments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerDevelopment = await _context.CareerDevelopments
                .Include(c => c.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (careerDevelopment == null)
            {
                return NotFound();
            }

            return View(careerDevelopment);
        }

        // GET: Admins/CareerDevelopments/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide");
            return View();
        }

        // POST: Admins/CareerDevelopments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,Type,Fromrole,Torole,Status,Date")] CareerDevelopment careerDevelopment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(careerDevelopment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", careerDevelopment.Ide);
            return View(careerDevelopment);
        }

        // GET: Admins/CareerDevelopments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerDevelopment = await _context.CareerDevelopments.FindAsync(id);
            if (careerDevelopment == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", careerDevelopment.Ide);
            return View(careerDevelopment);
        }

        // POST: Admins/CareerDevelopments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,Type,Fromrole,Torole,Status,Date")] CareerDevelopment careerDevelopment)
        {
            if (id != careerDevelopment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(careerDevelopment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CareerDevelopmentExists(careerDevelopment.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", careerDevelopment.Ide);
            return View(careerDevelopment);
        }

        // GET: Admins/CareerDevelopments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerDevelopment = await _context.CareerDevelopments
                .Include(c => c.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (careerDevelopment == null)
            {
                return NotFound();
            }

            return View(careerDevelopment);
        }

        // POST: Admins/CareerDevelopments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var careerDevelopment = await _context.CareerDevelopments.FindAsync(id);
            if (careerDevelopment != null)
            {
                _context.CareerDevelopments.Remove(careerDevelopment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CareerDevelopmentExists(int id)
        {
            return _context.CareerDevelopments.Any(e => e.Id == id);
        }
    }
}
