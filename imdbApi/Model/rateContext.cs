using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace imdbApi.Model
{
    public class rateContext:DbContext
    {
        public rateContext(DbContextOptions<rateContext> options) : base(options) { }
         public DbSet<Rate>  Rate { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rate>()
                .HasKey(r => r.rate_id); // rate_id'yi birincil anahtar olarak tanımlıyoruz

            base.OnModelCreating(modelBuilder);
        }
    }


}
