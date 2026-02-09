using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // =========================
    // LIST USERS
    // =========================
    public IActionResult Index()
    {
        return View(_userManager.Users.ToList());
    }

    // =========================
    // CREATE USER (GET)
    // =========================
    public IActionResult Create()
    {
        ViewBag.Roles = _roleManager.Roles.ToList(); // ✅ REQUIRED
        return View();
    }

    // =========================
    // CREATE USER (POST)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string email, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Email and password are required");
            ViewBag.Roles = _roleManager.Roles.ToList(); // ✅ REQUIRED
            return View();
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            ViewBag.Roles = _roleManager.Roles.ToList(); // ✅ REQUIRED
            return View();
        }

        if (!string.IsNullOrEmpty(role))
            await _userManager.AddToRoleAsync(user, role);

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // ACTIVATE / DEACTIVATE
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        // target user
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        // current logged-in user
        var currentUserId = _userManager.GetUserId(User);

        // 🚫 PREVENT SELF DEACTIVATION
        if (user.Id == currentUserId)
        {
            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "You cannot deactivate your own account.";
            return RedirectToAction(nameof(Index));
        }

        // toggle active / inactive
        user.LockoutEnd =
            user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now
                ? null
                : DateTimeOffset.Now.AddYears(100);

        await _userManager.UpdateAsync(user);

        TempData["AlertType"] = "success";
        TempData["AlertMessage"] = "User status updated successfully.";

        return RedirectToAction(nameof(Index));
    }

}
