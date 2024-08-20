using imdbApi.Model.Entity;

namespace imdbApi.Model
{
    public class Movie
    {
        public int id { get; set; }
        public string movieName { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public DateTime releaseDate { get; set; }
        public string imageUrl { get; set; } = string.Empty;
        public double rate { get; set; }
        // Many-to-Many ilişkiyi yönetmek için MovieActors navigation property
        public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public int categoryId { get; set; }
            
    }
}
