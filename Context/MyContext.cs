using LoginAndRegistration.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginAndRegistration.Context
{
    public class MyContext : DbContext
    {
        public MyContext (DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get; set;}
    }
}