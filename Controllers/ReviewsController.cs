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
    public class ReviewsController : BaseController
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

        // GET: Reviews/Delete/5
        public async Task<IActionResult> ViewReview()
        {
            var reviews = await _context.Reviews.ToListAsync();
            return View(reviews); // Ensure view expects @model IEnumerable<Review>
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(Review review)
        {
            if (ModelState.IsValid)
            {
                review.Date = DateTime.Now; // Set the current date for the review

                // Add review to the database context
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Redirect to a page, e.g., the business or consumer's reviews page
                return RedirectToAction("ViewReview", new { id = review.BusinessID });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    // Log error message
                    Console.WriteLine(error);
                }

            }

            // If the model is not valid, return the view to show validation errors
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
