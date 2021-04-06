using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WpfTest.Entitys
{
    public class User
    {
        private const string salt = "managerTask2021";

        public int id { get; set; }
        private string login;
        private string password;


        public string Login
        {
            get => login;
            set => login = value;
        }


        public string Password
        {
            get => password;
            set => password = value;
        }


        public static User GetAdminUser()
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


        public static User RegUser(string login, string password)
        {
            User user;

            using (AppContext context = new AppContext())
            {
                user = GetUserByLogin(context, login);

                if (user == null)
                {
                    user = new User(login, GetHash(password));
                    context.Users.Add(user);
                    context.SaveChanges();
                    return user;
                }
            }
            return null;
        }


        public static User AuthByLoginAndPassword(string login, string password)
        {
            User user;
            using (AppContext context = new AppContext())
            {
                user = GetUserByLogin(context, login);

                if (user != null)
                {
                    if (ValidPassword(user.password, password))
                    {
                        return user;
                    }
                }
            }
            return null;
        }


        private static bool ValidPassword(string hashPass, string pass)
        {
            return hashPass == GetHash(pass);
        }


        public static string GetHash(string password)
        {
            password += salt; 
            using (SHA1 hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }


        public List<Task> GetArrayParentTasks()
        {
            using (AppContext context = new AppContext())
            {
                return Task.GetAllTasksByUserIdAndTaskId(context, id);
            }
        }


        public List<Task> GetArrayByIdTasks(int? parentTaskId = null)
        {
            using (AppContext context = new AppContext())
            {
                return Task.GetAllTasksByUserIdAndTaskId(context, id, parentTaskId);
            }
        }


        public static User GetUserById(AppContext context, int? userId)
        {
            return context.Users.Find(userId);
        }


        private static User GetUserByLogin(AppContext context, string login)
        {
            return context.Users.Where(b => b.Login == login).FirstOrDefault();
        }


        private User() { }


        private User(string login, string password)
        {
            this.login = login;
            this.password = password;
        }
    }
}