using System.Data.Entity;
using WpfTest.Entitys;

namespace WpfTest
{
    public class AppContext : DbContext
    {
        public AppContext()
            : base("DbConnection")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<State> States { get; set; }


    }
}
