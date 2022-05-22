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

        private User GetUser(HttpContext context)
        {
            var user = context.User.Identity as ClaimsIdentity;
            if (user.IsAuthenticated == false) return null;
            string username = user.Claims.FirstOrDefault(x => x.Type == "Username").Value;
            return _context.User.Include(x => x.Chats).FirstOrDefault(m => m.Username == username);
        }

        [HttpPost]
        public async Task<IActionResult> PostInvitations([Bind("From, To, Server")] InvitationRequest request)
        {

            //GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            //var invitation = new Invitation
            //{
            //    InvitedUserEmailAddress = "admin@fabrikam.com",
            //    InviteRedirectUrl = "https://myapp.contoso.com"
            //};

            //await graphClient.Invitations
            //    .Request()
            //    .AddAsync(invitation);


            //User sender = GetUser(HttpContext);
            //User reciever = GetUser(HttpContext);

            //await _context.Chat.Include(x => x.Contact).ToListAsync();
            //var chats = sender.Chats.ToList();
            ////sender.Chats.Add();

            //Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == id);
            //if (findContact == null) return NotFound();
            //Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            //if (sender.Chats.Contains(findChat))
            //{
            //    Message message = new Message();
            //    message.from = request.From;
            //    message.To = request.To; 
            //    findChat.Messages.Add(message);
            //    await _context.SaveChangesAsync();
            //    return Created("", request);
            //}

            ////Chat newChat = new();


            return NotFound();
        }

    }
}

