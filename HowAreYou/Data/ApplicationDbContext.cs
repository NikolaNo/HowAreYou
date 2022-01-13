using BlazorLinks.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HowAreYou.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<SentimentAI> SentimentAIs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}