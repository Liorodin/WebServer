using WebServer.Models;

namespace WebServer.Services.Comments
{
    public interface ICommentService
    {
        public List<Comment> GetAll();

        public  Comment Get(int id);

        public void Create(Comment comment);

        public void Edit(Comment comment);

        public void Delete(int id);

        public List<Comment> serch(string query);
    }
}
