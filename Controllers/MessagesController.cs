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
using Microsoft.AspNetCore.Authorization;

namespace EmpowerU.Controllers
{
    [Authorize(Roles="Consumer,Business")]
    public class MessagesController : Controller
    {
        private readonly EmpowerUContext _context;

        public MessagesController(EmpowerUContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var empowerUContext = _context.Messages.Include(m => m.Receiver).Include(m => m.Sender);
            return View(await empowerUContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["ReceiverID"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["SenderID"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageID,SenderID,ReceiverID,MessageContent,IsRead,Timestamp")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiverID"] = new SelectList(_context.Users, "Id", "Name", message.ReceiverID);
            ViewData["SenderID"] = new SelectList(_context.Users, "Id", "Name", message.SenderID);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ReceiverID"] = new SelectList(_context.Users, "Id", "Name", message.ReceiverID);
            ViewData["SenderID"] = new SelectList(_context.Users, "Id", "Name", message.SenderID);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageID,SenderID,ReceiverID,MessageContent,IsRead,Timestamp")] Message message)
        {
            if (id != message.MessageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageID))
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
            ViewData["ReceiverID"] = new SelectList(_context.Users, "Id", "Name", message.ReceiverID);
            ViewData["SenderID"] = new SelectList(_context.Users, "Id", "Name", message.SenderID);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageID == id);
        }


        //Luyanda

        [HttpPost]
        public IActionResult SendMessage(string messageContent, string receiverId)
        {

            if (string.IsNullOrWhiteSpace(receiverId))
            {
                return BadRequest("Receiver ID is null or empty.");
            }

            // Attempt to parse the receiver ID
            if (!int.TryParse(receiverId, out int parsedReceiverId)) // Rename variable here
            {
                return BadRequest($"Invalid receiver ID: {receiverId}"); // This will give you the actual string that caused the issue
            }


            // Validate the input
            if (string.IsNullOrWhiteSpace(messageContent) || string.IsNullOrWhiteSpace(receiverId))
            {
                return BadRequest("Message content or receiver ID cannot be empty.");
            }

            // Convert receiverId from string to int
            if (!int.TryParse(receiverId, out int receiverIdInt))
            {
                return BadRequest("Invalid receiver ID.");
            }

            // Assuming SenderID is obtained from User claims
            int senderIdInt;
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out senderIdInt))
            {
                return BadRequest("Invalid sender ID.");
            }

            // Create the message object
            var message = new Message
            {
                MessageContent = messageContent,
                ReceiverID = receiverIdInt, // This should now be an int
                SenderID = senderIdInt, // Ensure this is also an int
                Timestamp = DateTime.Now,
                IsRead = false // Mark as unread by default
            };

            // Save the message to the database
            _context.Messages.Add(message);
            _context.SaveChanges();

            // Redirect to the messages index or wherever you need
            return RedirectToAction("Index");

          
        }


    }
}
