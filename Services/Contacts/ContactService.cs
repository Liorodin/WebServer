//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Http;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using WebServer.Data;
//using WebServer.Models;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;

//namespace WebServer.Services.Contacts
//{
//    public class ContactService : IContactService
//    {
//        private readonly WebServerContext _context;
//        private readonly HttpContext _httpContext;
//        public ContactService(WebServerContext context, HttpContext httpContext)
//        {
//            _context = context;
//            _httpContext = httpContext;
//        }
//        public void Delete(User current, int id)
//        {
//            if (_context.Comment == null) return;
//            Comment comment = Get(id);
//            if (comment == null) return;
//            _context.Remove(Get(id));
//            _context.SaveChanges();
//            return;
//        }

//        public void Edit(User current, Contact contact)
//        {
//            if (Get(comment.Id) == null) return;
//            comment.Time = DateTime.Now;
//            _context.Update(comment);
//            _context.SaveChanges();
//            return;
//        }

//        public Contact Get(User current, int id)
//        {
//            if (_context.Comment == null) return null;
//            Comment comment = _context.Comment.FirstOrDefault(x => x.Id == id);
//            return comment;
//        }

//        public List<Contact> GetAll(User current)
//        {
//            var user = HttpContext.User.Identity as ClaimsIdentity;
//            if (user.IsAuthenticated == false) return BadRequest();
//            string username = user.Claims.FirstOrDefault(x => x.Type == "Username").Value;

//            User? loggedUser = await _context.User.Include(x => x.Chats).FirstOrDefaultAsync(m => m.Username == username);
//            if (loggedUser == null) return NotFound();
//            if (loggedUser.Chats.Count() == 0) return Json("[]");
//            List<GetContactResponse> list = new List<GetContactResponse>();
//            foreach (Chat chat in loggedUser.Chats)
//            {
//                Chat? findChat = await _context.Chat.Include(x => x.Contact).Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == chat.Id);
//                Contact contact = findChat.Contact;
//                GetContactResponse con = new GetContactResponse(contact.Username, contact.Name, contact.Server);
//                if (findChat.Messages.Count() > 0)
//                {
//                    con.Last = findChat.Messages.Last().Content;
//                    con.Lastdate = findChat.Messages.Last().Time;
//                }
//                list.Add(con);
//            }
//            return Json(list);
//        }

//        public void Create(User current, Contact contact)
//        {
//            if (Get(comment.Id) != null) return;
//            comment.Time = DateTime.Now;
//            _context.Add(comment);
//            _context.SaveChanges();
//            return;
//        }
//    }
//}
