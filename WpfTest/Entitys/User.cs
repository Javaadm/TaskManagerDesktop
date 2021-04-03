using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfTest.Entitys { 
    public class User
    {
        public int id { get; set; }
        private string login;
        private string password;

        public string Login
        {
            get { return login; }
            set { login = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public static User getAdminUser()
        {
            User user;
            using (AppContext context = new AppContext())
            {
                user = context.Users.Where(b => b.Login == "admin" && b.Password == "admin").FirstOrDefault();

                if (user == null)
                {
                    user = new User("admin", "admin");
                    context.Users.Add(user);
                    context.SaveChanges();
                }

            }

            return user;
                       
        }

        public Task[] getArrayParentTasks()
        {
            Task[] tasks;
            using (AppContext context = new AppContext())
            {
                tasks = context.Tasks.Where(b => b.UserId == this.id && b.TaskId == null).ToArray();
            }
            return tasks;
        }

        public Task[] getArrayByIdTasks(int? parentTaskId = null)
        {
            Task[] tasks;
            using (AppContext context = new AppContext())
            {
                tasks = context.Tasks.Where(b => b.UserId == this.id && b.TaskId == parentTaskId).ToArray();
            }
            return tasks;
        }

        public User() { }

        public User(string login, string password)
        {
            this.login = login;
            this.password = password;
        }
     }
}