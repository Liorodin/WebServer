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
using static WebServer.Services.Contacts.IContactService;
using static WebServer.Controllers.ContactsController;
using WebServer.Controllers;

namespace WebServer.Services.Contacts
{
    public class ContactService : IContactService
    {
        private readonly WebServerContext _context;

        public ContactService(WebServerContext context)
        {
            _context = context;
        }

        public class TempMessage
        {
            public string? Content { get; set; }
        }

        public class AddContactResponse
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? Server { get; set; }
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

        public async Task<List<GetContactResponse>> GetAll(User current)
        {
            List<GetContactResponse> list = new List<GetContactResponse>();
            foreach (Chat chat in current.Chats)
            {
                Chat? findChat = await _context.Chat.Include(x => x.Contact).Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == chat.Id);
                Contact contact = findChat.Contact;
                GetContactResponse con = new GetContactResponse(contact.Username, contact.Name, contact.Server);
                if (findChat.Messages.Count() > 0)
                {
                    con.Last = findChat.Messages.Last().Content;
                    con.Lastdate = findChat.Messages.Last().Time;
                }
                list.Add(con);
            }
            return list;
        }

        public async Task<GetContactResponse> Get(User current, string id) 
        {
            foreach (Chat chat in current.Chats.ToList()) 
            { 
                Chat findChat = await _context.Chat.Include(x => x.Messages).Include(x => x.Contact).FirstOrDefaultAsync(x => x.Id == chat.Id);
                if (findChat.Contact.Username == id) 
                { 
                    GetContactResponse con = new GetContactResponse(findChat.Contact.Username, findChat.Contact.Name, findChat.Contact.Server);
                    if (findChat.Messages.Count > 0) 
                    {
                        con.Last = findChat.Messages.Last().Content;
                        con.Lastdate = findChat.Messages.Last().Time; 
                    } return con; 
                }
            } 
            return null;
        }


        // delete a contact
        public int Delete(User current, string id)
        {
            foreach (Chat chat in current.Chats.ToList())
            {
                Chat findChat = _context.Chat.Include(x => x.Messages).Include(x => x.Contact).FirstOrDefault(x => x.Id == chat.Id);
                if (findChat.Contact.Username == id)
                {
                    current.Chats.Remove(findChat);
                    _context.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }

        public int Create(User current, Contacts.AddContactResponse contact)
        {
            foreach (Chat chat in current.Chats)
            {
                Chat findChat = _context.Chat.Include(x => x.Contact).FirstOrDefault(y => y.Id == chat.Id);
                if (findChat.Contact.Username == contact.Id) return 0;
            }
            Chat newChat = new();
            Contact newContact = new();
            newContact.Username = contact.Id;
            newContact.Name = contact.Name;
            newContact.Server = contact.Server;
            newContact.Chat = newChat;
            newChat.Messages = new List<Message>();
            newChat.Contact = newContact;
            current.Chats.Add(newChat);
            _context.Add(newChat);
            _context.Add(newContact);
            _context.SaveChanges();
            return 1; 
        }

        public int Edit(User current, Contacts.AddContactResponse contact, string id)
        {
            foreach (Chat chat in current.Chats.ToList())
            {
                Chat findChat = _context.Chat.Include(x => x.Messages).Include(x => x.Contact).FirstOrDefault(x => x.Id == chat.Id);
                if (findChat.Contact.Username == id)
                {
                    findChat.Contact.Name = contact.Name;
                    findChat.Contact.Server = contact.Server;
                    _context.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }

        public async Task<List<Contacts.MessageResponse>> GetMessages(User current, string id)
        {
            var chats = current.Chats.ToList();

            foreach (var chat in chats)
            {
                Chat currentChat = await _context.Chat.Include(x => x.Contact).Include(z => z.Messages).FirstOrDefaultAsync(y => y.Id == chat.Id);
                if (currentChat.Contact.Username == id)
                {
                    List<Contacts.MessageResponse> messages = new();
                    foreach (var message in currentChat.Messages)
                    {
                        Contacts.MessageResponse messageResponse = new Contacts.MessageResponse(message.Id, message.Content, message.Time, message.from == current.Username);
                        messages.Add(messageResponse);
                    }
                    return messages;
                }
            }
            return null;
        }

        public async Task<Contacts.MessageResponse> GetMessage(User current, string id, int messageId) {


            List<Contacts.MessageResponse> msgs = await GetMessages(current, id);
            return msgs.FirstOrDefault(x => x.Id == messageId);
        }

        public int PostMessage(User current, Contacts.TempMessage tempMessage, string id)
        {
            var chats = current.Chats.ToList();
            foreach (var chat in chats)
            {
                Chat currentChat = _context.Chat.Include(x => x.Contact).Include(z => z.Messages).FirstOrDefault(y => y.Id == chat.Id);
                if (currentChat.Contact.Username == id)
                {
                    Message message = new Message();
                    message.Content = tempMessage.Content;
                    message.from = current.Username;
                    message.To = id;
                    message.Time = DateTime.Now;
                    message.Type = "text";
                    currentChat.Messages.Add(message);
                    _context.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }

        public int EditMessage(User current, Contacts.TempMessage message, string id, int messageId)
        {
            var chats = current.Chats.ToList();
            foreach (var chat in chats)
            {
                Chat currentChat = _context.Chat.Include(x => x.Contact).Include(z => z.Messages).FirstOrDefault(y => y.Id == chat.Id);
                if (currentChat.Contact.Username == id)
                {
                    Message findMessage = _context.Message.Find(messageId);
                    if (findMessage == null) return 0;
                    if (!(currentChat.Messages.Contains(findMessage))) return 0;
                    findMessage.Content = message.Content;
                    _context.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }

        public int DeleteMessage(User current, string id, int messageId)
        {
            var chats = current.Chats.ToList();

            foreach (var chat in chats)
            {
                Chat currentChat = _context.Chat.Include(x => x.Contact).Include(z => z.Messages).FirstOrDefault(y => y.Id == chat.Id);
                if (currentChat.Contact.Username == id)
                {
                    Message findMessage = _context.Message.Find(messageId);
                    if (findMessage == null) return 0;
                    if (!(currentChat.Messages.Contains(findMessage))) return 0;
                    currentChat.Messages.Remove(findMessage);
                    _context.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }
    }
}
