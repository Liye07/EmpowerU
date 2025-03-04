﻿using System;
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
using Stripe;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace EmpowerU.Controllers
{
    public class BusinessesController : BaseController
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

        [Authorize(Roles = "Business")]
        public IActionResult BusinessDashboard(int? id)
        {

            // Check if the id parameter is provided
            if (id == null)
            {
                return NotFound("Business ID is required.");
            }

            // Fetch the business details using the provided id
            var business = _context.Businesses
                                   .FirstOrDefault(b => b.Id == id);

            // Check if the business exists
            if (business == null)
            {
                return NotFound("Business not found.");
            }

            // Get current month name
            string currentMonth = DateTime.UtcNow.ToString("MMMM");

            // Fetch total number of distinct customers for this business
            int totalCustomers = _context.Appointments
                                          .Where(a => a.BusinessID == business.Id)
                                          .Select(a => a.ConsumerID)
                                          .Distinct()
                                          .Count();

            // Fetch total income for this business
            decimal totalIncome = _context.Payments
                                          .Where(p => p.BusinessID == business.Id)
                                          .Sum(p => (decimal?)p.Amount) ?? 0;

            // Fetch total number of appointments booked for this business
            int totalAppointments = _context.Appointments
                                            .Where(a => a.BusinessID == business.Id)
                                            .Count();

            // Fetch today's appointments for the business
            var today = DateTime.UtcNow;
            var todayAppointments = _context.Appointments
                                             .Where(a => a.BusinessID == business.Id && a.DateTime.Date == today)
                                             .Select(a => new
                                             {
                                                 Name = a.Consumer.Name,
                                                 DateTime = a.DateTime
                                             })
                                             .ToList<object>(); // Cast to List<object> instead of List<dynamic>

            // Calculate monthly income
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Query payments for the month
            decimal monthlyIncome = _context.Payments
                .Where(p => p.BusinessID == business.Id &&
                            p.PaymentDate >= startOfMonth &&
                            p.PaymentDate <= endOfMonth)
                .Sum(p => (decimal?)p.Amount) ?? 0;


            var notifications = _context.Notifications
                                        .Where(n => n.UserID == business.Id && !n.IsRead)
                                        .ToList();

            var unreadMessages = _context.Messages
                             .Where(m => m.ReceiverID == business.Id && !m.IsRead)
                             .ToList();

            // Pass the data to the View using ViewBag
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.TodayAppointments = todayAppointments; // Add today's appointments to ViewBag
            ViewBag.MonthlyIncome = monthlyIncome; // Add monthly income to ViewBag
            ViewBag.CurrentMonth = currentMonth;
            ViewBag.Notifications = notifications;
            ViewBag.UnreadMessages = unreadMessages;

            return View(business);  // Pass the business model to the view
        }




        // GET: Businesses/Details/5
        [AllowAnonymous]
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
        [Authorize(Roles = "Business")]
        public IActionResult CreateProfile()
        {
            return View();
        }

        // POST: BusinessProfile/CreateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
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
                        var newService = new Models.Service
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

        public IActionResult GetAvailableTimeSlots(int serviceID, DateTime bookingDate)
        {
            // Convert bookingDate to UTC
            bookingDate = bookingDate.ToUniversalTime();

            // Define working hours and slot duration
            TimeSpan startTime = TimeSpan.FromHours(7);  // 9:00 AM UTC+2 is 7:00 AM UTC
            TimeSpan endTime = TimeSpan.FromHours(15);  // 5:00 PM UTC+2 is 3:00 PM UTC
            TimeSpan slotDuration = TimeSpan.FromHours(1); // 1-hour slots

            // Get existing appointments for the selected date and service
            var bookedSlots = _context.Appointments
                .Where(a => a.ServiceID == serviceID && a.DateTime.Date == bookingDate.Date)
                .Select(a => a.DateTime.ToUniversalTime().TimeOfDay)
                .ToList();

            // Generate all possible time slots within the working hours
            var availableSlots = new List<TimeSpan>();
            for (var time = startTime; time < endTime; time += slotDuration)
            {
                // Only add the slot if it’s not already booked
                if (!bookedSlots.Contains(time))
                {
                    availableSlots.Add(time);
                }
            }

            // Return available time slots
            return Json(new { availableSlots = availableSlots.Select(s => s.ToString(@"hh\:mm")) });
        }



        [HttpPost]
        public IActionResult BookService(int serviceID, DateTime bookingDate, int businessID, string BookingTime)
        {
            if (ModelState.IsValid)
            {

                DateTime fullBookingDateTime = DateTime.Parse(BookingTime);
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var consumer = _context.Consumers.Find(userId);

                if (consumer == null)
                {
                    ModelState.AddModelError("", "Consumer not found.");
                    return View("Details", new { id = businessID }); 
                }

                var service = _context.Services.Find(serviceID);
                if (service == null)
                {
                    ModelState.AddModelError("", "Service not found.");
                    return View("Details", new { id = businessID });
                }

                var business = _context.Businesses.Find(businessID);
                if (business == null)
                {
                    ModelState.AddModelError("", "Business not found.");
                    return View("Details", new { id = businessID });
                }

                TimeSpan startBusinessHours = new TimeSpan(9, 0, 0); 
                TimeSpan endBusinessHours = new TimeSpan(17, 0, 0); 

                if (fullBookingDateTime.TimeOfDay < startBusinessHours || fullBookingDateTime.TimeOfDay > endBusinessHours)
                {
                    ModelState.AddModelError("", "Booking time must be between 9:00 AM and 5:00 PM.");
                    return View("Details", new { id = businessID }); 
                }

                var insertQuery = @"
    INSERT INTO ""Appointment"" (""BusinessID"", ""ConsumerID"", ""ServiceID"", ""DateTime"", ""Status"", ""Confirmation"")
    VALUES (@BusinessID, @ConsumerID, @ServiceID, @DateTime, @Status, @Confirmation)";

                var parameters = new[]
                {
    new NpgsqlParameter("@BusinessID", businessID),
    new NpgsqlParameter("@ConsumerID", consumer.Id),
    new NpgsqlParameter("@ServiceID", serviceID),
    new NpgsqlParameter("@DateTime", fullBookingDateTime),
    new NpgsqlParameter("@Status", "Scheduled"),
    new NpgsqlParameter("@Confirmation", true)
};

                _context.Database.ExecuteSqlRaw(insertQuery, parameters);


                var notificationService = new NotificationsController(_context);
                notificationService.AddNotification(consumer.Id, $"Your appointment for {service.ServiceName} in {business.Name} has been confirmed for {fullBookingDateTime:dd MMM yyyy} at {fullBookingDateTime:hh:mm tt}.");
                notificationService.AddNotification(businessID, $"{consumer.Name} {consumer.Surname} has booked an appointment for {service.ServiceName} on the {fullBookingDateTime:dd MMM yyyy} at {fullBookingDateTime:hh:mm tt}.");
                // Store the confirmation message in TempData
                TempData["ConfirmationMessage"] = $"Booking confirmed! \nService: {service.ServiceName}, \nDate: {fullBookingDateTime:dd MMM yyyy}, \nTime: {fullBookingDateTime:hh:mm tt}";

                return RedirectToAction("Details", new { id = businessID });
            }

            return View("Details", new { id = businessID }); 
        }



        // GET: Businesses/EditBusinessProfile/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> EditBusinessProfile(int? id)
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

            // If there is no associated LocationService, create an empty one
            if (business.LocationService == null)
            {
                business.LocationService = new LocationService();
            }

            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address", business.LocationID);
            return View(business);
        }

        // POST: Businesses/EditBusinessProfile/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> EditBusinessProfile(int id, [Bind("Description,LocationID,Rating,Id,Name,Email,PhoneNumber,Password,Role,LastLogin,BusinessCategory,LocationService")] Business business)
        {
            if (id != business.Id)
            {
                return NotFound();
            }

            // Remove password from model state validation for updates
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    // Load the existing business from the database to ensure we have the latest values
                    var existingBusiness = await _context.Businesses
                        .Include(b => b.LocationService)
                        .FirstOrDefaultAsync(b => b.Id == id);

                    if (existingBusiness == null)
                    {
                        return NotFound();
                    }

                    // Update properties from the model
                    existingBusiness.Description = business.Description;
                    existingBusiness.Rating = business.Rating;
                    existingBusiness.Name = business.Name;
                    existingBusiness.Email = business.Email;
                    existingBusiness.PhoneNumber = business.PhoneNumber;
                    existingBusiness.BusinessCategory = business.BusinessCategory;

                    // Update LocationService if it exists, otherwise create a new one
                    if (business.LocationService != null)
                    {
                        if (existingBusiness.LocationService != null)
                        {
                            // Update existing LocationService
                            existingBusiness.LocationService.Address = business.LocationService.Address;
                            existingBusiness.LocationService.Suburb = business.LocationService.Suburb;
                            existingBusiness.LocationService.City = business.LocationService.City;
                            existingBusiness.LocationService.PostalCode = business.LocationService.PostalCode;
                            existingBusiness.LocationService.Country = business.LocationService.Country;

                            // Ensure the entity is marked as modified
                            _context.Entry(existingBusiness.LocationService).State = EntityState.Modified;
                        }
                        else
                        {
                            // Create a new LocationService and assign the new ID to business.LocationID
                            var newLocationService = new LocationService
                            {
                                Address = business.LocationService.Address,
                                Suburb = business.LocationService.Suburb,
                                City = business.LocationService.City,
                                PostalCode = business.LocationService.PostalCode,
                                Country = business.LocationService.Country
                            };
                            _context.LocationServices.Add(newLocationService);
                            await _context.SaveChangesAsync();

                            // Link new LocationService ID to the business
                            existingBusiness.LocationID = newLocationService.LocationID;
                        }
                    }
                    else
                    {
                        // If LocationService is null, ensure LocationID is null as well
                        existingBusiness.LocationID = null;
                    }

                    // Save changes to the business
                    _context.Update(existingBusiness);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(BusinessDashboard), new { id = business.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessExists(business.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }

            // If we get here, something failed; redisplay the form.
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
        [Authorize(Roles = "Business")]
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

            var appointments = await _context.Appointments
                .Where(a => a.BusinessID == business.Id)
                .Include(a => a.Business)
                .Include(a => a.Consumer) // Ensure Consumer data is loaded
                .Include(a => a.Service)
                .ToListAsync();

            var currentDate = DateTime.UtcNow;

            ViewBag.PastAppointments = appointments
               .Where(a => a.DateTime < currentDate &&
                           (a.Status == "Completed" || a.Status == "Cancelled")) // Check both date and status
               .OrderByDescending(a => a.DateTime)
               .ToList();

            ViewBag.UpcomingAppointments = appointments
                .Where(a => a.DateTime >= currentDate || a.Status == "Pending")
                .OrderBy(a => a.DateTime)
                .ToList();

            ViewBag.Business = business; // Pass consumer details

            ViewBag.BusinessNames = appointments.Select(a => a.Business?.Description).Distinct().ToList();

            var firstAppointment = appointments.FirstOrDefault();
            return View(firstAppointment); // Pass the first appointment or adjust as needed
        }


        public class UpdateAppointmentStatusDto
        {
            public string Status { get; set; }
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

        [HttpPost]
        [Route("appointments/b_reschedule/{appointmentId}")]
        public JsonResult B_Reschedule(int appointmentId, [FromBody] Appointment request)
        {
            try
            {
                var appointment = _context.Appointments
                    .Include(a => a.Consumer)
                    .Include(a => a.Business)
                    .Include(a => a.Service) 
                    .FirstOrDefault(a => a.AppointmentID == appointmentId);

                if (appointment == null)
                    return Json(new { success = false, message = "Appointment not found." });

                if (appointment.Status != "Scheduled" && appointment.Status != "Pending")
                    return Json(new { success = false, message = "Only scheduled or pending appointments can be rescheduled." });

                if (request.DateTime < DateTime.UtcNow)
                    return Json(new { success = false, message = "The new appointment time must be in the future." });

                appointment.DateTime = request.DateTime;
                _context.SaveChanges();

                var consumer = appointment.Consumer;
                var business = appointment.Business;
                var service = appointment.Service;

                var notificationService = new NotificationsController(_context);

                notificationService.AddNotification(
                    appointment.Consumer.Id,
                    $"Your appointment for {appointment.Service.ServiceName} has been rescheduled to {appointment.DateTime:dd MMM yyyy} at {appointment.DateTime:hh:mm tt} by {business.Name}."
                );

                notificationService.AddNotification(
                    appointment.Business.Id,
                    $"{appointment.Consumer.Name} {appointment.Consumer.Surname} has rescheduled their appointment for {appointment.Service.ServiceName} to {appointment.DateTime:dd MMM yyyy} at {appointment.DateTime:hh:mm tt}."
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                return Json(new { success = false, message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost]
        [Route("appointments/b_cancel/{appointmentId}")]
        public IActionResult B_Cancel(int appointmentId)
        {
            var appointment = _context.Appointments
                .Include(a => a.Consumer)
                .Include(a => a.Business)
                .Include(a => a.Service) 
                .FirstOrDefault(a => a.AppointmentID == appointmentId);

            if (appointment == null)
                return Json(new { success = false, message = "Appointment not found." });

            appointment.Status = "Cancelled";
            _context.SaveChanges();

            var consumer = appointment.Consumer;
            var business = appointment.Business;
            var service = appointment.Service;

            if (consumer == null || business == null || service == null)
                return Json(new { success = false, message = "Missing required appointment details." });

            var notificationService = new NotificationsController(_context);

            notificationService.AddNotification(
                consumer.Id,
                $"Your appointment for {service.ServiceName} scheduled on {appointment.DateTime:dd MMM yyyy} at {appointment.DateTime:hh:mm tt} has been cancelled by {business.Name}."
            );

            notificationService.AddNotification(
                business.Id,
                $"Your appointment for {service.ServiceName} with {consumer.Name} {consumer.Surname} scheduled on {appointment.DateTime:dd MMM yyyy} at {appointment.DateTime:hh:mm tt} has been cancelled successfully."
            );

            return Json(new { success = true });
        }


        [HttpGet]
        public IActionResult LoadMoreReviews(int count, int businessId)
        {
            var reviews = _context.Reviews
                .Where(r => r.BusinessID == businessId)
                .OrderByDescending(r => r.Date)  // Sort by date descending
                .Skip(count) 
                .Take(5)
                .Select(r => new
                {
                    r.Title,
                    r.Comment,
                    r.Rating,
                    r.Date
                })
                .ToList();

            // Return the reviews as JSON
            return Json(reviews);
        }



    }
}
