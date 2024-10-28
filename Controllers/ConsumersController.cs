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


        public IActionResult Dashboard(int? id)
        {
            // Check if the id parameter is provided
            if (id == null)
            {
                return NotFound("Consumer ID is required.");
            }

            // Fetch the business details using the provided id
            var consumer = _context.Consumers
                                   .FirstOrDefault(b => b.Id == id);

            // Check if the Consumer exists
            if (consumer == null)
            {
                return NotFound("Consumer not found.");
            }

            // Get current month name
            string currentMonth = DateTime.Now.ToString("MMMM");

            // Fetch today's appointments for the consumer
            var today = DateTime.Today;
            var todayAppointments = _context.Appointments
                                             .Where(a => a.ConsumerID == consumer.Id && a.DateTime.Date == today)
                                             .Select(a => new
                                             {
                                                 Name = a.Consumer.Name,
                                                 DateTime = a.DateTime
                                             })
                                             .ToList<object>(); // Cast to List<object> instead of List<dynamic>

            // **New Notifications code**
            var notifications = _context.Notifications
                                        .Where(n => n.UserID == consumer.Id && !n.IsRead)
                                        .ToList();

            var unreadMessages = _context.Messages
                                   .Where(m => m.ReceiverID == consumer.Id && !m.IsRead)
                                   .ToList();

            ViewBag.TodayAppointments = todayAppointments;
            ViewBag.CurrentMonth = currentMonth;
            ViewBag.Notifications = notifications;
            ViewBag.UnreadMessages = unreadMessages; 

            return View(consumer);  // Pass the consumer model to the view
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
        public async Task<IActionResult> EditConsumerProfile(int id, [Bind("PreferredCategories,Id,Name,Surname,Email,PhoneNumber,Password,Role,LastLogin")] Consumer consumer)
        {
            if (id != consumer.Id)
            {
                return NotFound();
            }

            // Remove password from model state validation for updates
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing consumer record from the database
                    var existingConsumer = await _context.Consumers.FindAsync(id);
                    if (existingConsumer == null)
                    {
                        return NotFound();
                    }

                    // Update the existing consumer's properties
                    existingConsumer.Name = consumer.Name;
                    existingConsumer.Surname = consumer.Surname;
                    existingConsumer.Email = consumer.Email;
                    existingConsumer.PhoneNumber = consumer.PhoneNumber;
                    existingConsumer.PreferredCategories = consumer.PreferredCategories;

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Dashboard", "Consumers", new { id = consumer.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumerExists(consumer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user. Please try again.");
                    }
                }
            }

            // If we got this far, something failed; redisplay form with current data
            var existingConsumerData = await _context.Consumers.FindAsync(id);
            return View(existingConsumerData);
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

        public IActionResult AppointmentDetails()
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

            // Get the consumer's appointments directly from the Appointments table, including Business details
            var appointments = await _context.Appointments
                .Where(a => a.ConsumerID == consumer.Id)
                .Include(a => a.Business) // Include the Business information from appointments
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

            // If you want to show business names, you could do something like this:
            ViewBag.BusinessNames = appointments.Select(a => a.Business?.Description).Distinct().ToList();

            // Assuming you want to return the first appointment or handle differently based on your requirements
            var firstAppointment = appointments.FirstOrDefault();
            return View(firstAppointment); // Pass the first appointment or adjust as needed
        }




    }
}
