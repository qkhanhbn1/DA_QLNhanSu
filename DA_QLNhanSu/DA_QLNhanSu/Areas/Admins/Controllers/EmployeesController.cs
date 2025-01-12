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
    public class EmployeesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public EmployeesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Employees
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.Employees.Include(e => e.Idd1).Include(e => e.IddNavigation).Include(e => e.IdqNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Idd1)
                .Include(e => e.IddNavigation)
                .Include(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Ide == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Admins/Employees/Create
        public IActionResult Create()
        {
            ViewData["Idd"] = new SelectList(_context.Positions, "Idp", "Idp");
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Idd");
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Idq");
            return View();
        }

        // POST: Admins/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ide,Name,Gender,Birthday,Email,Phone,Cccd,Address,Image,Idd,Idp,IdAccount,Idq")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idd"] = new SelectList(_context.Positions, "Idp", "Idp", employee.Idd);
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Idd", employee.Idd);
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Idq", employee.Idq);
            return View(employee);
        }

        // GET: Admins/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["Idd"] = new SelectList(_context.Positions, "Idp", "Idp", employee.Idd);
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Idd", employee.Idd);
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Idq", employee.Idq);
            return View(employee);
        }

        // POST: Admins/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ide,Name,Gender,Birthday,Email,Phone,Cccd,Address,Image,Idd,Idp,IdAccount,Idq")] Employee employee)
        {
            if (id != employee.Ide)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Ide))
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
            ViewData["Idd"] = new SelectList(_context.Positions, "Idp", "Idp", employee.Idd);
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Idd", employee.Idd);
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Idq", employee.Idq);
            return View(employee);
        }

        // GET: Admins/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Idd1)
                .Include(e => e.IddNavigation)
                .Include(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Ide == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Admins/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Ide == id);
        }
    }
}
