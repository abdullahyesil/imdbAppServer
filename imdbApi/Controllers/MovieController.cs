﻿using imdbApi.DTO;
using imdbApi.Model;
using imdbApi.Model.Entity;
using imdbApi.Services.Abrastract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace imdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase


    {
        private readonly movieContext _moviecontext;
        private readonly IFileService _fileService;

        public MovieController(movieContext context, IFileService fileService)
        {
            _moviecontext = context;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> getMovies([FromQuery] int? page, int? size, string? value, int? categoryId)
        {

            int totalCount;
            List<MovieDto> Movies;
            if (!string.IsNullOrEmpty(value))
            {
                var normalizedValue = value.ToLower(); // Arama terimini küçük harfe çevirin

                Movies = await _moviecontext.Movies
                    .Where(i => i.movieName.ToLower().Contains(normalizedValue)) // Veritabanı sorgusunda da küçük harfe çevirin
                    .Skip(page * size ?? 0)
                    .Take(size ?? 10) // Varsayılan olarak bir sayfa boyutu belirleyin
                    .Select(r => new MovieDto { Id = r.id, MovieName = r.movieName, CategoryId = r.categoryId, ImageUrl = r.imageUrl, Rate = r.rate, releaseDate = r.releaseDate, Description = r.description })
                    .ToListAsync();

                totalCount = _moviecontext.Movies
                   .Where(i => i.movieName.ToLower().Contains(normalizedValue)).Count();


            }
            else if (categoryId != null)
            {

                Movies = await _moviecontext.Movies
                    .Where(i => i.categoryId == categoryId)
                    .Skip(page * size ?? 0)
                    .Take(size ?? 10) // Varsayılan olarak bir sayfa boyutu belirleyin
                    .Select(r => new MovieDto { Id = r.id, MovieName = r.movieName, CategoryId = r.categoryId, ImageUrl = r.imageUrl, Rate = r.rate, releaseDate = r.releaseDate, Description = r.description })
                    .ToListAsync();

                totalCount = _moviecontext.Movies
                   .Where(i => i.categoryId == categoryId).Count();

            }

            else
            {
                totalCount = _moviecontext.Movies.Count();
                Movies = await _moviecontext.Movies
                    .Skip(page * size ?? 0)
                    .Take(size ?? 10) // Varsayılan olarak bir sayfa boyutu belirleyin
                    .Select(r => new MovieDto { Id = r.id, MovieName = r.movieName, CategoryId = r.categoryId, ImageUrl = r.imageUrl, Rate = r.rate, releaseDate = r.releaseDate, Description = r.description })
                    .ToListAsync();
               
            }



            var result = new
            {
                TotalCount = totalCount,
                Movies = Movies
            };

            return Ok(result);


        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var movie = await _moviecontext.Movies
                .Where(i => i.id == id)
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync();

            if (movie == null)
            {
                return NotFound();
            }

            // Movie entity'sini MovieDto'ya dönüştürme
            var movieDto = new MovieDto
            {
                Id = movie.id,
                MovieName = movie.movieName,
                Description = movie.description,
                releaseDate = movie.releaseDate,
                ImageUrl = movie.imageUrl,
                Rate = movie.rate,
                Trailer = movie.trailer,
                CategoryId = movie.categoryId,
                Actors = movie.MovieActors.Select(ma => new ActorDto
                {
                    Id = ma.Actor.Id,
                    Name = ma.Actor.Name,
                    imageUrl= ma.Actor.imageUrl
                }).ToList()
            };

            return Ok(movieDto);
        }


        [HttpPost]
        public async Task<IActionResult> AddMovie([FromForm]MovieDto model)
        {
            // releaseDate'nin Kind'ini UTC olarak ayarlayalım, saat bilgisini sıfırlayalım
            model.releaseDate = DateTime.SpecifyKind(model.releaseDate.Date, DateTimeKind.Utc);

           

            if (model.ImageFile != null)
            {
                var fileResult = _fileService.SaveImage(model.ImageFile, 2400, 1600);
           

                if (fileResult.Item1 == 1 && !string.IsNullOrEmpty(fileResult.Item2))
                {
                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                 

                    // Tam URL'yi oluşturuyoruz
                    model.ImageUrl = Path.Combine(baseUrl, "res", fileResult.Item2).Replace("\\", "/");
                
                }
            }
            if (model.CarouselImageFile != null)
            {
                var fileResult = _fileService.SaveImage(model.CarouselImageFile, 831, 500);
              

                if (fileResult.Item1 == 1 && !string.IsNullOrEmpty(fileResult.Item2))
                {
                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    

                    // Tam URL'yi oluşturuyoruz
                    model.CarouselImage = Path.Combine(baseUrl, "res", fileResult.Item2).Replace("\\", "/");
                    
                }
            }



            // MovieDto'yu Movie entity'sine dönüştürelim
            var movieEntity = new Movie
            {
                movieName = model.MovieName,
                description = model.Description,
                releaseDate = model.releaseDate,
                rate = model.Rate,
                categoryId = model.CategoryId,
                imageUrl = model.ImageUrl,
                carouselImage = model.CarouselImage,
                trailer = model.Trailer,
                MovieActors = model.Actors.Select(ma => new MovieActor
                {
                    ActorId = ma.Id ?? 0
                    // Burada MovieId ayarlanmıyor çünkü EF tarafından otomatik olarak yapılır
                }).ToList()
            };


            // Entity'i veritabanına ekleyelim
            _moviecontext.Movies.Add(movieEntity);
            await _moviecontext.SaveChangesAsync();
            return CreatedAtAction(nameof(getMovies), new { id = movieEntity.id }, new { message = "Film başarıyla eklendi " });

        }


        [HttpDelete("delete")]
        public async Task<IActionResult> deleteMovie(int? Id)
        {
            if (Id != null)
            {
                var dltModel = await _moviecontext.Movies.FirstOrDefaultAsync(i => i.id == Id);

                if (dltModel != null)
                {
                    _moviecontext.Movies.Remove(dltModel);
                    try
                    {
                        await _moviecontext.SaveChangesAsync();
                    }
                    catch (Exception err)
                    {

                        return NotFound($"Error: {err}");
                    }
                    return Ok("Başarıyla silindi");

                }

                return BadRequest("model bulunamadı");
            }

            return NotFound("İd eşleşmiyor");
        }

        [HttpPut("updateMovie/{id}")]
        public async Task<IActionResult> updateMovie([FromForm] MovieDto entity, int id)
        {
            if (id != entity.Id)
            {
                return BadRequest("ID'ler uyuşmuyor");
            }

            var movie = await _moviecontext.Movies.Include(m => m.MovieActors).FirstOrDefaultAsync(i => i.id == id);
            if (movie == null)
            {
                return NotFound("Film bulunamadı");
            }

            movie.movieName = entity.MovieName;
            movie.description = entity.Description;
            movie.imageUrl = entity.ImageUrl;
            movie.trailer = entity.Trailer;
            movie.carouselImage = entity.CarouselImage;
            movie.releaseDate = DateTime.SpecifyKind(entity.releaseDate.Date, DateTimeKind.Utc);
            movie.rate = entity.Rate;
            movie.categoryId = entity.CategoryId;

            // Mevcut aktörleri temizle
            movie.MovieActors.Clear();

            // Yeni aktörleri ekle
            movie.MovieActors = entity.Actors.Select(ma => new MovieActor
            {
                ActorId = ma.Id ?? 0,
                MovieId = movie.id  // Bu satırı ekledik, EF'nin bu ilişkiyi düzgün kurabilmesi için
            }).ToList();

            try
            {
                await _moviecontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new { message = "Film başarıyla güncellendi" });
        }

        [HttpPost("MovieIds")]
        public async Task<IActionResult> GetMovieIds([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("ID listesi boş olamaz.");
            }

            // Veritabanında ID listesi ile eşleşen filmleri al
            var movies = await _moviecontext.Movies
                .Where(movie => ids.Contains(movie.id))
                .ToListAsync();

            if (movies == null || !movies.Any())
            {
                return NotFound("Belirtilen ID'lerle eşleşen film bulunamadı.");
            }

            return Ok(movies);
        }




    }
}
