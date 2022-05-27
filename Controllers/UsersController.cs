using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebServer.Data;
using WebServer.Models;

namespace WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly WebServerContext _context;
        public IConfiguration _configuration;
        public UsersController(WebServerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public class LoginUser
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private User GetUser(HttpContext context, string username)
        {
            return _context.User.Include(x => x.Chats).FirstOrDefault(m => m.Username == username);
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

            User? deleteUser = await _context.User.Include(x => x.Chats).FirstOrDefaultAsync(m => m.Username == id);

            if (deleteUser == null) return BadRequest();

            foreach (var chat in deleteUser.Chats)
            {
                Chat findChat = await _context.Chat.Include(y => y.Contact).FirstOrDefaultAsync(z => z.Id == chat.Id);
                _context.Contact.Remove(findChat.Contact);
            }
            _context.User.Remove(deleteUser);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([Bind("Username,Nickname,Password,Picture")] User newUser)
        {
            if (_context.User == null) return NotFound();

            if (await _context.User.FindAsync(newUser.Username) != null) return BadRequest();

            newUser.Chats = new List<Chat>();
            _context.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([Bind("Username,Password")] LoginUser user)
        {
            if (_context.User == null) return NotFound();

            var q = _context.User.Where(e => e.Username == user.Username && e.Password == user.Password);
            if (q.Any())
            {
                var Claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub , _configuration["JWTParams:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat , DateTime.Now.ToString()),
                    new Claim("Username",user.Username)
                };
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JWTParams:Issuer"],
                    _configuration["JWTParams:Audience"],
                    Claims,
                    expires: DateTime.Now.AddMinutes(50),
                    signingCredentials: mac);
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            ViewData["Error"] = "Username and/or password are incorrect.";
            return NotFound();
        }

        [HttpPost("User")]
        public IActionResult Get(string username)
        {
            if (_context.User == null) return NotFound();
            User user = _context.User.Find(username);
            if (user ==null) return NotFound();
            user.Password = null;
            return Json(user);                  
        }
    }
}
