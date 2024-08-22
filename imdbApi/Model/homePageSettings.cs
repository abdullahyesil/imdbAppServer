namespace imdbApi.Model
{
    public class homePageSettings
    {

        public  int Id { get; set; }

        public string? h1Name { get; set; }
        public bool surveys { get; set; }
        public int? surveyId { get; set; }
        public bool imdbAppStory { get; set; }
        public List<int>? imdbAppStoryId { get; set; }
        public bool carousel { get; set; }
        public List<int>? carouselId { get; set; }
        public string? carouselBaslik { get; set; }
    }
}
