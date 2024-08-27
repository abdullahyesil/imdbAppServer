using System.ComponentModel.DataAnnotations.Schema;

namespace imdbApi.Model.Entity
{
    public class Actors
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string imageUrl { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        // Many-to-Many ilişkiyi yönetmek için MovieActors navigation property
        public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
