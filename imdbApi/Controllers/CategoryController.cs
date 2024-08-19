
using imdbApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace imdbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {


        private readonly categoryContext _categoryContext;
        private readonly movieContext _movieContext;
        public CategoryController(categoryContext categoryContext, movieContext movieContext)
        {


            _categoryContext = categoryContext;
            _movieContext = movieContext;
        }

        [HttpGet]
        public async Task<IActionResult> getCategories()
        {
            var categories = await _categoryContext.Categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();

            if (categories == null) {

                return BadRequest();
            }

            return Ok(categories);

        }


        [HttpGet("/api/Category/GetById/{categoryid}")]

        public async Task<IActionResult> getCategoryById(int categoryid)
        {

            var m = await _categoryContext.Categories.Where(i => i.Id == categoryid).Select(m =>
            new Category
            {
                Id = m.Id,
                Name = m.Name

            }).FirstOrDefaultAsync();

            if (m==null)
            {
                return NotFound();
            }

            return Ok(m);


        }

        [HttpGet("{categoryid}")]
        public async Task<IActionResult> getCategoriesById(int categoryid)
        {

            var m = await _movieContext.Movies.Where(i => i.categoryId == categoryid).Select(m =>
            new Movie
            {
                id = m.id,
                movieName = m.movieName,
                description = m.description,
                categoryId = m.categoryId,
                imageUrl = m.imageUrl,
                releaseDate = m.releaseDate,
                rate = m.rate
            }).ToListAsync();


            if (m == null) { return NotFound(); }
            return Ok(m);


        }

        [HttpPost("/api/Category/addCategory")]
        public async Task<IActionResult> addCategory(Category model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _categoryContext.Categories.Add(model);
                    await _categoryContext.SaveChangesAsync();

                    // Kategori başarıyla eklenmişse 201 Created cevabı döndürülür
                    return CreatedAtAction(nameof(getCategories), new { id = model.Id }, new { message = "Kategori başarıyla eklendi." });
                }
                catch (DbUpdateException ex)
                {
                    // Veritabanı işlemleri sırasında bir hata oluştuğunda
                    // DbUpdateException yakalanır ve hata mesajı ile birlikte 500 Internal Server Error döndürülür
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Veritabanı işlemleri sırasında bir hata oluştu.", error = ex.Message });
                }
                catch (Exception ex)
                {
                    // Diğer tüm hata durumlarında genel bir hata mesajı ve 500 Internal Server Error döndürülür
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Beklenmeyen bir hata oluştu.", error = ex.Message });
                }
            }

            return BadRequest();


        }

        [HttpPut("/api/Category/update/{id}")]
        public async Task<IActionResult> updateCategory(Category entity, int id)
        {
            if (id != entity.Id)
            { return BadRequest("idler uyuşmuyor"); }

            var category = await _categoryContext.Categories.FirstOrDefaultAsync(i => i.Id == id);
            if (category == null) { return NotFound("Bulunamadı"); }

            category.Id = entity.Id;
            category.Name = entity.Name;

            try
            {
                await _categoryContext.SaveChangesAsync();

            }
            catch (Exception err)
            {

                return BadRequest(err);
            }

            return Ok(new { message = "Kategori başarıyla güncelledi" });
        }

            [HttpDelete("/api/Category/deleteCategory/{id}")]
        public async Task<IActionResult> deleteCategory(int id)
        {
            if (id != null)
            {
                var dltModel = await _categoryContext.Categories.FirstOrDefaultAsync(i => i.Id == id);

                if (dltModel != null)
                {
                    try 
                    {   _categoryContext.Categories.Remove(dltModel);
                        await _categoryContext.SaveChangesAsync();
                    }
                    catch (Exception err)
                    {
                        return NotFound($"Error: {err}");
                    }
                    return Ok(new { message = "Kategori başarıyla silindi." });

                }
                return BadRequest("model bulunamadı");
            }

            return NotFound("İd eşleşmiyor");
        }


    }
}
