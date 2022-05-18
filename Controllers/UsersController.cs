using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly WebServerContext _context;

        public UsersController(WebServerContext context)
        {
            _context = context;
        }

        public class LoginUser
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'WebServerContext.User'  is null.");
            }

            string? loggedUsername = HttpContext.Session.GetString("username");
            if (loggedUsername == null) return BadRequest();

            if (loggedUsername != "LeonardoR" || loggedUsername != "SirinB") return BadRequest();

            User? deleteUser = await _context.User.Include(x => x.Chats).FirstOrDefaultAsync(m => m.Username == id));

            if (deleteUser == null) return BadRequest();

            foreach (var chat in deleteUser.Chats)
            {
                Chat findChat = await _context.Chat.Include(y=>y.Contact).FirstOrDefaultAsync(z=>z.Id==chat.Id);
                _context.Contact.Remove(findChat.Contact);
            }
            _context.User.Remove(deleteUser);

            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool UserExists(string id)
        {
            return (_context.User?.Any(e => e.Username == id)).GetValueOrDefault();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Register([Bind("Username,Nickname,Password,Picture")] User newUser)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'WebServerContext.User' is null.");
            }

            if (await _context.User.FindAsync(newUser.Username) != null) return BadRequest();
            if (newUser.Picture == null) newUser.Picture = "avatar";

            newUser.Chats = new List<Chat>();
            _context.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([Bind("Username,Password")] LoginUser user)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'WebServerContext.User' is null.");
            }

            var q = _context.User.Where(e => e.Username == user.Username && e.Password == user.Password);
            if (q.Any())
            {
                HttpContext.Session.SetString("username", q.First().Username);
                return Ok();
            }
            ViewData["Error"] = "Username and/or password are incorrect.";
            return NotFound();
        }
    }
}
