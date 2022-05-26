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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static WebServer.Services.Contacts.IContactService;
using static WebServer.Controllers.ContactsController;
using WebServer.Controllers;

namespace WebServer.Services.Contacts
{
    public class ContactService : IContactService
    {
        private readonly WebServerContext _context;

        public ContactService(WebServerContext context)
        {
            _context = context;
        }

        public class TempMessage
        {
            public string? Content { get; set; }
        }

        public class AddContactResponse
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? Server { get; set; }
        }

        public class MessageResponse
        {
            public MessageResponse(int id, string content, DateTime? created, bool sent)
            {
                this.Created = created;
                this.Sent = sent;
                this.Id = id;
                this.Content = content;
            }
            public int Id { get; set; }
            public string Content { get; set; }

            public DateTime? Created { get; set; }
            public bool Sent { get; set; }
        }

        public async Task<List<GetContactResponse>> GetAll(User current)
        {
            List<GetContactResponse> list = new List<GetContactResponse>();
            foreach (Chat chat in current.Chats)
            {
                Chat? findChat = await _context.Chat.Include(x => x.Contact).Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == chat.Id);
                Contact contact = findChat.Contact;
                GetContactResponse con = new GetContactResponse(contact.Username, contact.Name, contact.Server);
                if (findChat.Messages.Count() > 0)
                {
                    con.Last = findChat.Messages.Last().Content;
                    con.Lastdate = findChat.Messages.Last().Time;
                }
                list.Add(con);
            }
            return list;
        }

        public async Task<GetContactResponse> Get(User current, string id)
        {

            var chats = current.Chats.ToList();

            List<GetContactResponse> list = new List<GetContactResponse>();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return null;
            Chat? findChat = await _context.Chat.Include(x => x.Messages).Include(x => x.Contact).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (current.Chats.Contains(findChat))
            {
                GetContactResponse con = new GetContactResponse(findContact.Username, findContact.Name, findContact.Server);
                if (findChat != null && findChat.Messages.Count() > 0)
                {
                    con.Last = findChat.Messages.Last().Content;
                    con.Lastdate = findChat.Messages.Last().Time;
                }
                return con;
            }
            return null;
        }


        // delete a contact
        public int Delete(User current, string messageId)
        {

            _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = current.Chats.ToList();

            List<GetContactResponse> list = new List<GetContactResponse>();

            Contact findContact = _context.Contact.FirstOrDefault(x => x.Username == messageId);

            if (findContact == null) return 0;
            Chat? findChat = _context.Chat.Include(x => x.Messages).FirstOrDefault(x => x.Id == findContact.ChatId);
            if (current.Chats.Contains(findChat))
            {
                Message findMessage = _context.Message.Find(messageId);
                if (findMessage == null) return 0;
                if (!(findChat.Messages.Contains(findMessage))) return 0;
                findChat.Messages.Remove(findMessage);
                _context.SaveChanges();
                return 0;
            }
            return 0;
        }

        public int Create(User current, Contacts.AddContactResponse contact)
        {
            foreach (Chat chat in current.Chats)
            {
                Chat findChat = _context.Chat.Include(x => x.Contact).FirstOrDefault(y => y.Id == chat.Id);
                if (findChat.Contact.Username == contact.Id) return 0;
            }
            Chat newChat = new();
            Contact newContact = new();
            newContact.Username = contact.Id;
            newContact.Name = contact.Name;
            newContact.Server = contact.Server;
            newContact.Chat = newChat;
            newChat.Messages = new List<Message>();
            newChat.Contact = newContact;
            current.Chats.Add(newChat);
            _context.Add(newChat);
            _context.Add(newContact);
            _context.SaveChangesAsync();
            return 1; 
        }

        public int Edit(User current, Contacts.AddContactResponse contact, string id)
        {
            _context.Chat.Include(x => x.Contact).ToList();
            var chats = current.Chats.ToList();

            Contact findContact = _context.Contact.FirstOrDefault(x => x.Username == id);
            if (findContact == null || findContact.Server != contact.Server) return 0;
            Chat? findChat = _context.Chat.Include(x => x.Messages).FirstOrDefault(x => x.Id == findContact.ChatId);
            if (current.Chats.Contains(findChat))
            {
                findContact.Name = contact.Name;
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        //public Task<int> Create(User current, ContactsController.AddContactResponse contact)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> Edit(User current, ContactsController.AddContactResponse contact, string id)
        //{
        //    throw new NotImplementedException();
        //}









        //////////////////////////////////////////////////////////////
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
    }
}
