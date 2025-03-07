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
    public class SalaryCalculationsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public SalaryCalculationsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/SalaryCalculations
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.SalaryCalculations.Include(s => s.IdEmployeeallowanceNavigation).Include(s => s.IdSalaryadvanceNavigation).Include(s => s.IdSalaryhistoryNavigation).Include(s => s.IdTimesheetNavigation).Include(s => s.IdeNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/SalaryCalculations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryCalculation = await _context.SalaryCalculations
                .Include(s => s.IdEmployeeallowanceNavigation)
                .Include(s => s.IdSalaryadvanceNavigation)
                .Include(s => s.IdSalaryhistoryNavigation)
                .Include(s => s.IdTimesheetNavigation)
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Ids == id);
            if (salaryCalculation == null)
            {
                return NotFound();
            }

            return View(salaryCalculation);
        }

        // GET: Admins/SalaryCalculations/Create
        public IActionResult Create()
        {
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Id");
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Idsa");
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Id");
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Id");
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide");
            return View();
        }

        // POST: Admins/SalaryCalculations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ids,Ide,IdSalaryhistory,IdEmployeeallowance,IdTimesheet,IdSalaryadvance,Month,Year,TotalSalary,Date")] SalaryCalculation salaryCalculation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryCalculation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Id", salaryCalculation.IdEmployeeallowance);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Idsa", salaryCalculation.IdSalaryadvance);
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Id", salaryCalculation.IdSalaryhistory);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Id", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryCalculation.Ide);
            return View(salaryCalculation);
        }

        // GET: Admins/SalaryCalculations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryCalculation = await _context.SalaryCalculations.FindAsync(id);
            if (salaryCalculation == null)
            {
                return NotFound();
            }
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Id", salaryCalculation.IdEmployeeallowance);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Idsa", salaryCalculation.IdSalaryadvance);
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Id", salaryCalculation.IdSalaryhistory);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Id", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryCalculation.Ide);
            return View(salaryCalculation);
        }

        // POST: Admins/SalaryCalculations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ids,Ide,IdSalaryhistory,IdEmployeeallowance,IdTimesheet,IdSalaryadvance,Month,Year,TotalSalary,Date")] SalaryCalculation salaryCalculation)
        {
            if (id != salaryCalculation.Ids)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryCalculation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryCalculationExists(salaryCalculation.Ids))
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
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Id", salaryCalculation.IdEmployeeallowance);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Idsa", salaryCalculation.IdSalaryadvance);
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Id", salaryCalculation.IdSalaryhistory);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Id", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryCalculation.Ide);
            return View(salaryCalculation);
        }

        // GET: Admins/SalaryCalculations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryCalculation = await _context.SalaryCalculations
                .Include(s => s.IdEmployeeallowanceNavigation)
                .Include(s => s.IdSalaryadvanceNavigation)
                .Include(s => s.IdSalaryhistoryNavigation)
                .Include(s => s.IdTimesheetNavigation)
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Ids == id);
            if (salaryCalculation == null)
            {
                return NotFound();
            }

            return View(salaryCalculation);
        }

        // POST: Admins/SalaryCalculations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaryCalculation = await _context.SalaryCalculations.FindAsync(id);
            if (salaryCalculation != null)
            {
                _context.SalaryCalculations.Remove(salaryCalculation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryCalculationExists(int id)
        {
            return _context.SalaryCalculations.Any(e => e.Ids == id);
        }
    }
}
