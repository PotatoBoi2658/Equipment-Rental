using Equipment_Rental.Data;
using Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equipment_Rental.Controllers
{
    [Authorize]
    public class RentalRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RentalRequestsController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult MyRequests()
        {
            var userId = _userManager.GetUserId(User);

            var requests = _context.RentalRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Items)
                .ThenInclude(i => i.EquipmentItem)
                .ToList();

            return View(requests);
        }

        public IActionResult Create()
        {
            ViewBag.Equipment = _context.EquipmentItems.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RentalRequest request, List<RentalRequestItem> items)
        {
            var userId = _userManager.GetUserId(User);

            if (request.StartDate < DateTime.Today ||
                request.EndDate < request.StartDate)
            {
                ModelState.AddModelError("", "Invalid dates.");
                ViewBag.Equipment = _context.EquipmentItems.ToList();
                return View(request);
            }

            request.UserId = userId;
            request.Status = "Pending";

            request.Items = new List<RentalRequestItem>();

            foreach (var item in items)
            {
                var equipment = _context.EquipmentItems.Find(item.EquipmentItemId);

                if (equipment == null)
                    continue;

                if (item.Quantity <= 0)
                {
                    ModelState.AddModelError("", "Quantity must be greater than 0.");
                    ViewBag.Equipment = _context.EquipmentItems.ToList();
                    return View(request);
                }

                if (item.Quantity > equipment.QuantityAvailable)
                {
                    ModelState.AddModelError("", $"{equipment.Name} not enough in stock.");
                    ViewBag.Equipment = _context.EquipmentItems.ToList();
                    return View(request);
                }

                request.Items.Add(new RentalRequestItem
                {
                    EquipmentItemId = item.EquipmentItemId,
                    Quantity = item.Quantity
                });

                equipment.QuantityAvailable -= item.Quantity;
            }

            _context.RentalRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("MyRequests");
        }

        
        // GET: RentalRequests/Edit/5
        public IActionResult Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var request = _context.RentalRequests
                .Include(r => r.Items)
                .ThenInclude(i => i.EquipmentItem)
                .FirstOrDefault(r => r.Id == id && r.UserId == userId);

            if (request == null) return NotFound();
            return View(request);
        }

        // POST: RentalRequests/Edit/5
        [HttpPost]
        public IActionResult Edit(RentalRequest model)
        {
            var request = _context.RentalRequests.Find(model.Id);
            if (request == null || request.Status != "Pending") return BadRequest();

            request.StartDate = model.StartDate;
            request.EndDate = model.EndDate;
            request.Purpose = model.Purpose;

            _context.SaveChanges();
            return RedirectToAction("MyRequests");
        }

        // GET: RentalRequests/Delete/5
        public IActionResult Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var request = _context.RentalRequests
                .FirstOrDefault(r => r.Id == id && r.UserId == userId);

            if (request == null) return NotFound();
            return View(request);
        }
    }
}
