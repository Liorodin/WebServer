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
    public class ContactsController : Controller
    {
        private readonly WebServerContext _context;

        public ContactsController(WebServerContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            return _context.Contacts != null ?
                        View(await _context.Contacts.Include(x => x.Users).ToListAsync()) :
                        Problem("Entity set 'WebServerContext.Contacts'  is null.");
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(string Id)
        {
            if (Id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contacts = await _context.Contacts.Include(x => x.Users)
                .FirstOrDefaultAsync(m => m.Username == Id);
            if (contacts == null)
            {
                return NotFound();
            }

            return View(contacts);
        }

        // GET: Contacts/Create
        public async Task<IActionResult> Create()
        {
            var users = await _context.User.ToListAsync();
            ViewBag.Users = new SelectList(users, "Username", "Nickname");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Username, string Users)
        {
            if (Username == Users)
            {
                return RedirectToAction("Create");
            }
            User? thisUSer = await _context.User.FindAsync(Username);
            if (thisUSer == null) return RedirectToAction("Create");

            User? newContact = await _context.User.FindAsync(Users);
            if (newContact == null) return RedirectToAction("Create");

            Contacts? myContacts = await _context.Contacts.Include(x => x.Users)
                    .FirstOrDefaultAsync(m => m.Username == Username);

            if (myContacts == null) return RedirectToAction("Create");

            if (!(myContacts.Users.Contains(newContact)))
            {
                Contacts? thierContacts = await _context.Contacts.Include(x => x.Users)
                    .FirstOrDefaultAsync(m => m.Username == Users);
                if (thierContacts == null) return RedirectToAction("Create");
                myContacts.Users.Add(newContact);
                thierContacts.Users.Add(thisUSer);

                MessageList messageList = new MessageList();
                messageList.Users = new List<User> { thisUSer, newContact };
                messageList.Messages = new List<Message>();
                _context.MessageList.Add(messageList);


                thisUSer.Conversations = new List<MessageList> { messageList };
                newContact.Conversations = new List<MessageList> { messageList };

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contacts = await _context.Contacts.FindAsync(Id);
            var users = await _context.User.ToListAsync();
            ViewBag.Users = new SelectList(users, nameof(Models.User.Username), nameof(Models.User.Nickname));
            if (contacts == null)
            {
                return NotFound();
            }
            return View(contacts);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, [Bind("Username")] Contacts contacts, string Users)
        {
            if (Id != contacts.Username)
            {
                return View(contacts);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    User user = await _context.User.FindAsync(Users);
                    contacts = await _context.Contacts.Include(x => x.Users).FirstOrDefaultAsync(m => m.Username == Id);
                    contacts.Users.Add(user);
                    _context.Update(contacts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactsExists(contacts.Username))
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
            return View(contacts);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contacts = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Username == Id);
            if (contacts == null)
            {
                return NotFound();
            }

            return View(contacts);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'WebServerContext.Contacts'  is null.");
            }
            var contacts = await _context.Contacts.FindAsync(Id);
            if (contacts != null)
            {
                _context.Contacts.Remove(contacts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactsExists(string id)
        {
            return (_context.Contacts?.Any(e => e.Username == id)).GetValueOrDefault();
        }
    }
}
