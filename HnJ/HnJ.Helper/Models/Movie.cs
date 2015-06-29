using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnJ.Helper.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string PosterName { get; set; }
        public string Actors { get; set; }
        public string Directors { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Lazy<List<GenreMovie>> GenreMovies { get; set; }
        public Lazy<List<Review>> Reviews { get; set; }
    }
}
