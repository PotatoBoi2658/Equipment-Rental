using Equipment_Rental.Data;
using Equipment_Rental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equipment_Rental.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<IdentityUser> _signInManager;

		public AccountController(SignInManager<IdentityUser> signInManager)
		{
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
