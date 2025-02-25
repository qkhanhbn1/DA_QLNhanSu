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
    public class OnLeavesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public OnLeavesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/OnLeaves
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.OnLeaves
                .Include(e => e.IdeNavigation)  // Include Department
                 // Include Qualification
                .OrderBy(c => c.Id);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var onLeave = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(onLeave);
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
                .ThenInclude(e => e.IddNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdqNavigation)
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/OnLeaves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,RequestDate,ReleaseDate,ExpirationDate,Content,Status")] OnLeave onLeave)
        {
            if (ModelState.IsValid)
            {
                _context.Add(onLeave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", onLeave.Ide);
            return View(onLeave);
        }

        // GET: Admins/OnLeaves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var onLeave = await _context.OnLeaves
                .Include(l => l.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation) 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (onLeave == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", onLeave.Ide);
            return View(onLeave);
        }

        // POST: Admins/OnLeaves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,RequestDate,ReleaseDate,ExpirationDate,Content,Status")] OnLeave onLeave)
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", onLeave.Ide);
            return View(onLeave);
        }

      
        private bool OnLeaveExists(int id)
        {
            return _context.OnLeaves.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var onleave = await _context.OnLeaves.FindAsync(id);
            if (onleave == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.OnLeaves.Remove(onleave);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công!" }); // Trả về JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            var onleave = _context.OnLeaves.Find(id);
            if (onleave == null)
            {
                return NotFound();
            }

            onleave.Status = !(onleave.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(onleave.Status);
        }
    }
}
