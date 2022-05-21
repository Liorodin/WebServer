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

namespace WebServer.Services
{
    public class CommentService : ICommentService
    {
        private readonly WebServerContext _context;
        public CommentService(WebServerContext context)
        {
            _context = context;
        }
        public void Delete(int id)
        {
            if (_context.Comment == null) return;
            Comment comment = Get(id);
            if (comment == null) return;
            _context.Remove(Get(id));
            _context.SaveChanges();
            return;
        }

        public void Edit(Comment comment)
        {
            if(Get(comment.Id)==null) return;
            comment.Time = DateTime.Now;
            _context.Update(comment);
            _context.SaveChanges();
            return;
        }

        public Comment Get(int id)
        {
            if (_context.Comment == null) return null;
            Comment comment = _context.Comment.FirstOrDefault(x => x.Id == id);
            return comment;
        }

        public  List<Comment> GetAll()
        {
            return  _context.Comment != null ? _context.Comment.ToList() : new List<Comment>();

        }

        public void Create(Comment comment)
        {
            if (Get(comment.Id) != null) return;
            comment.Time = DateTime.Now;
            _context.Add(comment);
            _context.SaveChanges();
            return;
        }

        public List<Comment> serch(string query)
        {
            var list = from comment in _context.Comment
                    where comment.Name.Contains(query) ||
                    comment.Feedback.Contains(query)
                    select comment;
            return list.ToList();
        }
    }
}
