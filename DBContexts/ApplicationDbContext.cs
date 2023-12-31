using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KalumAuthManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KalumAuthManagement.DBContexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}