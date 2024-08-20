namespace imdbApi.DTO
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string Description { get; set; }
        public DateTime releaseDate { get; set; }
        public string ImageUrl { get; set; }
        public double Rate { get; set; }
        public int CategoryId { get; set; }
        public List<ActorDto> Actors { get; set; } = new List<ActorDto>();
    }
}
