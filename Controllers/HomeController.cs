using EmpowerU.Models;
using EmpowerU.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EmpowerU.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmpowerUContext _context; // Add your DbContext here

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, EmpowerUContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // Initialize DbContext
        }


  

        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Home"; // Set active page for Home

            // Fetch featured businesses
            var featuredBusinesses = await _context.Businesses
                .Where(b => b.Rating > 4) // Adjust this condition as needed
                .OrderByDescending(b => b.Rating)
                .Take(3) // Limit to top 3 businesses, change as needed
                .ToListAsync();

            return View(featuredBusinesses); // Pass the list to the view

        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("Index"); // Return to Index if search term is empty
            }

            var results = await _context.Businesses
                .Where(b => b.Name.Contains(searchTerm) || b.Description.Contains(searchTerm))
                .ToListAsync();

            return PartialView("_SearchResults", results); // Return a partial view with the results
        }

        public IActionResult SecuritySettings()
        {
            return View();
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
