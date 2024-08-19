using imdbApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace imdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        readonly movieContext _settingsContext;

        public AdminController(movieContext settingsContext)
        {
            _settingsContext = settingsContext;
        }
        [HttpGet]
        public async Task<IActionResult> get()
        {
            var model = await _settingsContext.imdbSettings.Select(r => new imdbSettings
            {
              
                Name = r.Name,
                Meta = r.Meta,
                Author = r.Author,
                Description = r.Description,
                Facebook = r.Facebook,
                Instagram = r.Instagram,
                Tiktok = r.Tiktok,
                Twitter = r.Twitter,
                Youtube = r.Youtube
            }).FirstOrDefaultAsync();

            return Ok(model);
            
        }
    }
}
