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
    public class EmployeesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public EmployeesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Employees
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 5; // Số bản ghi trên mỗi trang

            var query = _context.Employees
                .Include(e => e.IddNavigation)  // Include Department
                .Include(e => e.IdpNavigation)          // Include Position
                .Include(e => e.IdqNavigation) // Include Qualification
                .OrderBy(c => c.Ide);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name)).OrderBy(c => c.Ide);
            }

            var employee = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(employee);
        }

        // GET: Admins/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.IddNavigation)
                .Include(e => e.IdpNavigation)
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
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name");
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name");
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name");
            return View();
        }

        // POST: Admins/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ide,Name,Gender,Birthday,Email,Phone,Cccd,Address,Image,Idd,Idp,Idq")] Employee employee)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0)
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        employee.Image = "/Images/" + FileName;
                    }
                }
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name", employee.Idd);
                ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", employee.Idp);
                ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name", employee.Idq);
                ViewBag.error = ex.Message;
                return View(employee);
            }
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
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name", employee.Idd);
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", employee.Idp);
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name", employee.Idq);
            return View(employee);
        }

        // POST: Admins/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ide,Name,Gender,Birthday,Email,Phone,Cccd,Address,Image,Idd,Idp,Idq")] Employee employee)
        {
            if (id != employee.Ide)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var file = files[0];
                        var FileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            employee.Image = "/Images/" + FileName;
                        }
                    }
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Ide))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return View(employee);
                    }
                }
                
            }
            // Đưa ra lỗi của modelState để bạn có thể debug
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
            // Xem danh sách lỗi trong debug console hoặc log
            foreach (var errorMessage in errorMessages)
            {
                Console.WriteLine(errorMessage);
            }
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name", employee.Idd);
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", employee.Idp);
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name", employee.Idq);
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
                .Include(e => e.IddNavigation)
                .Include(e => e.IdpNavigation)
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
