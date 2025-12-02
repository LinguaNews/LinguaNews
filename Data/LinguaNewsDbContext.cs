using LinguaNews.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguaNews.Data
{
    public class LinguaNewsDbContext : DbContext
    {
        public LinguaNewsDbContext(DbContextOptions<LinguaNewsDbContext> options)
            : base(options)
        {
        }

        // These properties will become tables in the database
        public DbSet<ArticleSnapshot> ArticleSnapshots { get; set; }
        public DbSet<Translation> Translations { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Add custom logic here (e.g., audit logging)
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}