using Microsoft.EntityFrameworkCore;
using WebAPI.DbEntities;

namespace WebAPI.DbContexts
{
    public class BankDbContext: DbContext
    {
        public BankDbContext(DbContextOptions dbContextOptions)
           : base(dbContextOptions) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
    }
}
