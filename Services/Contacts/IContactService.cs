using Microsoft.AspNetCore.Mvc;
using WebServer.Models;
using WebServer.Controllers;
using static WebServer.Controllers.ContactsController;

namespace WebServer.Services.Contacts
{

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

    public class GetContactResponse
    {
        public GetContactResponse(string? id, string? name, string? server)
        {
            this.Id = id;
            this.Name = name;
            this.Server = server;
        }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Server { get; set; }
        public string? Last { get; set; }
        public DateTime? Lastdate { get; set; }
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

    public interface IContactService
    {

        public  Task<List<GetContactResponse>> GetAll(User current);

        public  Task<GetContactResponse> Get(User current, string id);

        public int Create(User current, AddContactResponse contact);

        public int Edit(User current, AddContactResponse contact, string id);

        public int Delete(User current, string id);

        //public Task<IActionResult> GetMessages(string id);

        //public Task<IActionResult> GetMessage(string id, int messageId);

        //public Task<IActionResult> PostMessage(TempMessage tempMessage, string id);

        //public Task<IActionResult> EditMessage(TempMessage message, string id, int messageId);

        //public Task<IActionResult> DeleteMessage(string id, int messageId);

    }
}
