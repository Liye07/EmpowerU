using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpowerU.Models;
using EmpowerU.Models.Data;
using Stripe;

namespace EmpowerU.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly EmpowerUContext _context;

        public PaymentsController(EmpowerUContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var payments = _context.Payments.Include(p => p.Business).Include(p => p.Consumer);
            return View(await payments.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Business)
                .Include(p => p.Consumer)
                .FirstOrDefaultAsync(m => m.PaymentID == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email");
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email");
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentID,ConsumerID,BusinessID,Amount,PaymentDate,PaymentStatus")] PaymentEmpowerU payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email", payment.BusinessID);
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email", payment.ConsumerID);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();

            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email", payment.BusinessID);
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email", payment.ConsumerID);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentID,ConsumerID,BusinessID,Amount,PaymentDate,PaymentStatus")] PaymentEmpowerU payment)
        {
            if (id != payment.PaymentID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentID)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email", payment.BusinessID);
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email", payment.ConsumerID);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.Business)
                .Include(p => p.Consumer)
                .FirstOrDefaultAsync(m => m.PaymentID == id);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Create Payment (Stripe)
        // GET: Create Payment (Stripe)
        public ActionResult CreatePayment(int appointmentId)
        {
            try
            {
                // Retrieve the appointment and service details
                var appointment = _context.Appointments
                                          .Include(a => a.Service)
                                          .FirstOrDefault(a => a.AppointmentID == appointmentId);

                // Handle case where appointment or service is not found
                if (appointment == null)
                {
                    return NotFound($"The appointment with ID {appointmentId} was not found.");
                }

                if (appointment.Service == null)
                {
                    return NotFound($"The appointment with ID {appointmentId} does not have an associated service.");
                }

                // Retrieve the price from the service
                decimal servicePrice = appointment.Service.Price;

                // Set your Stripe API key
                StripeConfiguration.ApiKey = "sk_test_51QFgQzGLtm1zZAcU1QCGWO7Xr6R8yMt4x6yD0UXR0nyBwPl99qgx4u2zpirOCOhtN7LyfyANT2Oa7mOOKBvuE6Kp00pRGluzCQ";

                // Create a PaymentIntent with the service price and currency
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(servicePrice * 100), // Convert price to cents for Stripe
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "AppointmentId", appointmentId.ToString() },
                        { "ServiceId", appointment.Service.ServiceID.ToString() }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = service.Create(options);

                // Pass the client secret to the view to confirm the payment client-side
                ViewBag.ClientSecret = paymentIntent.ClientSecret;

                // Initialize a PaymentEmpowerU model to pass to the view
                var paymentModel = new PaymentEmpowerU
                {
                    ConsumerID = appointment.ConsumerID,
                    BusinessID = appointment.BusinessID,
                    Amount = servicePrice,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Pending",
                    PaymentIntentId = paymentIntent.Id,
                    Currency = "USD",
                    AppointmentID = appointment.AppointmentID // Add AppointmentID here
                };

                // Return the view with the payment model
                return View(paymentModel);
            }
            catch (StripeException stripeEx)
            {
                // Log the error
                return BadRequest($"Stripe error: {stripeEx.Message}");
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        // POST: Process Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentEmpowerU paymentData)
        {
            if (ModelState.IsValid)
            {
                var payment = new PaymentEmpowerU
                {
                    ConsumerID = paymentData.ConsumerID,
                    BusinessID = paymentData.BusinessID,
                    Amount = paymentData.Amount,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = paymentData.PaymentStatus,
                    PaymentIntentId = paymentData.PaymentIntentId,
                    AppointmentID = paymentData.AppointmentID // Include AppointmentID
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            return BadRequest(new { success = false, message = "Invalid payment data" });
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentID == id);
        }

        // POST: Confirm Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm([FromBody] PaymentEmpowerU payment)
        {
            if (payment == null || payment.Amount <= 0 || string.IsNullOrEmpty(payment.PaymentIntentId))
            {
                return BadRequest("Invalid payment details.");
            }

            try
            {
                // Save payment details to the database
                payment.PaymentDate = DateTime.UtcNow;
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, "Error saving payment: " + ex.Message);
            }
        }

        // Payment Success View
        public IActionResult Success()
        {
            return View(); // Return a view that shows the payment success message
        }



    }

}
