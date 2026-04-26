using Equipment_Rental.Data;
using Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Equipment_Rental.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var items = _context.EquipmentItems.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                items = items.Where(x => x.Name.Contains(search));

            return View(items.ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(EquipmentItem item)
        {
            _context.EquipmentItems.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(EquipmentItem item)
        {
            var existing = _context.EquipmentItems.Find(item.Id);

            if (existing == null)
                return NotFound();

            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.QuantityAvailable = item.QuantityAvailable;
            existing.ImageUrl = item.ImageUrl;
            existing.Condition = item.Condition;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var item = _context.EquipmentItems.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var item = _context.EquipmentItems.Find(id);

            if (item == null)
                return NotFound();

            _context.EquipmentItems.Remove(item);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
