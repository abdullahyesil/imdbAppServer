using System.Text.Json.Serialization;

namespace imdbApi.Model.Entity.Surveys
{
    public class Survey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
       
        public List<Option>? Options { get; set; }
    }
}
