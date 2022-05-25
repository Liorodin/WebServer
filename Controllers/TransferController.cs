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
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : Controller
    {
        private readonly WebServerContext _context;

        public TransferController(WebServerContext context)
        {
            _context = context;
        }

        public class TransferRequest
        {
            public string? From { get; set; }
            public string? To { get; set; }
            public string? Content { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> PostTransfer([Bind("From, To, Content")] TransferRequest request)
        {
            User sender = _context.User.Include(x => x.Chats).Where(u => u.Username == request.From).FirstOrDefault();
            if (sender == null)
            {
                return BadRequest();
            }

            await _context.Chat.Include(x => x.Contact).ToListAsync();
            var chats = sender.Chats.ToList();

            Contact findContact = await _context.Contact.FirstOrDefaultAsync(x => x.Username == request.To);
            if (findContact == null) return NotFound();
            Chat? findChat = await _context.Chat.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == findContact.ChatId);
            if (sender.Chats.Contains(findChat))
            {
                Message message = new Message();
                message.Content = request.Content;
                message.from = request.From;
                message.To = request.To;
                message.Time = DateTime.Now;
                message.Type = "text";
                findChat.Messages.Add(message);
                await _context.SaveChangesAsync();
                return Created("", request);
            }
            return NotFound();
        }
    }
}

