using Kottatar.Entities.Entity_Models;
using Microsoft.EntityFrameworkCore;

namespace Kottatar.Data
{
    public class KottatarContext : DbContext
    {
        public DbSet<Music> Musics { get; set; }

        public DbSet<Instrument> Instruments { get; set; }

        public KottatarContext(DbContextOptions<KottatarContext> ctx)
            : base(ctx)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Music>()
                .HasMany(m => m.Instruments)
                .WithOne(i => i.Music)
                .HasForeignKey(i => i.MusicId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        

    }
}
