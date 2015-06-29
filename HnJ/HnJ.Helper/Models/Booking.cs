using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnJ.Helper.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }
        public Guid CinemaId { get; set; }
        public decimal Price { get; set; }
        public DateTime BookingDate { get; set; }

        public Lazy<User> User { get; set; }
        public Lazy<Movie> Movie { get; set; }
        public Lazy<Cinema> Cinema { get; set; }
    }
}
