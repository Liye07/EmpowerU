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
using System.Security.Claims;

namespace EmpowerU.Controllers
{
    public class BusinessesController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // Change here if User is int
        private readonly EmpowerUContext _context;

        public BusinessesController(EmpowerUContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Businesses
        public async Task<IActionResult> Index()
        {
            var businesses = await _context.Businesses
                    .Include(b => b.LocationService)
                    .ToListAsync();

            return View(businesses);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Businesses/Details/5
        public IActionResult Details(int id)
        {
            // Fetch the business along with its services and location service
            var business = _context.Businesses
                .Include(b => b.Reviews) // Include reviews if necessary
                .Include(b => b.Services) // Include services
                .Include(b => b.LocationService) // Include location service
                .FirstOrDefault(b => b.Id == id); // Assuming 'Id' is the primary key

            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // GET: BusinessProfile/CreateProfile
        public IActionResult CreateProfile()
        {
            return View();
        }

        // POST: BusinessProfile/CreateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(Business model)
        {
            if (ModelState.IsValid)
            {
                // Save business description and other business details
                var newBusiness = new Business
                {
                    Description = model.Description,
                    LocationID = model.LocationID,
                    Rating = model.Rating,
                    BusinessCategory = model.BusinessCategory,
                    // Other fields can be added as needed
                };

                // Add business to the database
                _context.Businesses.Add(newBusiness);
                await _context.SaveChangesAsync();

                // Save services associated with the business
                if (model.Services != null && model.Services.Count > 0)
                {
                    foreach (var service in model.Services)
                    {
                        var newService = new Service
                        {
                            BusinessID = newBusiness.Id, // Foreign Key
                            ServiceName = service.ServiceName,
                            ServiceDescription = service.ServiceDescription,
                            Price = service.Price
                        };
                        _context.Services.Add(newService);
                    }

                    await _context.SaveChangesAsync(); // Save all services
                }

                // Redirect to a different page (e.g., profile details or confirmation)
                return RedirectToAction("ProfileDetails", new { id = newBusiness.Id });
            }

            // If the model state is invalid, return the form with validation messages
            return View(model);
        }

        [HttpPost]
        public IActionResult BookService(int serviceID, DateTime bookingDate, int businessID)
        {
            if (ModelState.IsValid)
            {
                // Get the currently logged-in user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from claims
                var consumer = _context.Consumers.Find(userId); // Assuming you have a Consumers DbSet

                if (consumer == null)
                {
                    return Json(new { success = false, message = "Consumer not found." });
                }

                // Fetch the service name based on the serviceID
                var service = _context.Services.Find(serviceID);
                if (service == null)
                {
                    return Json(new { success = false, message = "Service not found." });
                }

                var appointment = new Appointment
                {
                    BusinessID = businessID,
                    ConsumerID = consumer.Id, // Use the Consumer's ID
                    ServiceType = service.ServiceName, // Assign the service name
                    DateTime = bookingDate,
                    Status = "Confirmed",
                    Confirmation = true
                };

                // Save the appointment to the database
                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                // Return a success response
                return Json(new { success = true, message = "Booking confirmed!", appointment });
            }

            // If the model state is not valid, return an error response
            return Json(new { success = false, message = "Failed to book appointment. Please try again." });
        }




        // GET: Businesses/EditBusinessProfile/5
        public async Task<IActionResult> EditBusinessProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }
            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address", business.LocationID);
            return View(business);
        }

        // POST: Businesses/EditBusinessProfile/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBusinessProfile(int id, [Bind("Description,LocationID,Rating,Id,Name,Email,PhoneNo,Password,Role,LastLogin")] Business business)
        {
            if (id != business.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(business);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessExists(business.Id))
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
            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address", business.LocationID);
            return View(business);
        }

        // GET: Businesses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Businesses
                .Include(b => b.LocationService)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // POST: Businesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business != null)
            {
                _context.Businesses.Remove(business);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessExists(int id)
        {
            return _context.Businesses.Any(e => e.Id == id);
        }

        // GET: Businesses/ManageAppointments/5
        public async Task<IActionResult> ManageAppointments(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Businesses
                .Include(b => b.LocationService)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (business == null)
            {
                return NotFound();
            }

            // Retrieve all appointments for the business
            var appointments = await _context.Appointments
                .Where(a => a.BusinessID == id)
                .Include(a => a.Consumer)  // Eager load the Consumer
                .ToListAsync();

            // Pass appointments and business to the view
            ViewBag.Business = business;
            ViewBag.Appointments = appointments;

            return View();
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDto statusDto)
        {
            if (statusDto == null || string.IsNullOrWhiteSpace(statusDto.Status))
            {
                return BadRequest("Invalid data.");
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = statusDto.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class UpdateAppointmentStatusDto
        {
            public string Status { get; set; }
        }


    }
}
