using System.ComponentModel.DataAnnotations.Schema;

namespace imdbApi.DTO
{
    public class ActorDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? imageUrl { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
