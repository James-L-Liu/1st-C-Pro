using System;
using A2.Models;

namespace A2.Data
{
    public interface IA2Repo
    {
        public User AddNewUser(User user);
        public bool ValidLogin(string UserName, string Password);
        public Product getProductById(int id);
        public bool IsOrganizor(string name, string password);
        public void addEvent(Event e);
        public int countEvent();
        public Event getEvent(int id);




        IEnumerable<Product> GetProducts();
        IEnumerable<Product> GetItemsByName(string namePartial);
        Comment getCommentById(int id);
        IEnumerable<Comment> getAllComments(int number);
        Comment AddComment(Comment comment);
    }
}