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

namespace WebServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : Controller
    {
        private readonly WebServerContext _context;

        public ContactsController(WebServerContext context)
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

        public class GetContactResponse
        {
            public GetContactResponse(string? id, string? name, string? server)
            {
                this.Id = id;
                this.Name = name;
                this.Server = server;
            }
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? Server { get; set; }
            public string? Last { get; set; }
            public DateTime? Lastdate { get; set; }
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

        private User GetLoggedUser(HttpContext context)
        {
            var user = context.User.Identity as ClaimsIdentity;
            if (user.IsAuthenticated == false) return null;
            string username = user.Claims.FirstOrDefault(x => x.Type == "Username").Value;
            return _context.User.Include(x => x.Chats).FirstOrDefault(m => m.Username == username);
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            User loggedUser = GetLoggedUser(HttpContext);
            if (loggedUser.Chats.Count == 0) return Json("[]");
            List<GetContactResponse> list = new List<GetContactResponse>();
            foreach (Chat chat in loggedUser.Chats)
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
            return Json(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(string id)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();

            var chats = loggedUser.Chats.ToList();

            List<GetContactResponse> list = new List<GetContactResponse>();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                GetContactResponse con = new GetContactResponse(findContact.Username, findContact.Name, findContact.Server);
                if (findChat != null && findChat.Messages.Count() > 0)
                {
                    con.Last = findChat.Messages.Last().Content;
                    con.Lastdate = findChat.Messages.Last().Time;
                }
                return Json(con);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([Bind("Id,Name,Server")] AddContactResponse contact)
        {
            User loggedUser = GetLoggedUser(HttpContext);
            foreach (Chat chat in loggedUser.Chats)
            {
                Chat findChat = await _context.Chat.Include(x => x.Contact).FirstOrDefaultAsync(y => y.Id == chat.Id);
                if (findChat.Contact.Username == contact.Id) return BadRequest();
            }
            Chat newChat = new();
            Contact newContact = new();
            newContact.Username = contact.Id;
            newContact.Name = contact.Name;
            newContact.Server = contact.Server;
            newContact.Chat = newChat;
            newChat.Messages = new List<Message>();
            newChat.Contact = newContact;
            loggedUser.Chats.Add(newChat);
            _context.Add(newChat);
            await _context.SaveChangesAsync();
            return Created("", contact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(string id)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();

            var chats = loggedUser.Chats.ToList();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                loggedUser.Chats.Remove(findChat);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditContact([Bind("Id,Name,Server")] AddContactResponse contact ,string id)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = loggedUser.Chats.ToList();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null || findContact.Server!=contact.Server) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                findContact.Name = contact.Name;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessages(string id)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = loggedUser.Chats.ToList();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                List<MessageResponse> messages = new();
                foreach (var message in findChat.Messages)
                {
                    MessageResponse messageResponse = new MessageResponse(message.Id, message.Content, message.Time, message.from == loggedUser.Username);
                    messages.Add(messageResponse);
                }
                return Json(messages);
            }
            return NotFound();
        }

        [HttpGet("{id}/messages/{messageId}")]
        public async Task<IActionResult> GetMessage(string id, int messageId)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = loggedUser.Chats.ToList();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                Message findMessage = await _context.Message.FindAsync(messageId);
                if (findMessage == null) return NotFound();
                if (!(findChat.Messages.Contains(findMessage))) return NotFound();
                MessageResponse messageResponse = new MessageResponse(findMessage.Id, findMessage.Content, findMessage.Time, findMessage.from == loggedUser.Username);
                return Json(messageResponse);
            }
            return NotFound();
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> PostMessage([Bind("Content")] TempMessage tempMessage, string id)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = loggedUser.Chats.ToList();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                Message message = new Message();
                message.Content = tempMessage.Content;
                message.from = loggedUser.Username;
                message.To = id;
                message.Time = DateTime.Now;
                message.Type = "text";
                findChat.Messages.Add(message);
                await _context.SaveChangesAsync();
                return Created("", tempMessage);
            }
            return NotFound();
        }

        [HttpPut("{id}/messages/{messageId}")]
        public async Task<IActionResult> EditMessage([Bind("content")] TempMessage message, string id, int messageId)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = loggedUser.Chats.ToList();

            List<GetContactResponse> list = new List<GetContactResponse>();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                Message findMessage = await _context.Message.FindAsync(messageId);
                if(findMessage == null) return NotFound();
                if(!(findChat.Messages.Contains(findMessage))) return NotFound();
                findMessage.Content = message.Content;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(string id, int messageId)
        {
            User loggedUser = GetLoggedUser(HttpContext);

            await _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = loggedUser.Chats.ToList();

            List<GetContactResponse> list = new List<GetContactResponse>();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (loggedUser.Chats.Contains(findChat))
            {
                Message findMessage = await _context.Message.FindAsync(messageId);
                if (findMessage == null) return NotFound();
                if (!(findChat.Messages.Contains(findMessage))) return NotFound();
                findChat.Messages.Remove(findMessage);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }
    }
}

