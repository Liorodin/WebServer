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

    }
}

