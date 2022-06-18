using WebServer.Models;

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

        public Task<List<MessageResponse>> GetMessages(User user, string id);

        public Task<MessageResponse> GetMessage(User user, string id, int messageId);

        public int PostMessage(User current, TempMessage tempMessage, string id);

        public int EditMessage(User current, TempMessage message, string id, int messageId);

        public int DeleteMessage(User current, string id, int messageId);

    }
}
