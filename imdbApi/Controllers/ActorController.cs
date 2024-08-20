﻿using imdbApi.DTO;
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
            }
            else
            {
                actors = await _movieContext.Actors
                    .Skip(page ?? 0)
                    .Take(size ?? 10) // Varsayılan olarak bir sayfa boyutu belirleyin
                    .Select(r => new ActorDto { Id = r.Id, Name = r.Name })
                    .ToListAsync();
            }

            var totalCount = await _movieContext.Actors.CountAsync();

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
                return Conflict("An actor with the same name already exists.");
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
                return StatusCode(500, "An error occurred while saving the actor. Please try again.");
            }

            // Eklenen aktörün ID'si ile birlikte yanıt döneriz
            return CreatedAtAction(nameof(GetActors), new { id = actor.Id }, actorDto);
        }

        [HttpPut("updateActor/{id}")]
        public async Task<IActionResult> UpdateActor(int id, [FromBody] ActorDto actorDto)
        {
            if (actorDto == null || id != actorDto.Id)
            {
                return BadRequest("Invalid data.");
            }

            var actor = await _movieContext.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound("Actor not found.");
            }

            actor.Name = actorDto.Name;

            _movieContext.Actors.Update(actor);
            await _movieContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
