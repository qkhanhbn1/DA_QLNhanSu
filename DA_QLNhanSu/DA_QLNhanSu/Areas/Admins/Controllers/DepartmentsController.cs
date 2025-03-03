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
    public class DepartmentsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public DepartmentsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Departments
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.Departments 
                                 
                .OrderBy(c => c.Idd);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name)).OrderBy(c => c.Idd);
            }

            var departments = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(departments);
        }

        // GET: Admins/Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Idd == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Admins/Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idd,Name,Content")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Admins/Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Admins/Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idd,Name,Content")] Department department)
        {
            if (id != department.Idd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Idd))
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
            return View(department);
        }

        // GET: Admins/Departments/Delete/5
        

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Idd == id);
        }
        public IActionResult Employees(int id)
        {
            
            var employees = _context.Employees
            .Include(e => e.IdpNavigation) // Load thông tin chức vụ
            .Where(e => e.Idd == id)
            .ToList();
            var department = _context.Departments.FirstOrDefault(d => d.Idd == id);

            if (department == null)
            {
                return NotFound();
            }

            ViewBag.DepartmentName = department.Name;
            return View("Employees", employees); // Đảm bảo có "Employees"
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.Departments.Remove(department);
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
