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
    public class EmployeeAllowancesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public EmployeeAllowancesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/EmployeeAllowances
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.EmployeeAllowances
                .Include(e => e.IdeNavigation)  // Include Department
                .Include(e => e.IdAllowancesNavigation)          // Include Position
                 // Include Qualification
                .OrderBy(c => c.Id);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var employeeAllowance = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAllowance = await _context.EmployeeAllowances
                .Include(e => e.IdAllowancesNavigation)
                .Include(e => e.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAllowance == null)
            {
                return NotFound();
            }

            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Create
        public IActionResult Create()
        {
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name");
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/EmployeeAllowances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,IdAllowances,Money")] EmployeeAllowance employeeAllowance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeAllowance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAllowance = await _context.EmployeeAllowances
                .Include(e => e.IdAllowancesNavigation)
                .Include(e => e.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAllowance == null)
            {
                return NotFound();
            }
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // POST: Admins/EmployeeAllowances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,IdAllowances,Money")] EmployeeAllowance employeeAllowance)
        {
            if (id != employeeAllowance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeAllowance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeAllowanceExists(employeeAllowance.Id))
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
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Delete/5
        
        private bool EmployeeAllowanceExists(int id)
        {
            return _context.EmployeeAllowances.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var employeeAllowance = await _context.EmployeeAllowances.FindAsync(id);
            if (employeeAllowance == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.EmployeeAllowances.Remove(employeeAllowance);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công!" }); // Trả về JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
