using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpowerU.Models.Data;
using EmpowerU.Models;
using Microsoft.AspNetCore.Identity;

namespace EmpowerU.Controllers
{
    public class ConsumersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // Change here if User is int
        private readonly EmpowerUContext _context;
        private readonly ILogger<ConsumersController> _logger;

        public ConsumersController(EmpowerUContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger<ConsumersController> logger) // Change here if User is int
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Consumers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Consumers.ToListAsync());
        }

        // GET: Consumers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumer = await _context.Consumers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consumer == null)
            {
                return NotFound();
            }

            return View(consumer);
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
                var user = new Consumer
                {
                    Email = consumer.Email,
                    Name = consumer.Name,
                    Surname = consumer.Surname,
                    PhoneNumber = consumer.PhoneNumber,
                    PreferredCategories = consumer.PreferredCategories,
                    UserName = consumer.UserName, // Add this line to set the username
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
                    return RedirectToAction("Login", "Home");
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


        // GET: Consumers/EditConsumerProfile/5
        public async Task<IActionResult> EditConsumerProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumer = await _context.Consumers.FindAsync(id);
            if (consumer == null)
            {
                return NotFound();
            }
            return View(consumer);
        }

        // POST: Consumers/EditConsumerProfile/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConsumerProfile(int id, [Bind("PreferredCategories,Id,Name,Surname,Email,PhoneNo,Password,Role,LastLogin")] Consumer consumer)
        {
            if (id != consumer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumerExists(consumer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(consumer);
        }

        // GET: Consumers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumer = await _context.Consumers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consumer == null)
            {
                return NotFound();
            }

            return View(consumer);
        }

        // POST: Consumers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumer = await _context.Consumers.FindAsync(id);
            if (consumer != null)
            {
                _context.Consumers.Remove(consumer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumerExists(int id)
        {
            return _context.Consumers.Any(e => e.Id == id);
        }

        public IActionResult Search()
        {
            return View();  // This will render the Search view.
        }

        [Route("Consumers/AppointmentDetails/{id?}")]
        public async Task<IActionResult> AppointmentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the consumer by ID
            var consumer = await _context.Consumers.FirstOrDefaultAsync(c => c.Id == id);

            if (consumer == null)
            {
                return NotFound();
            }

            // Get the consumer's appointments directly from the Appointments table
            var appointments = await _context.Appointments
                .Where(a => a.ConsumerID == consumer.Id)
                .ToListAsync();

            // Get the current date to filter appointments
            var currentDate = DateTime.Now;

            // Filter past and upcoming appointments
            ViewBag.PastAppointments = appointments
                .Where(a => a.DateTime < currentDate)
                .OrderByDescending(a => a.DateTime)
                .ToList();

            ViewBag.UpcomingAppointments = appointments
                .Where(a => a.DateTime >= currentDate)
                .OrderBy(a => a.DateTime)
                .ToList();

            ViewBag.Consumer = consumer; // Pass consumer details

            return View(consumer); // Make sure to pass the consumer object
        }
    }
}
