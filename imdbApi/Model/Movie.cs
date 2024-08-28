using imdbApi.Model.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace imdbApi.Model
{
    public class Movie
    {
        public int id { get; set; }
        public string movieName { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public DateTime releaseDate { get; set; }
        public string? imageUrl { get; set; } = string.Empty;
        public string? carouselImage { get; set; }
        public string trailer { get; set; }
        public double rate { get; set; }
        // Many-to-Many ilişkiyi yönetmek için MovieActors navigation property
        public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public int categoryId { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        [NotMapped]
        public IFormFile? CarouselImageFile { get; set; }

    }
}
