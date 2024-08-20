namespace imdbApi.Model.Entity.Surveys
{
    public class Option
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string OptionText { get; set; }
        public int VoteCount { get; set; } = 0;

        public Survey Survey { get; set; }
    }
}
