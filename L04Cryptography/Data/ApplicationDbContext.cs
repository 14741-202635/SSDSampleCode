using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace L04Cryptography.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
    public DbSet<L04Cryptography.Models.BankAccount> BankAccount { get; set; } = default!;
    public DbSet<L04Cryptography.Models.Password> Password { get; set; } = default!;
    }
}
