using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpowerU.Models;
using EmpowerU.Models.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;

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
/*            Console.WriteLine($"Incoming Data: BizID={review.BusinessID}, ConsumerID={review.ConsumerID}, Title={review.Title}, Rating={review.Rating}");*/

            ModelState.Remove(nameof(review.Business));
            ModelState.Remove(nameof(review.Consumer));

            if (ModelState.IsValid)
            {
                // Parameterized query to prevent SQL injection
                var insertQuery = @"INSERT INTO Review (BusinessID, ConsumerID, Title, Rating, Comment, Date) VALUES (@BusinessID, @ConsumerID, @Title, @Rating, @Comment, @Date)";

                // Execute the raw SQL query with parameters
                await _context.Database.ExecuteSqlRawAsync(insertQuery,
                    new SqlParameter("@BusinessID", review.BusinessID),
                    new SqlParameter("@ConsumerID", review.ConsumerID),
                    new SqlParameter("@Title", review.Title),
                    new SqlParameter("@Rating", review.Rating),
                    new SqlParameter("@Comment", review.Comment ?? string.Empty), // Avoid null values for Comment
                    new SqlParameter("@Date", DateTime.UtcNow));


                // Calculate the new average rating for the business
                var averageRating = await _context.Reviews
                                                  .Where(r => r.BusinessID == review.BusinessID)
                                                  .AverageAsync(r => r.Rating);

                // Update the business's rating
                var business = await _context.Businesses
                                              .FirstOrDefaultAsync(b => b.Id == review.BusinessID);

                if (business != null)
                {
                    // If there are no reviews, set rating to 0
                    business.Rating = (decimal?)(averageRating > 0 ? averageRating : 0);

                    _context.Update(business);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Details", "Businesses", new { id = review.BusinessID });
            }
            else
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"{modelState.Key}: {error.ErrorMessage}");
                    }
                }

/*                Console.WriteLine($"Invalid State = BizID : {review.BusinessID}, ConsumerID : {review.ConsumerID}");*/

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
