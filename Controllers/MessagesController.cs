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
    public class MessagesController : BaseController
    {
        private readonly EmpowerUContext _context;

        public MessagesController(EmpowerUContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int businessID)
        {
            int currentUserId;
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out currentUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            // Find or create a conversation between the current user and the business
            var conversation = await _context.Conversations
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c => (c.User1ID == currentUserId && c.User2ID == businessID) ||
                                          (c.User1ID == businessID && c.User2ID == currentUserId));

            if (conversation == null)
            {
                // Create a new conversation if none exists
                conversation = new Conversation
                {
                    User1ID = currentUserId,
                    User2ID = businessID
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }

            return View(conversation.Messages);
        }


        [HttpPost]
        public IActionResult SendMessage(string messageContent, int receiverId)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                return BadRequest("Message content cannot be empty.");
            }

            int senderId;
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out senderId))
            {
                return BadRequest("Invalid sender ID.");
            }

            var conversation = _context.Conversations
               .FirstOrDefault(c => (c.User1ID == senderId && c.User2ID == receiverId) ||
                        (c.User1ID == receiverId && c.User2ID == senderId));

            // If no conversation exists, create a new one
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1ID = senderId,
                    User2ID = receiverId,
                    CreatedDate = DateTime.Now
                };

                _context.Conversations.Add(conversation);
                _context.SaveChanges(); // Save to get the conversation ID
            }

            var receiver = _context.Users.Find(receiverId);
            if (receiver == null)
            {
                // Handle the error (e.g., show an error message or redirect)
                return NotFound("The specified receiver does not exist.");
            }

            // Proceed with message creation
            var message = new Message
            {
                MessageContent = messageContent,
                SenderID = senderId, // Assume you have the sender ID here
                ReceiverID = receiverId,
                Timestamp = DateTime.Now,
                IsRead = false,
                ConversationID = conversation.ConversationID
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Index", new { businessID = receiverId });
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
   




    }
}
