using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServer.Data;
using WebServer.Models;
using WebServer.Services;
using WebServer.Services.Comments;

namespace WebServer.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _service;

        public CommentsController(WebServerContext context)
        {
            _service = new CommentService(context);
        }

        public double Average()
        {
            double sum = 0;
            List<Comment> comments = _service.GetAll();
            foreach (var comment in comments)
            {
                sum += comment.Rating;
            }
            return comments.Count != 0 ? Math.Round(sum / comments.Count, 2) : 0;
        }

        // GET: Comments
        public IActionResult Index()
        {
<<<<<<< HEAD
            ViewBag.RatingAvg = Average();
=======
>>>>>>> 30991e981e0c2734afde741704d2d51f1d2102a7
            return View(_service.GetAll());
        }

        public IActionResult Search(string query)
        {
            var list = _service.serch(query);
            return Json(list);
        }


        // GET: Comments/Details/5
        public IActionResult Details(int id)
        {
            Comment comment = _service.Get(id);
            if (comment == null) return NotFound();
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
        public IActionResult Create([Bind("Id,Name,Feedback,Rating")] Comment comment)
        {
            if (!(ModelState.IsValid)) return View(comment);
            _service.Create(comment);
            return RedirectToAction(nameof(Index));
        }

        // GET: Comments/Edit/5
        public IActionResult Edit(int id)
        {
            Comment comment = _service.Get(id);
            if (comment == null) return NotFound();
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Feedback,Rating")] Comment comment)
        {
            if (id != comment.Id) return NotFound();
            if (!(ModelState.IsValid)) return View(comment);
            _service.Edit(comment);
            return RedirectToAction(nameof(Index));
        }

        // GET: Comments/Delete/5
        public IActionResult Delete(int id)
        {
            Comment comment = _service.Get(id);
            if (comment == null) return NotFound();
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
