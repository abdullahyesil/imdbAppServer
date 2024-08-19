using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace imdbApi.Model
{
    public class Rate
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rate_id { get; set; }
        public int movie_id { get; set; }
        public int user_id { get; set; }
        public int rate { get; set; }
    }
}
