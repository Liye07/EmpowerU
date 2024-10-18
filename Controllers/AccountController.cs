using EmpowerU.Models;
using EmpowerU.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EmpowerU.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // Change here if User is int
        private readonly EmpowerUContext _context;

        public AccountController(EmpowerUContext context, ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                    await _userManager.FindByNameAsync(user.UserName);
                    user.LastLogin = DateTime.UtcNow; // Set the last login time
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home"); // Redirect to the home page or desired page after successful login
                }
            }

            // Add an error to the model state for invalid login attempts
            ModelState.AddModelError(string.Empty, "Invalid login attempt."); // Add an error to the model state
            ViewData["LoginError"] = "Invalid login attempt."; // Set the error message to be displayed in the view
            return View(loginModel); // Return the view with the model containing the error
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Log out the user
            await _signInManager.SignOutAsync();

            // Redirect to the home page or login page after logout
            return RedirectToAction("Index", "Login");
        }

        public IActionResult RegisterConsumer()
        {
            return View();  // This will render the RegisterConsumer view.
        }

        // POST: Consumers/RegisterConsumer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterConsumer([Bind("PreferredCategories,UserName,Name,Surname,Email,PhoneNumber,Password")] Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                // Phone Number validation: Ensure correct format
                if (!Regex.IsMatch(consumer.PhoneNumber, @"^\d{10}$")) // Example for a 10-digit phone number
                {
                    ModelState.AddModelError("PhoneNumber", "Please enter a valid 10-digit phone number.");
                    return View(consumer);
                }

                // Enhanced Email validation using a stricter regular expression
                var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(consumer.Email, emailRegex))
                {
                    ModelState.AddModelError("Email", "Please enter a valid email address.");
                    return View(consumer);
                }

                // Check if the email is already in use
                var existingUserWithEmail = await _userManager.FindByEmailAsync(consumer.Email);
                if (existingUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(consumer);
                }

                // Check if the phone number is already in use
                var existingUserWithPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == consumer.PhoneNumber);
                if (existingUserWithPhone != null)
                {
                    ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                    return View(consumer);
                }

                var user = new Consumer
                {
                    Email = consumer.Email,
                    Name = consumer.Name,
                    Surname = consumer.Surname,
                    PhoneNumber = consumer.PhoneNumber,
                    PreferredCategories = consumer.PreferredCategories,
                    UserName = consumer.UserName,
                    Role = "Consumer"
                };

                var result = await _userManager.CreateAsync(user, consumer.Password);
                if (result.Succeeded)
                {
                    // Ensure the role exists before assigning
                    if (!await _roleManager.RoleExistsAsync("Consumer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>("Consumer")); // Use int if User is int
                    }

                    await _userManager.AddToRoleAsync(user, "Consumer");
                    return RedirectToAction("Login", "Account");
                }

                // Capture and log any creation errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    // Log error message
                    Console.WriteLine(error);
                }
            }

            // If model state is invalid, redisplay the form with errors
            return View(consumer);
        }



        // GET: Businesses/RegisterBusiness
        public IActionResult RegisterBusiness()
        {

            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address");
            return View();
        }

        // POST: Businesses/RegisterBusiness
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterBusiness([Bind("Description,Rating,Name,UserName,Email,PhoneNumber,Password,BusinessCategory,LocationService")] Business business)
        {
            if (ModelState.IsValid)
            {
                // Phone Number validation: Ensure correct format
                if (!Regex.IsMatch(business.PhoneNumber, @"^\d{10}$")) // Example for a 10-digit phone number
                {
                    ModelState.AddModelError("PhoneNumber", "Please enter a valid 10-digit phone number.");
                    return View(business);
                }

                // Enhanced Email validation using a stricter regular expression
                var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(business.Email, emailRegex))
                {
                    ModelState.AddModelError("Email", "Please enter a valid email address.");
                    return View(business);
                }

                // Check if the email is already in use
                var existingUserWithEmail = await _userManager.FindByEmailAsync(business.Email);
                if (existingUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(business);
                }

                // Check if the phone number is already in use
                var existingUserWithPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == business.PhoneNumber);
                if (existingUserWithPhone != null)
                {
                    ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                    return View(business);
                }

                // Check if LocationService is provided
                if (business.LocationService != null)
                {
                    // Ensure the LocationService has necessary fields set
                    if (string.IsNullOrWhiteSpace(business.LocationService.Address))
                    {
                        ModelState.AddModelError("LocationService.Address", "Address is required.");
                        return View(business);
                    }

                    // Add LocationService and save changes to get LocationID
                    _context.LocationServices.Add(business.LocationService);
                    await _context.SaveChangesAsync(); // Save to get the LocationID
                }

                var user = new Business
                {
                    Email = business.Email,
                    Name = business.Name,
                    UserName = business.UserName,
                    PhoneNumber = business.PhoneNumber,
                    Role = "Business",
                    LocationID = business.LocationService?.LocationID ?? 0, // Use the newly created LocationID
                    Description = business.Description,
                    Rating = business.Rating,
                    BusinessCategory = business.BusinessCategory
                };

                // Create user in the Identity framework
                var result = await _userManager.CreateAsync(user, business.Password);
                if (result.Succeeded)
                {
                    // Ensure the role exists before assigning
                    if (!await _roleManager.RoleExistsAsync("Business"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>("Business")); // Use int if User is int
                    }

                    await _userManager.AddToRoleAsync(user, "Business");
                    return RedirectToAction("Login", "Account");
                }

                // Capture and log any creation errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    // Log error message
                    Console.WriteLine(error);
                }
            }

            // Ensure that LocationID is populated for dropdown lists in the View
            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address", business.LocationService?.LocationID);

            return View(business);
        }



    }
}
