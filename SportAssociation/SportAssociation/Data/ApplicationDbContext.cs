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
        public DbSet<SportAssociation.Models.Manager> Manager { get; set; }
        public DbSet<SportAssociation.Models.Stadium> Stadium { get; set; }
        public DbSet<SportAssociation.Models.Super_User> Super_User { get; set; }
        public DbSet<SportAssociation.Models.Club> Club { get; set; }
        public DbSet<SportAssociation.Models.Association_Manager> Association_Manager { get; set; }
        public DbSet<SportAssociation.Models.Representative> Representative { get; set; }
        public DbSet<SportAssociation.Models.System_Admin> System_Admin { get; set; }
        public DbSet<SportAssociation.Models.Ticket> Ticket { get; set; }
        public DbSet<SportAssociation.Models.Ticket_Buying_Transactions> Ticket_Buying_Transactions { get; set; }
        public DbSet<SportAssociation.Models.Match> Match { get; set; }
    }
}