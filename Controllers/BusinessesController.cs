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
    public class BusinessesController : Controller
    {
        private readonly EmpowerUContext _context;

        public BusinessesController(EmpowerUContext context)
        {
            _context = context;
        }

        // GET: Businesses
        public async Task<IActionResult> Index()
        {
            var empowerUContext = _context.Businesses.Include(b => b.LocationService);
            return View(await empowerUContext.ToListAsync());
        }

        // GET: Businesses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Businesses
                .Include(b => b.LocationService)
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // GET: Businesses/RegisterBusiness
        public IActionResult RegisterBusiness()
        {
            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address");
            return View();
        }

        // POST: Businesses/RegisterBusiness
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterBusiness([Bind("Rating,Name,Email,PhoneNo,Password,Role,LastLogin")] Business business)
        {
            if (ModelState.IsValid)
            {
                _context.Add(business);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address", business.LocationID);
            return View(business);
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
        public async Task<IActionResult> EditBusinessProfile(int id, [Bind("Description,LocationID,Rating,UserID,Name,Email,PhoneNo,Password,Role,LastLogin")] Business business)
        {
            if (id != business.UserID)
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
                    if (!BusinessExists(business.UserID))
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
                .FirstOrDefaultAsync(m => m.UserID == id);
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
            return _context.Businesses.Any(e => e.UserID == id);
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
                .FirstOrDefaultAsync(m => m.UserID == id);

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
