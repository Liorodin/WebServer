using WebServer.Models;

namespace WebServer.Services.Contacts
{
    public interface IContactService
    {
        public List<Contact> GetAll(User current);

        public Contact Get(User current, int id);

        public void Create(User current, Contact contact);

        public void Edit(User current, Contact contact);

        public void Delete(User current, int id);

    }
}
