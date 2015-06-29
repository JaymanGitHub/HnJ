using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnJ.Helper.Models
{
    public class GenreMovie
    {
        public Guid Id { get; set; }
        public Guid GenreId { get; set; }
        public Guid MovieId { get; set; }

        public Lazy<Genre> Genre { get; set; }
        public Lazy<Movie> Movie { get; set; }
    }
}
