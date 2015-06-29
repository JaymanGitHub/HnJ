using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnJ.Helper.Models
{
    public class CinemaAmenity
    {
        public Guid Id { get; set; }
        public Guid CinemaId { get; set; }
        public Guid AmenityId { get; set; }

        public Lazy<Cinema> Cinema { get; set; }
        public Lazy<Amenity> Amenity { get; set; }
    }
}
