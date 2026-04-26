using Equipment_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Equipment_Rental.Data;

namespace Equipment_Rental.Controllers
{
    [Authorize(Roles = "Admin")] // Само админи имат достъп!
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Списък с потребители
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserListViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(model);
        }

        // Създаване (GET)
        public IActionResult Create()
        {
            // Взимаме всички роли, за да ги покажем в падащо меню
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        // Създаване (POST)
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    
                    await _userManager.AddToRoleAsync(user, "User");

                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Взимаме текущите роли на потребителя
            var userRoles = await _userManager.GetRolesAsync(user);
            // Взимаме всички роли в системата за падащото меню
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRoles.FirstOrDefault(), // Взимаме първата роля (обикновено имат по една)
                Roles = allRoles
            };

            return View(model);
        }

        // Редактиране (POST)
        [HttpPost]
        // Редактиране (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // 1. Обновяваме основните данни
            user.UserName = model.UserName;
            user.Email = model.Email;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors) ModelState.AddModelError("", error.Description);
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors) ModelState.AddModelError("", error.Description);
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        // Изтриване (POST)
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Намираме всички заявки на този потребител
                var userRequests = _context.RentalRequests.Where(r => r.UserId == id).ToList();

                // Изтриваме ги от базата
                if (userRequests.Any())
                {
                    _context.RentalRequests.RemoveRange(userRequests);
                    _context.SaveChanges();
                }

                // Безопасно да изтрием потребител
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}