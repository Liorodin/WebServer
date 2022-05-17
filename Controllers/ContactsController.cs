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
            public string? content { get; set; }
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
                Chat? findChat = await _context.Chat.Include(x => x.Contacts).Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == chat.Id);
                Contact contact = findChat.Contacts.First();
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

    }
}
