using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HnJ.Helper
{
    public class Path
    {
        private static string basePath = HttpContext.Current.Server.MapPath("/XML/");

        public static string Amenity = basePath + "Amenity.xml";
        public static string Booking = basePath + "Booking.xml";
        public static string Cinema = basePath + "Cinema.xml";
        public static string CinemaAmenity = basePath + "CinemaAmenity.xml";
        public static string Genre = basePath + "Genre.xml";
        public static string GenreMovie = basePath + "GenreMovie.xml";
        public static string Movie = basePath + "Movie.xml";
        public static string Review = basePath + "Review.xml";
        public static string User = basePath + "User.xml";
    }
}
