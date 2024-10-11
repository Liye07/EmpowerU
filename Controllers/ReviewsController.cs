using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpowerU.Models;
using EmpowerU.Models.Data;

namespace EmpowerU.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly EmpowerUContext _context;

        public ReviewsController(EmpowerUContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var empowerUContext = _context.Reviews.Include(r => r.Business).Include(r => r.Consumer);
            return View(await empowerUContext.ToListAsync());
        }

        // GET: Reviews/ViewReview/5
        public ActionResult ViewReview()
        {
            var reviews = new List<Review>
        {
            new Review { Title = "Great product!", Comment = "Really enjoyed using this.", ReviewerName = "John Doe", Rating = 5, Date = DateTime.Now },
            new Review { Title = "Could be better", Comment = "There were some issues with this.", ReviewerName = "Jane Smith", Rating = 3, Date = DateTime.Now }
        };

            return View(reviews); 
        }

        // GET: Reviews/CreateReview
        public IActionResult CreateReview()
        {
            return View();
        }

        // POST: Reviews/CreateReview
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more ViewReview, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview( Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewReview");
            }
            
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email", review.BusinessID);
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email", review.ConsumerID);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewID,BusinessID,ConsumerID,Rating,Comment,Date")] Review review)
        {
            if (id != review.ReviewID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewID))
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
            ViewData["BusinessID"] = new SelectList(_context.Businesses, "UserID", "Email", review.BusinessID);
            ViewData["ConsumerID"] = new SelectList(_context.Consumers, "UserID", "Email", review.ConsumerID);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Business)
                .Include(r => r.Consumer)
                .FirstOrDefaultAsync(m => m.ReviewID == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewID == id);
        }
    }
}
