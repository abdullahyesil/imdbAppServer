using imdbApi.DTO.SurveysDTO;
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
            var model = await _settingsContext.imdbSettings.FirstOrDefaultAsync(i => i.Id == imdbSettings.Id);

            if (model != null)
            {
                model.Name = imdbSettings.Name;
                model.Author = imdbSettings.Author;
                model.Description = imdbSettings.Description;
                model.Meta = imdbSettings.Meta;
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


            else { return BadRequest("Geçersiz istek"); }

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

        //Surveys

        [HttpGet("Survey")]
        public async Task<ActionResult<List<SurveyDTO>>> GetSurveys()
        {
            var surveys = await _settingsContext.Surveys
                .OrderByDescending(s => s.CreatedDate)  // En son eklenen anketler önce gelecek şekilde sıralama
                .ToListAsync();

            var surveyDTOs = surveys.Select(s => new SurveyDTO
            {
                Id = s.Id,
                Title = s.Title,
                CreatedDate = s.CreatedDate
            }).ToList();

            return Ok(surveyDTOs);
        }

        [HttpGet("Survey/{id}")]
        public async Task<ActionResult<SurveyDTO>> GetSurvey(int id)
        {
            var survey = await _settingsContext.Surveys
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            var surveyDto = new SurveyDTO
            {
                Id = survey.Id,
                Title = survey.Title,
                CreatedDate = survey.CreatedDate,
                Options = survey.Options.Select(o => new OptionDTO
                {
                    Id = o.Id,
                    SurveyId = o.SurveyId,
                    OptionText = o.OptionText,
                    VoteCount = o.VoteCount
                }).ToList()
            };

            return Ok(surveyDto);
        }


        [HttpPost("Survey/")]
        public async Task<ActionResult<SurveyDTO>> CreateSurvey(SurveyDTO surveyDto)
        {
            var survey = new Survey
            {
                Title = surveyDto.Title,
                CreatedDate = surveyDto.CreatedDate,
                Options = surveyDto.Options.Select(o => new Option
                {
                    SurveyId = o.SurveyId,
                    OptionText = o.OptionText,
                    VoteCount = o.VoteCount
                }).ToList()
            };

            _settingsContext.Surveys.Add(survey);
            await _settingsContext.SaveChangesAsync();

            // Survey oluşturulduktan sonra DTO'yu geri döndürmek için survey nesnesini surveyDto'ya dönüştürüyoruz
            surveyDto.Id = survey.Id;
            surveyDto.Options.ForEach(o => o.Id = survey.Options.First(opt => opt.OptionText == o.OptionText).Id);

            return CreatedAtAction(nameof(GetSurvey), new { id = surveyDto.Id }, surveyDto);
        }


        [HttpPost("Survey/vote/")]
        public async Task<IActionResult> Vote([FromBody]int optionId)
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
        public async Task<ActionResult<IEnumerable<OptionResultDTO>>> GetSurveyResults(int id)
        {
            var survey = await _settingsContext.Surveys
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            var totalVotes = survey.Options.Sum(o => o.VoteCount);

            var results = survey.Options.Select(o => new OptionResultDTO
            {
                OptionText = o.OptionText,
                VoteCount = o.VoteCount,
                VotePercentage = totalVotes == 0 ? 0 : (o.VoteCount * 100.0 / totalVotes)
            }).ToList();

            return Ok(results);
        }
        //Survey bİTİŞ


        //homePages

        [HttpPut("HomeSettings")]
        public async Task<IActionResult> putSettings(homePageSettings settings)
        {
            var model = await _settingsContext.homePageSettings.FirstOrDefaultAsync(s => s.Id == settings.Id);
                if(model != null)
           {
                model.h1Name = settings.h1Name;
                model.imdbAppStory = settings.imdbAppStory;
                model.imdbAppStoryId = settings.imdbAppStoryId;
                model.surveys = settings.surveys;
                model.surveyId = settings.surveyId;
                model.carousel = settings.carousel;
                model.carouselId = settings.carouselId;
                model.carouselBaslik = settings.carouselBaslik;

                try
                {
                    await _settingsContext.SaveChangesAsync();
                    return Ok(model);
                }
                catch (Exception)
                {

                    throw;
                }
            }

                return BadRequest("Bulunamadı");

        }

        [HttpGet("HomeSettings")]
        public async Task<IActionResult> getHomeSettings()
        {
            var model = await _settingsContext.homePageSettings.FirstOrDefaultAsync();
            return Ok(model);

        }


        //homeSettingsBitiş

        //Story

        [HttpPost("Story")]
        public async Task<IActionResult> addStory(ImdbAppStory model)
        {
            try
            {
                await _settingsContext.imdbAppStory.AddAsync(model);
                _settingsContext.SaveChanges();
                return Created();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("Story")]
        public async Task<IActionResult> getStory()
        {
            var model = await _settingsContext.imdbAppStory.ToListAsync();
            return Ok(model);
        }

        [HttpPost("StoryIds")]
        public async Task<IActionResult> GetStoryIds([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("ID listesi boş olamaz.");
            }

            // Veritabanında ID listesi ile eşleşen Story'leri al
            var stories = await _settingsContext.imdbAppStory
                .Where(story => ids.Contains(story.Id))
                .ToListAsync();

            if (stories == null || !stories.Any())
            {
                return NotFound("Belirtilen ID'lerle eşleşen hikaye bulunamadı.");
            }

            return Ok(stories);
        }


    }


}
