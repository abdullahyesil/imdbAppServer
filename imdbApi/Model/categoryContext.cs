using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace imdbApi.Model
{
    public class categoryContext:DbContext
    {
        public categoryContext(DbContextOptions<categoryContext> options):base (options) {}

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //has data ile model içinde başlangıç verisi oluşturdum.
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Macera" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 2, Name = "Aksiyon" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 3, Name = "Korku" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 4, Name = "Romantik Komedi" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 5, Name = "Bilim Kurgu" });
        }


    }
}
