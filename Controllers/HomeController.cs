using EmpowerU.Models;
using EmpowerU.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EmpowerU.Controllers
{
    public class HomeController : BaseController
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
                //.Where(b => b.Rating > 1) // Adjust this condition as needed
                .OrderByDescending(b => b.Rating)
                .Take(3) // Limit to top 3 businesses, change as needed
                .ToListAsync();



            return View(featuredBusinesses); // Pass the list to the view


        }

        private const double EarthRadius = 6371; // Radius of the Earth in kilometers

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            lat1 = DegreesToRadians(lat1);
            lat2 = DegreesToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadius * c; // Distance in kilometers
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }


        public IActionResult RedirectToSearch(double lat, double lon)
        {
            // Pass latitude and longitude as route parameters to Consumers/Search
            return RedirectToAction("Search", "Consumers", new { lat = lat, lon = lon });
        }

        public IActionResult Search(double lat, double lon)
        {
            double radius = 10; // Define the search radius in kilometers
            var businesses = _context.LocationServices
                .Where(b => CalculateDistance(lat, lon, b.Latitude, b.Longitude) <= radius)
                .ToList();

            return View(businesses);
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
            return View();
        }


    }
}
