using Microsoft.EntityFrameworkCore;

namespace UserManagerAPI.Models
{
    public class UserServiceContext:DbContext
    {
        public UserServiceContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
