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

namespace WebServer.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            string? loggedUsername = HttpContext.Session.GetString("username");
            if (loggedUsername == null) return NotFound();
            User? loggedUser = await _context.User.Include(x => x.Chats).FirstOrDefaultAsync(m => m.Username == loggedUsername);

            if (loggedUser == null) return NotFound();
            if (loggedUser.Chats.Count() == 0) return Json("[]");
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
            string? username = HttpContext.Session.GetString("username");
            User? loggedUser = await _context.User.Include(x => x.Chats).FirstOrDefaultAsync(m => m.Username == username);
            if (loggedUser == null) return NotFound();

            await _context.Chat.Include(x => x.Contact).ToListAsync();

            var chats = loggedUser.Chats.ToList();

             if (loggedUser.Chats.Count() == 0) return Json("[]");
            List<GetContactResponse> list = new List<GetContactResponse>();

            //MessageList? chat = await _context.MessageList.Include(x => x.Users).Include(x => x.Messages).FirstOrDefaultAsync(x => x.Users.Contains(getUser) && x.Users.Contains(loggedUser));
            //ContactResponse con = new ContactResponse(getUser.Username, getUser.Nickname, getUser.Server);
            //if (chat != null && chat.Messages.Count() > 0)
            //{
            //    con.last = chat.Messages.Last().Content;
            //    con.lastdate = chat.Messages.Last().Time;
            //}
            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([Bind("Id,Name,Server")] AddContactResponse contact)
        {
            string? loggedUsername = HttpContext.Session.GetString("username");
            if (loggedUsername == null) return NotFound();
            User? loggedUser = await _context.User.Include(x => x.Chats).FirstOrDefaultAsync(m => m.Username == loggedUsername);
            foreach (Chat chat in loggedUser.Chats)
            {
                Chat findChat = await _context.Chat.Include(x => x.Contact).FirstOrDefaultAsync(y=>y.Id==chat.Id);
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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteContact(string id)
        //{
        //    //string? username = HttpContext.Session.GetString("username");
        //    //User? loggedUser = await _context.User.Include(x => x.Conversations).Include(x => x.Contacts).FirstOrDefaultAsync(m => m.Username == username);
        //    //Contacts? removeContact = loggedUser.Contacts.Where(x => x.Username == id).FirstOrDefault();
        //    //if (removeContact == null) return BadRequest();

        //    //loggedUser.Contacts.Remove(removeContact);
        //    //// MessageList removeChat = loggedUser.Conversations.Where(x => x.Users.Where(x => x.Username == id));
        //    //await _context.SaveChangesAsync();
        //    return NoContent();
        //}


        //[HttpGet("{id}/messages")]
        //public async Task<IActionResult> GetMessages(string id)
        //{
        //    //string? username = HttpContext.Session.GetString("username");
        //    //User? loggedUser = await _context.User.Include(x => x.Conversations).FirstOrDefaultAsync(m => m.Username == username);
        //    //foreach (var chat in loggedUser.Conversations)
        //    //{
        //    //    MessageList? specificChat = await _context.MessageList.Include(x => x.Users).Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == chat.Id);
        //    //    User? user = specificChat.Users.Where(x => x.Username == id).FirstOrDefault();
        //    //    if (user != null)
        //    //    {
        //    //        List<MessageResponse> list = new List<MessageResponse>();
        //    //        foreach (var message in chat.Messages)
        //    //        {
        //    //            MessageResponse messageResponse = new MessageResponse(message.Id, message.Content, message.Time, message.To == id);
        //    //            list.Add(messageResponse);
        //    //        }
        //    //        return Json(list);
        //    //    }
        //    //}
        //    return NoContent();
        //}

        //[HttpGet("{id}/messages/{messageId}")]
        //public async Task<IActionResult> GetMessage(string id, int messageId)
        //{
        //    string? username = HttpContext.Session.GetString("username");
        //    User? loggedUser = await _context.User.Include(x => x.Conversations).FirstOrDefaultAsync(m => m.Username == username);
        //    Message? message = await _context.Message.FindAsync(messageId);
        //    if (message == null) return NoContent();
        //    if (message.from != username || message.To != id) return NoContent();
        //    MessageResponse messageResponse = new MessageResponse(messageId, message.Content, message.Time, message.To == id);
        //    return Json(messageResponse);
        //}

        //[HttpDelete("{id}/messages/{messageId}")]
        //public async Task<IActionResult> DeleteMessage(string id, int messageId)
        //{
        //    string? username = HttpContext.Session.GetString("username");
        //    User? loggedUser = await _context.User.Include(x => x.Conversations).FirstOrDefaultAsync(m => m.Username == username);
        //    Message? message = await _context.Message.FindAsync(messageId);
        //    if (message == null) return BadRequest();
        //    if (message.from != username || message.To != id) return BadRequest();
        //    _context.Message.Remove(message);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpPost("{id}/messages")]
        //public async Task<IActionResult> PostMessage([Bind("Content")] TempMessage tempMessage, string id)
        //{
        //    //string? username = HttpContext.Session.GetString("username");
        //    //User? loggedUser = await _context.User.Include(x => x.Conversations).FirstOrDefaultAsync(m => m.Username == username);
        //    //if (loggedUser == null) return BadRequest();
        //    //User? sendingTo = await _context.User.FindAsync(id);
        //    //if (sendingTo == null) return BadRequest();
        //    //Message message = new Message();
        //    //message.Content = tempMessage.content;
        //    //message.from = username;
        //    //message.To = id;
        //    //message.Time = DateTime.Now;
        //    //message.Type = "text";
        //    //MessageList? currentMessageList = await _context.MessageList.Include(x => x.Users).Include(x => x.Messages).FirstOrDefaultAsync(m => m.Users.Contains(loggedUser) && m.Users.Contains(sendingTo));
        //    //if (currentMessageList == null) return BadRequest();
        //    //message.MessageList = currentMessageList;
        //    //message.MessageListId = currentMessageList.Id;

        //    //currentMessageList.Messages.Add(message);
        //    //_context.Add(message);

        //    //await _context.SaveChangesAsync();
        //    return Created("", tempMessage);
        //}

        //[HttpPut("{id}/messages/{messageId}")]
        //public async Task<IActionResult> DeleteMessage([Bind("content")] TempMessage newMessage, string id, int messageId)
        //{
        //    string? username = HttpContext.Session.GetString("username");
        //    User? loggedUser = await _context.User.Include(x => x.Conversations).FirstOrDefaultAsync(m => m.Username == username);
        //    Message? message = await _context.Message.FindAsync(messageId);
        //    if (message == null) return BadRequest();
        //    if (message.from != username || message.To != id) return BadRequest();
        //    message.Content = newMessage.content;
        //    message.Time = DateTime.Now;
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
    }
}

