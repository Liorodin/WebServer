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
using WebServer.Controllers;

namespace WebServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : Controller
    {
        private readonly WebServerContext _context;

        public InvitationsController(WebServerContext context)
        {
            _context = context;
        }

        public class InvitationRequest
        {
            public string? From { get; set; }
            public string? To { get; set; }
            public string? Server { get; set; }
        }

        private User GetUser(HttpContext context, string username)
        {
            return _context.User.Include(x => x.Chats).FirstOrDefault(m => m.Username == username);
        }

        [HttpPost]
        public async Task<IActionResult> PostInvitations([Bind("From, To, Server")] InvitationRequest request)
        {
            //UsersController u = new UsersController(_context, configuration);
            User sender = GetUser(HttpContext, request.From);
            //User reciever = GetUser(HttpContext, request.To);

            //
            //await _context.Chat.Include(x => x.Contact).ToListAsync();
            //var chats = sender.Chats.ToList();
            //sender.Chats.Add();

            foreach (Chat chat in sender.Chats)
            {
                Chat findChat = await _context.Chat.Include(x => x.Contact).FirstOrDefaultAsync(y => y.Id == chat.Id);
                if (findChat.Contact.Username == request.To) return BadRequest();
            }
            Chat newChat = new();
            Contact newContact = new();
            newContact.Username = request.To;
            newContact.Name = request.To;
            newContact.Server = request.Server;
            newContact.Chat = newChat;
            newChat.Messages = new List<Message>();
            newChat.Contact = newContact;
            sender.Chats.Add(newChat);
            _context.Add(newChat);
            await _context.SaveChangesAsync();
            return Created("", request);
        }
    }
}

