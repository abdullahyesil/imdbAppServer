using imdbApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace imdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {


        private readonly rateContext _rateContext;
        private readonly movieContext _movieContext;

        public RateController(rateContext rateContext, movieContext movieContext)
        {
            _rateContext = rateContext;
            _movieContext = movieContext;
        }

        [HttpGet]
        public async Task<IActionResult> getRating()
        {
            var getRating =  await _rateContext.Rate.Select(c => new Rate
            {
                rate_id = c.rate_id,
                movie_id = c.movie_id,
                user_id = c.user_id,
                rate = c.rate
            }).ToListAsync();

            if (getRating == null) { return NotFound(new { message = "bulunamadı" }); };

            return Ok(getRating);
        }

        [HttpPost("useVote")]
        public async Task<IActionResult> addRate(Rate rate) { 
           
            _rateContext.Rate.Add(rate); //Oyumuzu ekledik...
            await _rateContext.SaveChangesAsync(); //Databaseyi kayıt ettik.

                //Geçmiş oyları getirdik. 
            var getRating = await _rateContext.Rate.Where(i => rate.movie_id == i.movie_id).Select(c => new Rate
                {
                    rate_id = c.rate_id,
                    movie_id = c.movie_id,
                    user_id = c.user_id,
                    rate = c.rate
                }).ToListAsync();


            //Ortalama işlemleri
            var rating = 0;
            for (int i = 0; i < getRating.Count; i++)
            {

                rating = rating+ getRating[i].rate;
                
            }

            rating = rating/getRating.Count;


            var movie = await _movieContext.Movies.FirstOrDefaultAsync(i => i.id == rate.movie_id);
            if (movie == null)
            {
                return NotFound("Bulunamadı");  
            }
            movie.rate=rating;
      

            try
            {
                await _movieContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return Ok(new { message = "Tebrikler oyunuz başarıyla kullanıldı." });
        }





    }
}
