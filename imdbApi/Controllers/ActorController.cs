using imdbApi.DTO;
using imdbApi.Model;
using imdbApi.Model.Entity;
using imdbApi.Services.Abrastract;
using imdbApi.Services.Implementation;
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
        readonly IFileService _fileService;

        public ActorController(movieContext movieContext, IFileService fileService)
        {
            _movieContext = movieContext;
            _fileService = fileService;
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
        public async Task<IActionResult> AddActor([FromForm]ActorDto actorDto)
        {

            if (actorDto == null)
            {
                return BadRequest("Actor data is null.");
            }

            if (actorDto.File != null)
            {
                var fileResult = _fileService.SaveImage(actorDto.File, 1600, 900);
                if (fileResult.Item1 == 1 && !string.IsNullOrEmpty(fileResult.Item2))
                {
                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
            

                    // Tam URL'yi oluşturuyoruz
                    actorDto.imageUrl = Path.Combine(baseUrl, "res", fileResult.Item2).Replace("\\", "/");
                 
                }
            }

            // Aynı isimde bir aktörün olup olmadığını kontrol edin
            var existingActor = await _movieContext.Actors
                .FirstOrDefaultAsync(a => a.Name == actorDto.Name);

            if (existingActor != null)
            {
                return Conflict(new { message = "Zaten bu isimle bir oyuncu var." });
            }

            var actor = new Actors
            {
                Name = actorDto.Name,
                imageUrl = actorDto.imageUrl,
            };

            try
            {
                _movieContext.Actors.Add(actor);
                await _movieContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Hata yönetimi için loglama yapılabilir veya hata mesajı dönebilirsiniz
                return StatusCode(500,new { message = "Oyuncu kaydedilirken bir hata oluştu. Lütfen tekrar deneyin." });
            }

            // Eklenen aktörün ID'si ile birlikte yanıt döneriz
            return CreatedAtAction(nameof(GetActors), new { id = actor.Id }, actorDto);
        }

        [HttpPut("updateActor")]
        public async Task<IActionResult> UpdateActor([FromForm] ActorDto actorDto)
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

            // Önceki resmin yolunu al
            var existingImageUrl = actor.imageUrl;

            if (actorDto.File != null)
            {
                // Yeni resmi kaydet
                var fileResult = _fileService.SaveImage(actorDto.File, 1600, 900);
                if (fileResult.Item1 == 1 && !string.IsNullOrEmpty(fileResult.Item2))
                {
                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";

                    // Yeni resmi tam URL ile kaydet
                    actor.imageUrl = Path.Combine(baseUrl, "res", fileResult.Item2).Replace("\\", "/");

                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(existingImageUrl))
                    {
                        // Tam URL'den dosya adını çıkarıyoruz
                        var oldImageFileName = Path.GetFileName(existingImageUrl);

                        // Dosya adını silme metoduna gönderiyoruz
                        _fileService.DeleteImage(oldImageFileName);

                    }
                }
                else
                {
                    return BadRequest("Yeni resim yüklenemedi.");
                }
            }

            // Aktörün diğer bilgilerini güncelle
            actor.Name = actorDto.Name;

            _movieContext.Actors.Update(actor);
            await _movieContext.SaveChangesAsync();

            return NoContent();
        }



        [HttpDelete("delete/{id}")]
        public IActionResult deleteActor([FromRoute]int id)
        {
            var actor =  _movieContext.Actors.FirstOrDefault(i=> i.Id == id);

            if (actor == null)
            {
                return NotFound("İd ile kullanıcı mevcut değil");
            }


            _movieContext.Actors.Remove(actor);
            try
            {
                _movieContext.SaveChanges();

                var response = new ResponseMessage()
                {
                    isSucceed = true,
                    Message = "Başarıyla silindi."
                };
                return Ok(response);
            }
            catch (Exception)
            {

                throw;
            }

            

        }

    }
}
