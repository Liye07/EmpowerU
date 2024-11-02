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
        public ActionResult CreatePayment()
        {
            // Use Stripe sandbox API key for testing
            StripeConfiguration.ApiKey = "sk_test_51QFgQzGLtm1zZAcU1QCGWO7Xr6R8yMt4x6yD0UXR0nyBwPl99qgx4u2zpirOCOhtN7LyfyANT2Oa7mOOKBvuE6Kp00pRGluzCQ";

            var options = new PaymentIntentCreateOptions
            {
                Amount = 5000, // Test amount (5000 = $50.00 in cents)
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = service.Create(options);

            // Pass the client secret to the view
            ViewBag.ClientSecret = paymentIntent.ClientSecret;

            return View();


        }

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
                    PaymentIntentId = paymentData.PaymentIntentId
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            return BadRequest(new { success = false, message = "Invalid payment data" });
        }
    



// GET: Payments
public async Task<IActionResult> Index()
        {
            var empowerUContext = _context.Payments.Include(p => p.Business).Include(p => p.Consumer);
            return View(await empowerUContext.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Business)
                .Include(p => p.Consumer)
                .FirstOrDefaultAsync(m => m.PaymentID == id);
            if (payment == null)
            {
                return NotFound();
            }

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email", payment.BusinessID);
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email", payment.ConsumerID);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentID,ConsumerID,BusinessID,Amount,PaymentDate,PaymentStatus")] PaymentEmpowerU payment)
        {
            if (id != payment.PaymentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentID))
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
            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email", payment.BusinessID);
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email", payment.ConsumerID);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Business)
                .Include(p => p.Consumer)
                .FirstOrDefaultAsync(m => m.PaymentID == id);
            if (payment == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentID == id);
        }
    }
}
