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
    public class LeaveJobsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public LeaveJobsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/LeaveJobs
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.LeaveJobs.Include(l => l.IdpNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/LeaveJobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveJob = await _context.LeaveJobs
                .Include(l => l.IdpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveJob == null)
            {
                return NotFound();
            }

            return View(leaveJob);
        }

        // GET: Admins/LeaveJobs/Create
        public IActionResult Create()
        {
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Idp");
            return View();
        }

        // POST: Admins/LeaveJobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nameemployee,Idp,Image,Type,Status,Date")] LeaveJob leaveJob)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveJob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Idp", leaveJob.Idp);
            return View(leaveJob);
        }

        // GET: Admins/LeaveJobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveJob = await _context.LeaveJobs.FindAsync(id);
            if (leaveJob == null)
            {
                return NotFound();
            }
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Idp", leaveJob.Idp);
            return View(leaveJob);
        }

        // POST: Admins/LeaveJobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nameemployee,Idp,Image,Type,Status,Date")] LeaveJob leaveJob)
        {
            if (id != leaveJob.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveJob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveJobExists(leaveJob.Id))
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
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Idp", leaveJob.Idp);
            return View(leaveJob);
        }

        // GET: Admins/LeaveJobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveJob = await _context.LeaveJobs
                .Include(l => l.IdpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveJob == null)
            {
                return NotFound();
            }

            return View(leaveJob);
        }

        // POST: Admins/LeaveJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveJob = await _context.LeaveJobs.FindAsync(id);
            if (leaveJob != null)
            {
                _context.LeaveJobs.Remove(leaveJob);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveJobExists(int id)
        {
            return _context.LeaveJobs.Any(e => e.Id == id);
        }
    }
}
