using imdbApi.Model;
using imdbApi.Model.Entity.Surveys;
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
        [HttpGet("Settings")]
        public async Task<IActionResult> get()
        {
            var model = await _settingsContext.imdbSettings.Select(r => new imdbSettings
            { 
                Id = r.Id,
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

        [HttpPut("Settings")]
        public async Task<IActionResult> settingsPut(imdbSettings imdbSettings)
        {
        var model = await _settingsContext.imdbSettings.FirstOrDefaultAsync(i=> i.Id == imdbSettings.Id);

            if (model != null)
            {
                model.Name = imdbSettings.Name;
                model.Author = imdbSettings.Author;
                model.Description = imdbSettings.Description;
                model.Meta= imdbSettings.Meta;
                model.Instagram = imdbSettings.Instagram;
                model.Facebook = imdbSettings.Facebook;
                model.Youtube = imdbSettings.Youtube;
                model.Tiktok = imdbSettings.Tiktok;
                model.Twitter = imdbSettings.Twitter;

                try
                {
                    await _settingsContext.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception err)
                {

                    throw err;
                }
            }

       
            else { return BadRequest("Geçersiz istek");  }
            
        }
        [HttpGet("Home")]
        public async Task<IActionResult> getHomePage()
        {
            return Ok();
        }
        [HttpPut("Home")]
        public async Task<IActionResult> PutHomePage()
        {
            return Ok();
        }


        //Survey
        [HttpGet("Survey/{id}")]
        public async Task<ActionResult<Survey>> GetSurvey(int id)
        {
            var survey = await _settingsContext.Surveys.Include(s => s.Options).FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        [HttpPost("Survey/")]
        public async Task<ActionResult<Survey>> CreateSurvey(Survey survey)
        {
            _settingsContext.Surveys.Add(survey);
            await _settingsContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
        }

        [HttpPost("Survey/vote/{optionId}")]
        public async Task<IActionResult> Vote(int optionId)
        {
            var option = await _settingsContext.Options.FindAsync(optionId);
            if (option == null)
            {
                return NotFound();
            }

            option.VoteCount++;
            await _settingsContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Survey/{id}/results")]
        public async Task<ActionResult> GetSurveyResults(int id)
        {
            var survey = await _settingsContext.Surveys.Include(s => s.Options).FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            var totalVotes = survey.Options.Sum(o => o.VoteCount);
            var results = survey.Options.Select(o => new
            {
                o.OptionText,
                o.VoteCount,
                VotePercentage = totalVotes == 0 ? 0 : (o.VoteCount * 100.0 / totalVotes)
            });

            return Ok(results);
        }

        //Survey bİTİŞ
    }


}
