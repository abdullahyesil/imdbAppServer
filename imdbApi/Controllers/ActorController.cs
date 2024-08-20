using imdbApi.DTO;
using imdbApi.Model;
using imdbApi.Model.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace imdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {

        readonly movieContext _movieContext;

        public ActorController(movieContext movieContext)
        {
            _movieContext = movieContext;
        }

        [HttpGet("getActors")]
        public async Task<IActionResult> GetActors([FromQuery] int? page, int? size, string? value)
        {
            var totalCount= 0;
            List<ActorDto> actors;
            if (!string.IsNullOrEmpty(value))
            {
                var normalizedValue = value.ToLower(); // Arama terimini küçük harfe çevirin

                actors = await _movieContext.Actors
                    .Where(i => i.Name.ToLower().Contains(normalizedValue)) // Veritabanı sorgusunda da küçük harfe çevirin
                    .Skip(page ?? 0)
                    .Take(size ?? 10) // Varsayılan olarak bir sayfa boyutu belirleyin
                    .Select(r => new ActorDto { Id = r.Id, Name = r.Name })
                    .ToListAsync();

                 totalCount = await _movieContext.Actors
                    .Where(i => i.Name.ToLower().Contains(normalizedValue)).CountAsync();


            }
            else
            {
                actors = await _movieContext.Actors
                    .Skip(page ?? 0)
                    .Take(size ?? 10) // Varsayılan olarak bir sayfa boyutu belirleyin
                    .Select(r => new ActorDto { Id = r.Id, Name = r.Name })
                    .ToListAsync();
                 totalCount = await _movieContext.Actors.CountAsync();
            }

          

            var result = new
            {
                TotalCount = totalCount,
                Actors = actors
            };

            return Ok(result);
        }


        [HttpPost("addActor")]
        public async Task<IActionResult> AddActor(ActorDto actorDto)
        {
            if (actorDto == null)
            {
                return BadRequest("Actor data is null.");
            }

            // Aynı isimde bir aktörün olup olmadığını kontrol edin
            var existingActor = await _movieContext.Actors
                .FirstOrDefaultAsync(a => a.Name == actorDto.Name);

            if (existingActor != null)
            {
                return Conflict(new { message = "An actor with the same name already exists." });
            }

            var actor = new Actors
            {
                Name = actorDto.Name
            };

            try
            {
                _movieContext.Actors.Add(actor);
                await _movieContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Hata yönetimi için loglama yapılabilir veya hata mesajı dönebilirsiniz
                return StatusCode(500,new { message = "An error occurred while saving the actor. Please try again." });
            }

            // Eklenen aktörün ID'si ile birlikte yanıt döneriz
            return CreatedAtAction(nameof(GetActors), new { id = actor.Id }, actorDto);
        }

        [HttpPut("updateActor")]
        public async Task<IActionResult> UpdateActor([FromBody] ActorDto actorDto)
        {
            if (actorDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var actor = await _movieContext.Actors.FindAsync(actorDto.Id);
            if (actor == null)
            {
                return NotFound("Actor not found.");
            }

            actor.Name = actorDto.Name;

            _movieContext.Actors.Update(actor);
            await _movieContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> deleteActor([FromRoute]int id)
        {
            var actor = await _movieContext.Actors.FirstOrDefaultAsync(i=> i.Id == id);

            if (actor != null)
            {
                 _movieContext.Actors.Remove(actor);
                try
                {
                  _movieContext.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }

                return Ok("Başarıyla silindi");
            }
            return NotFound("İd ile kullanıcı mevcut değil");

        }

    }
}
