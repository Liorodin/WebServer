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
using WebServer.Services.Contacts;

namespace WebServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : Controller
    {
        private readonly WebServerContext _context;
        private readonly IContactService _service;

        public ContactsController(WebServerContext context)
        {
            _context = context;
            _service = new ContactService(context);
        }

        private async Task<User> GetLoggedUser(HttpContext context)
        {
            var user = context.User.Identity as ClaimsIdentity;
            if (user.IsAuthenticated == false) return null;
            string username = user.Claims.FirstOrDefault(x => x.Type == "Username").Value;
            return await _context.User.Include(x => x.Chats).FirstOrDefaultAsync(m => m.Username == username);
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();       
            if (loggedUser.Chats.Count == 0) return Json(loggedUser.Chats);
            return Json(await _service.GetAll(loggedUser));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(string id)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            var res = await _service.Get(loggedUser, id);
            if (res == null)
            {
                return NotFound();
            }
            return Json(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([Bind("Id,Name,Server")] AddContactResponse contact)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            if (_service.Create(loggedUser, contact) == 0)
            {
                return BadRequest();
            }
            else return Created("", contact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(string id)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            int res = _service.Delete(loggedUser, id);
            if (res == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditContact([Bind("Id,Name,Server")] AddContactResponse contact, string id)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            if (_service.Edit(loggedUser, contact, id) == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessages(string id)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            var messages = await _service.GetMessages(loggedUser, id);
            if (messages == null)
            {
                return NotFound();
            }
            return Json(messages);
        }

        [HttpGet("{id}/messages/{messageId}")]
        public async Task<IActionResult> GetMessage(string id, int messageId)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            var msg = await _service.GetMessage(loggedUser, id, messageId);
            if (msg == null)
            {
                return NotFound();
            }
            return Json(msg);
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> PostMessage([Bind("Content")] TempMessage tempMessage, string id)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            int result = _service.PostMessage(loggedUser, tempMessage, id);
            if (result == 1)
            {
                return Created("", tempMessage);
            }
            return NotFound();
        }

        [HttpPut("{id}/messages/{messageId}")]
        public async Task<IActionResult> EditMessage([Bind("content")] TempMessage message, string id, int messageId)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            if (_service.EditMessage(loggedUser, message, id, messageId) == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(string id, int messageId)
        {
            User loggedUser = await GetLoggedUser(HttpContext);
            if (loggedUser == null) return BadRequest();
            if (_service.DeleteMessage(loggedUser, id, messageId) == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}

