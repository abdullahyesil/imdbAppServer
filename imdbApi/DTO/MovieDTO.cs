using System.ComponentModel.DataAnnotations.Schema;

namespace imdbApi.DTO
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string Description { get; set; }
        public DateTime releaseDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? CarouselImage { get; set; }
        public double Rate { get; set; }
        public string Trailer { get; set; }
        public int CategoryId { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public List<ActorDto> Actors { get; set; } = new List<ActorDto>();
        [NotMapped]
        public IFormFile? CarouselImageFile { get; set; }
    }
}
