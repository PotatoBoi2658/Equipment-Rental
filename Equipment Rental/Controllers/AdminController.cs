using Equipment_Rental.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equipment_Rental.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            var model = new
            {
                Users = _context.Users.Count(), // ако ползваш Identity → ще го заменим
                Equipment = _context.EquipmentItems.Count(),
                Requests = _context.RentalRequests.Count(),
                Pending = _context.RentalRequests.Count(x => x.Status == "Pending")
            };

            return View(model);
        }
        public IActionResult Requests()
        {
            var requests = _context.RentalRequests
                .Include(r => r.User)
                .Include(r => r.Items)
                .ThenInclude(i => i.EquipmentItem)
                .ToList();

            return View(requests);
        }
        public IActionResult ChangeStatus(int id, string status)
        {
            var req = _context.RentalRequests.Find(id);

            if (req == null)
                return NotFound();

            req.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Requests");
        }
    }
}
