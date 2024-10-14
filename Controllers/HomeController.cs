using EmpowerU.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmpowerU.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SecuritySettngs()
        {
            return View();
        }

        // GET: Home/Login
        // GET: Render the Login view
        public IActionResult Login()
        {
            return View();
        }

        // POST: Handle the login form submission
        [HttpPost]
        [ValidateAntiForgeryToken] // Helps protect against CSRF attacks
        public async Task<IActionResult> Login([FromForm] Login loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel); // Return the view with validation errors
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user != null)
            {
                // Now attempt to sign in using the UserName and Password
                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Optionally update LastLogin here or perform any post-login actions
                    return RedirectToAction("Index", "Home"); // Redirect to the home page or desired page after successful login
                }
            }

            // Add an error to the model state for invalid login attempts
            ModelState.AddModelError(string.Empty, "Invalid login attempt."); // Add an error to the model state
            ViewData["LoginError"] = "Invalid login attempt."; // Set the error message to be displayed in the view
            return View(loginModel); // Return the view with the model containing the error
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
