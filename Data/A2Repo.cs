using A2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using A2.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace A2.Data
{
    public class A2Repo : IA2Repo
    {
        private readonly A2DbContext _dbContext;

        public A2Repo(A2DbContext dbContext) {
            _dbContext = dbContext;
        }

        public User AddNewUser(User user)
        {
            User existingUser = _dbContext.Users.FirstOrDefault(e => e.UserName == user.UserName);
            // User existingUser1 = _dbContext.Find<User>(new[] { user.UserName});
            // another way to find a user with the same name ???
            if (existingUser == null)
            {
                EntityEntry<User> userToAdd = _dbContext.Users.Add(user);
                User u = userToAdd.Entity;
                _dbContext.SaveChanges();
                return u;
            }
            else
            {
                return null;
            }
        }

        public bool ValidLogin(string UserName, string Password)
        {
            User u = _dbContext.Users.FirstOrDefault(e => e.UserName == UserName && e.Password == Password);
            if (u == null)
                return false;
            return true;
        }

        public Product getProductById(int id)
        {
            Product p = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            return p;
        }

        public bool IsOrganizor(string name, string password)
        {
            Organizor o = _dbContext.Organizors.FirstOrDefault(o => o.Name == name && o.Password == password);
            if (o == null)
                return false;
            return true;
        }

        public void addEvent(Event e)
        {
            EntityEntry<Event> newEvent = _dbContext.Events.Add(e);
            Event ev = newEvent.Entity;
            _dbContext.SaveChanges();
        }

        public int countEvent()
        {
            IEnumerable<Event> events = _dbContext.Events;
            return events.Count();
        }

        public Event getEvent(int id)
        {
            Event e = _dbContext.Events.FirstOrDefault(e => e.Id == id);
            return e;
        }









        public IEnumerable<Product> GetProducts()
        {
            IEnumerable<Product> products = _dbContext.Products.ToList<Product>();
            return products;
        }

        public IEnumerable<Product> GetItemsByName(string namePartial)
        {
            IEnumerable<Product> p = _dbContext.Products.Where(e => e.Name.Contains(namePartial.ToLower()));
            return p;
        }

        public Comment getCommentById(int id)
        {
            Comment c = _dbContext.Comments.FirstOrDefault(e => e.Id == id);
            return c;
        }

        public Comment AddComment(Comment comment)
        {
            EntityEntry<Comment> c = _dbContext.Comments.Add(comment);
            Comment com = c.Entity;
            _dbContext.SaveChanges();
            return com;
        }

        public IEnumerable<Comment> getAllComments(int number)
        {
            IEnumerable<Comment> c = _dbContext.Comments.ToList<Comment>();

            //IEnumerable<Comment> comments = c.Where((value, index) => index >= (c.Count() - number)
            //&& index < c.Count());

            IEnumerable<Comment> flip = c.OrderByDescending(e => e.Id);
            IEnumerable<Comment> result = flip.Where((e, i) => i < number);
            //return comments.Reverse();
            return result;
        }
    }
}