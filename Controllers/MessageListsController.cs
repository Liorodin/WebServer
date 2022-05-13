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
                          View(await _context.MessageList.ToListAsync()) :
                          Problem("Entity set 'WebServerContext.MessageList'  is null.");
        }

        // GET: MessageLists/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: MessageLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MessageLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] MessageList messageList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(messageList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(messageList);
        }

        // GET: MessageLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MessageList == null)
            {
                return NotFound();
            }

            var messageList = await _context.MessageList.FindAsync(id);
            if (messageList == null)
            {
                return NotFound();
            }
            return View(messageList);
        }

        // POST: MessageLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] MessageList messageList)
        {
            if (id != messageList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageListExists(messageList.Id))
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
            return View(messageList);
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
