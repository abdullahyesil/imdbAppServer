namespace imdbApi.DTO.SurveysDTO
{
    public class SurveyDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public List<OptionDTO>? Options { get; set; }
    }

}
