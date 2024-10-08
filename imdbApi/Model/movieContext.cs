﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Numerics;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;
using imdbApi.Model.Entity;
using imdbApi.Model.Entity.Surveys;

namespace imdbApi.Model
{
    public class movieContext : IdentityDbContext<appUser, appRole, int>
    {

        public movieContext(DbContextOptions<movieContext> option) : base(option) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<imdbSettings> imdbSettings { get; set; }
        public DbSet<ImdbAppStory> imdbAppStory { get; set; }
        public DbSet<Rate> Rate { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<Actors> Actors { get; set; }
        public DbSet<homePageSettings> homePageSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);  // hatdata ile başlangıç verileri oluşturuyoruz.

            // MovieActor için anahtar ve ilişkileri tanımlama
            builder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            builder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors) // Doğru ilişki kurmak için MovieActors kullanılmalı
                .HasForeignKey(ma => ma.MovieId);

            builder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors) // Doğru ilişki kurmak için MovieActors kullanılmalı
                .HasForeignKey(ma => ma.ActorId);

            builder.Entity<Rate>()
       .HasKey(r => r.rate_id); // rate_id'yi birincil anahtar olarak tanımlıyoruz
            builder.Entity<Category>().HasData(new Category { Id = 1, Name = "Macera" });
            builder.Entity<Category>().HasData(new Category { Id = 2, Name = "Aksiyon" });
            builder.Entity<Category>().HasData(new Category { Id = 3, Name = "Korku" });
            builder.Entity<Category>().HasData(new Category { Id = 4, Name = "Romantik Komedi" });
            builder.Entity<Category>().HasData(new Category { Id = 5, Name = "Bilim Kurgu" });
            builder.Entity<Movie>().HasData(new Movie() { id = 1, movieName = "Esaretin Bedeli", description = "Over the course of several years, two convicts form a friendship, seeking consolation and, eventually, redemption through basic compassion.", imageUrl = "https://i.imgur.com/MVLnB1j.jpg", releaseDate = new DateTime(1999, 12, 4, 0, 0, 0, DateTimeKind.Utc), rate = 4.8, trailer = "https://www.youtube.com/embed/g-6g2uEjF1s?si=nylvjTIQU3in_fmK", categoryId = 5 });
            builder.Entity<Movie>().HasData(new Movie() { id = 2, movieName = "Baba", description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.", imageUrl = "https://i.imgur.com/ISX6YEd.jpeg", releaseDate = new DateTime(1999, 12, 4, 0, 0, 0, DateTimeKind.Utc), rate = 5, trailer = "https://www.youtube.com/embed/g-6g2uEjF1s?si=nylvjTIQU3in_fmK", categoryId = 1 });
            builder.Entity<Movie>().HasData(new Movie()
            {
                id = 3,
                movieName = "Kara Şövalye",
                description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                imageUrl = "https://i.imgur.com/s96o1bI.jpeg",
                trailer = "https://www.youtube.com/embed/g-6g2uEjF1s?si=nylvjTIQU3in_fmK",
                releaseDate = new DateTime(1999, 12, 4, 0, 0, 0, DateTimeKind.Utc),
                rate = 4.0,
                categoryId = 2
            });
            builder.Entity<Movie>().HasData(new Movie()
            {
                id = 4,
                movieName = "Yüzüklerin Efendisi: Kralın Dönüşü",
                description = "Gandalf and Aragorn lead the World of Men against Saurons army to draw his gaze from Frodo and Sam as they approach Mount Doom with the One Ring.",
                imageUrl = "https://i.imgur.com/XVa7aaQ.jpeg",
                releaseDate = new DateTime(1999, 12, 4, 0, 0, 0, DateTimeKind.Utc),
                rate = 4.4,
                categoryId = 4,
                trailer = "https://www.youtube.com/embed/g-6g2uEjF1s?si=nylvjTIQU3in_fmK"
            });

            builder.Entity<Movie>().HasData(new Movie()
            {
                id = 5,
                movieName = "Schindlerin Listesi",
                description = "In German - occupied Poland during World War II, industrialist Oskar Schindler gradually becomes concerned for his Jewish workforce after witnessing their persecution by the Nazis.",
                imageUrl = "https://i.imgur.com/WXEcXuC.jpeg",
                releaseDate = new DateTime(1999, 12, 4, 0, 0, 0, DateTimeKind.Utc),
                rate = 4.5,
                trailer = "https://www.youtube.com/embed/g-6g2uEjF1s?si=nylvjTIQU3in_fmK",
                categoryId = 5
            });

            builder.Entity<imdbSettings>().HasData(new imdbSettings()
            {
                Id = 1,
                Name = "Movie App",
                Description = "Açıklama Satırı",
                Author = "Abdullah Yeşil",
                Meta = new List<string> { "MovieApp", "imdbApp" }
            });


            builder.Entity<homePageSettings>().HasData(new homePageSettings()
            {
                Id = 1,
                h1Name = "",
                carousel = true,
                carouselBaslik = "Popüler Filmler",
                carouselId = new List<int> { 2, 1 },
                surveys = true,
                surveyId = 2,
                imdbAppStory = true,
                imdbAppStoryId = new List<int> { 1, 2 }
            });

        }

    }
}
