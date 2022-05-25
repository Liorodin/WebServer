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
            User sender = _context.User.Include(x => x.Chats).Where(u => u.Username == request.To).FirstOrDefault();
            if (sender == null)
            {
                return BadRequest();
            }
            Chat newChat = new();
            Contact newContact = new();
            newContact.Username = request.From;
            newContact.Name = request.From;
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

