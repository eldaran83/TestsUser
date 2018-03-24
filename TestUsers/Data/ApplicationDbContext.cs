using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestUsers.Models;
using TestUsers.Models.BO;

namespace TestUsers.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Utilisateur> Utilisateurs { get; set; } // save en BDD les utilisateurs
        public DbSet<Aventure> Aventures { get; set; } //save les aventures
        public DbSet<MessageAventure> LesMessagesDesAventures { get; set; } //save les messages de l aventure

    }
}
