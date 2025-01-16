using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_QLNhanSu.Models;
using X.PagedList;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class ContractsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public ContractsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Contracts
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 8; // Số bản ghi trên mỗi trang

            var query = _context.Contracts
                  // Include Department
                .Include(e => e.IdeNavigation)          // Include Position
                .Include(e => e.IdpNavigation) // Include Qualification
                .OrderBy(c => c.Ide);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Ide);
            }

            var contracts = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(contracts);
        }

        // GET: Admins/Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.IdeNavigation)
                .Include(c => c.IdpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Admins/Contracts/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name");
            return View();
        }

        // POST: Admins/Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,SigningDate,ReleaseDate,ExpirationDate,Content,ContractDuration,Idp,DailyWage")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", contract.Ide);
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", contract.Idp);
            return View(contract);
        }

        // GET: Admins/Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", contract.Ide);
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", contract.Idp);
            return View(contract);
        }

        // POST: Admins/Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,SigningDate,ReleaseDate,ExpirationDate,Content,ContractDuration,Idp,DailyWage")] Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", contract.Ide);
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", contract.Idp);
            return View(contract);
        }

        // GET: Admins/Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.IdeNavigation)
                .Include(c => c.IdpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Admins/Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
