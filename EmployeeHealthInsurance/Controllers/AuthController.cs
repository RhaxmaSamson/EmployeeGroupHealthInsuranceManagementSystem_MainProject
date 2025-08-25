using EmployeeHealthInsurance.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class AuthController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    // IMPORTANT: Reuse the Index view instead of a missing Login.cshtml
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string role)
    {
        ViewBag.Role = role;
        return View("Index", new LoginDto());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string role, LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Role = role;
            return View("Index", model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            ViewBag.Role = role;
            return View("Index", model);
        }

        // Enforce role gate for HR/Admin login tile
        if (!string.IsNullOrWhiteSpace(role) && role.ToLower().StartsWith("hr"))
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var isHr = await _userManager.IsInRoleAsync(user, "HRManager");
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isHr && !isAdmin)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "You are not authorized for HR/Admin access.");
                ViewBag.Role = role;
                return View("Index", model);
            }
        }

        // *** ADDED: Set the success message in TempData before redirecting ***
        TempData["SuccessMessage"] = "Welcome! Login Successful! 🥳";

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Auth");
    }
}