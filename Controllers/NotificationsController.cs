using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmpowerU.Models;
using EmpowerU.Models.Data;
using System.Security.Claims;

namespace EmpowerU.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly EmpowerUContext _context;

        public NotificationsController(EmpowerUContext context)
        {
            _context = context;
        }

        public IActionResult GetProfilePicture(int userId)
        {
            var user = _context.Users.Find(userId); // Replace with your actual user retrieval logic
            if (user == null || user.ProfilePicture == null)
            {
                return NotFound();
            }

            // Convert byte array to base64 string
            string base64Image = Convert.ToBase64String(user.ProfilePicture);
            string imageDataURL = $"data:image/jpeg;base64,{base64Image}"; // Assuming the image is a JPEG

            return Json(new { imageData = imageDataURL });
        }


        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the notification using the provided ID
            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.NotificationID == id);

            if (notification == null)
            {
                return NotFound();
            }

            // Fetch notifications based on whether the user is a consumer or a business
            var notifications = await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserID == notification.UserID)
                .ToListAsync();

            return View(notifications);
        }


        public void AddNotification(int _userID, string _notificationContent)
        {
            var notification = new Notification
            {
                UserID = _userID,
                NotificationContent = _notificationContent,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public IActionResult GetNotifications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var notifications = _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .OrderByDescending(n => n.Timestamp)
                .ToList();

            return Json(new { notifications });
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", notification.UserID);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificationID,UserID,NotificationContent,IsRead,Timestamp")] Notification notification)
        {
            if (id != notification.NotificationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.NotificationID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", notification.UserID);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.NotificationID == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.NotificationID == id);
        }



    }
}
