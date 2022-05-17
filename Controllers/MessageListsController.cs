using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class MessageListsController : Controller
    {
        private readonly WebServerContext _context;

        public MessageListsController(WebServerContext context)
        {
            _context = context;
        }

        // GET: MessageLists
        public async Task<IActionResult> Index()
        {
            return _context.MessageList != null ?
                              View(await _context.MessageList.Include(x => x.Users).Include(x => x.Messages).ToListAsync()) :
                          Problem("Entity set 'WebServerContext.MessageList'  is null.");
        }


        // GET: MessageLists/Create
        public async Task<IActionResult> CreateAsync()
        {
            var users = await _context.User.ToListAsync();
            ViewBag.Users = new SelectList(users, "Username", "Nickname");
            return View();
        }

        // POST: MessageLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] Users ,string Messages)
        {
            return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                var userContacts = await _context.Contacts.Include(x => x.Users)
                .FirstOrDefaultAsync(m => m.Username == Users[0]);
                if (userContacts == null)
                    return RedirectToAction("Create");

                User? fromUser = await _context.User.FindAsync(Users[0]);
                if (fromUser == null)
                    return RedirectToAction("Create");

                User? toUser = await _context.User.FindAsync(Users[1]);
                if (toUser == null)
                    return RedirectToAction("Create");

                if (userContacts.Users.Contains(toUser))
                {
                    //var allMessages = await _context.Message.ToListAsync();
                    Message message = new Message();
                    message.Content = Messages;
                //    message.From = fromUser;
                    message.Time = DateTime.Now;
                }
                //_context.Add(messageList);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Create");
        }

        // GET: MessageLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MessageList == null)
            {
                return NotFound();
            }

            var messageList = await _context.MessageList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (messageList == null)
            {
                return NotFound();
            }

            return View(messageList);
        }

        // POST: MessageLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MessageList == null)
            {
                return Problem("Entity set 'WebServerContext.MessageList'  is null.");
            }
            var messageList = await _context.MessageList.FindAsync(id);
            if (messageList != null)
            {
                _context.MessageList.Remove(messageList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageListExists(int id)
        {
            return (_context.MessageList?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
