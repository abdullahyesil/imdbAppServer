using imdbApi.DTO;
using imdbApi.Model;
using imdbApi.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        public MovieController(movieContext context)
        {
            _moviecontext = context;
        }

        [HttpGet]
        public async Task<IActionResult> getMovies()
        {
            try
            {
                var movies = await _moviecontext.Movies
                    .Select(m => new Movie
                    {
                        id = m.id,
                        movieName = m.movieName,
                        description = m.description,
                        MovieActors = m.MovieActors,
                        imageUrl = m.imageUrl,
                        releaseDate = m.releaseDate,
                        rate = m.rate,
                        categoryId = m.categoryId,
                    })
                    .ToListAsync();

                if (movies == null || movies.Count == 0)
                {
                    return NotFound(); // Eğer film bulunamazsa NotFound döndür
                }

                return Ok(movies); // Filmleri başarıyla bulursa OK (200) ve filmleri döndür
            }
            catch (Exception ex)
            {
                // Herhangi bir istisna durumunda buraya düşeriz
                // Loglama yapılabilir, uygun bir hata mesajı gönderilebilir
                return StatusCode(StatusCodes.Status500InternalServerError, "Veritabanından filmler alınırken bir hata oluştu.");
            }
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
                CategoryId = movie.categoryId,
                Actors = movie.MovieActors.Select(ma => new ActorDto
                {
                    Id = ma.Actor.Id,
                    Name = ma.Actor.Name
                }).ToList()
            };

            return Ok(movieDto);
        }


        [HttpPost]
        public async Task<IActionResult> AddMovie(MovieDto model)
        {
            // releaseDate'nin Kind'ini UTC olarak ayarlayalım, saat bilgisini sıfırlayalım
            model.releaseDate = DateTime.SpecifyKind(model.releaseDate.Date, DateTimeKind.Utc);

            // MovieDto'yu Movie entity'sine dönüştürelim
            var movieEntity = new Movie
            {
                movieName = model.MovieName,
                description = model.Description,
                releaseDate = model.releaseDate,
                imageUrl = model.ImageUrl,
                rate = model.Rate,
                categoryId = model.CategoryId,
                MovieActors = model.Actors.Select(ma => new MovieActor
                {
                    ActorId = ma.Id
                    // Burada MovieId ayarlanmıyor çünkü EF tarafından otomatik olarak yapılır
                }).ToList()
            };

            // Entity'i veritabanına ekleyelim
            _moviecontext.Movies.Add(movieEntity);
            await _moviecontext.SaveChangesAsync();

            // Yeni eklenen film için oluşturulan id'yi almak için entity'den alıyoruz
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
        public async Task<IActionResult> updateMovie(Movie entity, int id)
        {

            if (id != entity.id)
            { 
                return BadRequest("idler uyuşmuyor");
            }
            var movie = await _moviecontext.Movies.FirstOrDefaultAsync(i => i.id == id);
            if (movie == null) {
                return NotFound("Bulunamadı");
            }

            movie.movieName = entity.movieName;
            movie.description = entity.description;
            movie.imageUrl = entity.imageUrl;
            movie.releaseDate = DateTime.SpecifyKind(entity.releaseDate.Date, DateTimeKind.Utc);
            movie.rate = entity.rate;
            movie.categoryId = entity.categoryId;
            movie.MovieActors = entity.MovieActors;

            try
            {
                await _moviecontext.SaveChangesAsync();
            }
            catch(Exception ex){
                return BadRequest(ex.Message);
            }

            return Ok(new {message = "Film başarıyla güncelledi" });
            


        }
    


}
}
