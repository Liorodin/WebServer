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

        [HttpPost]
        public async Task<IActionResult> PostInvitations([Bind("From, To, Server")] InvitationRequest request)
        {
            User reciever = _context.User.Include(x => x.Chats).FirstOrDefault(y => y.Username == request.To);
            if (reciever == null) return BadRequest();

            foreach (Chat chat in reciever.Chats)
            {
                Chat findChat = await _context.Chat.Include(x => x.Contact).FirstOrDefaultAsync(y => y.Id == chat.Id);
                if (findChat.Contact.Username == request.From) return BadRequest();
            }
            Chat newChat = new();
            Contact newContact = new();
            newContact.Username = request.From;
            newContact.Name = request.From;
            newContact.Server = request.Server;
            newContact.Chat = newChat;
            newChat.Messages = new List<Message>();
            newChat.Contact = newContact;
            reciever.Chats.Add(newChat);
            _context.Add(newChat);
            await _context.SaveChangesAsync();
            return Created("", request);
        }
    }
}

