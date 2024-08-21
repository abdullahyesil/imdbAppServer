namespace imdbApi.DTO.SurveysDTO
{
    public class OptionDTO
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string? OptionText { get; set; }
        public int VoteCount { get; set; } = 0;
    }

}
