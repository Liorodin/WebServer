﻿using System;
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

namespace WebServer.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _service;

        public CommentsController(WebServerContext context)
        {
            _service = new CommentService(context);
        }

        // GET: Comments
        public IActionResult Index()
        {
<<<<<<< HEAD
              return _context.Comment != null ?                
                          View(await _context.Comment.ToListAsync()) :
                          Problem("Entity set 'WebServerContext.Comment'  is null.");
=======
            return View(_service.GetAll());
>>>>>>> 14e8010f0d4363f10b40fb10fce2e6e3d0168d7b
        }

        public async Task<IActionResult> Search(string query)
        {
            var list = _service.serch(query);
            return Json(list);
        }


        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int id)
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
        public async Task<IActionResult> Create([Bind("Id,Name,Feedback,Rating")] Comment comment)
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
