using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportAssociation.Models;

namespace SportAssociation.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SportAssociation.Models.Fan> Fan { get; set; }
        public DbSet<SportAssociation.Models.HostRequest> HostRequest { get; set; }
    }
}