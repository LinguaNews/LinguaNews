// FIX 1: The 'using' statement should match your Models folder path
using LinguaNews.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LinguaNews.Data
{
    // FIX 2: You wanted the class name to be 'LinguaNewsDbContext'
    public class LinguaNewsDbContext : DbContext
    {
        // This constructor must also be updated to match the new class name
        public LinguaNewsDbContext(DbContextOptions<LinguaNewsDbContext> options)
            : base(options)
        {
        }

        // These properties will become your tables in the database
        public DbSet<ArticleSnapshot> ArticleSnapshots { get; set; }
        public DbSet<Translation> Translations { get; set; }

        internal async Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}