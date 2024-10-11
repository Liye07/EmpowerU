using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpowerU.Models.Data;

namespace EmpowerU.Controllers
{
    public class ConsumersController : Controller
    {
        private readonly EmpowerUContext _context;

        public ConsumersController(EmpowerUContext context)
        {
            _context = context;
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
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (consumer == null)
            {
                return NotFound();
            }

            return View(consumer);
        }

        // GET: Consumers/RegisterConsumer
        public IActionResult RegisterConsumer()
        {
            return View();
        }

        // POST: Consumers/RegisterConsumer
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterConsumer([Bind("PreferredCategories,UserID,Name,Email,PhoneNo,Password,Role,LastLogin")] Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConsumerProfile(int id, [Bind("PreferredCategories,UserID,Name,Email,PhoneNo,Password,Role,LastLogin")] Consumer consumer)
        {
            if (id != consumer.UserID)
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
                    if (!ConsumerExists(consumer.UserID))
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
                .FirstOrDefaultAsync(m => m.UserID == id);
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
            return _context.Consumers.Any(e => e.UserID == id);
        }

        public IActionResult Search()
        {
            return View();  // This will render the Search.cshtml view.
        }

        [Route("Consumers/AppointmentDetails/{id?}")]
        public async Task<IActionResult> AppointmentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the consumer by ID
            var consumer = await _context.Consumers.FirstOrDefaultAsync(c => c.UserID == id);

            if (consumer == null)
            {
                return NotFound();
            }

            // Get the consumer's appointments directly from the Appointments table
            var appointments = await _context.Appointments
                .Where(a => a.ConsumerID == consumer.UserID)
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
