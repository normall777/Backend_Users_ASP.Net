using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplicationUsers.Models;

namespace WebApplicationUsers.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> //Пункт 5
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
