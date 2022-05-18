#nullable enable
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
    public class CommentsController : Controller
    {
        private readonly WebServerContext _context;

        public CommentsController(WebServerContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            List<Comment> comments = await _context.Comment.ToListAsync();
            comments.Sort((a, b) => -DateTime.Compare(a.Time,b.Time));
            return View(comments);
        }

        //[HttpPost]

        //public async Task<IActionResult> Search()
        //{
        //    List<Comment> comments = await _context.Comment.ToListAsync();
        //    comments.Sort((a, b) => -DateTime.Compare(a.Time, b.Time));
        //    return View(comments);
        //}

        //public async Task<IActionResult> Search(string query)
        //{
        //    var q = from    comment in _context.Comment
        //            where comment.Name.Contains(query) ||
        //            comment.Feedback.Contains(query)
        //            select comment;
        //    return View(await q.ToListAsync());
        //}

        public async Task<IActionResult> Search(string query)
        {
            var q = from comment in _context.Comment
                    where comment.Name.Contains(query) ||
                    comment.Feedback.Contains(query)
                    select comment;
            return Json(await q.ToListAsync());
        }


        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Feedback,Rating")] Comment comment)
        {
            comment.Time = DateTime.Now;
            if (ModelState.IsValid)
            {

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(Id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, [Bind("Id,Name,Feedback,Rating")] Comment comment)
        {
            if (Id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comment.Time = DateTime.Now;
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.Id == Id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            var comment = await _context.Comment.FindAsync(Id);
            if (comment != null)
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int Id)
        {
            return _context.Comment.Any(e => e.Id == Id);
        }
    }
}
