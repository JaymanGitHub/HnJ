using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace HnJ.Helper
{
    public class Bookings
    {
        #region Get

        public static Booking Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new Booking();

                return item;
            }
            catch
            {
                return new Booking();
            }
        }

        public static List<Booking> GetByUserId(Guid userId)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.UserId == userId).ToList();

                if (item == null)
                    return new List<Booking>();

                return item;
            }
            catch
            {
                return new List<Booking>();
            }
        }

        public static List<Booking> GetAll()
        {
            try
            {
                string data = Path.Booking;

                if (!File.Exists(Path.Booking))
                    return new List<Booking>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var bookings = new List<Booking>();
                bookings = (from rows in ds.Tables[0].AsEnumerable()
                            select new Booking
                            {
                                Id = Guid.Parse(rows[0].ToString()),
                                UserId = Guid.Parse(rows[1].ToString()),
                                MovieId = Guid.Parse(rows[2].ToString()),
                                CinemaId = Guid.Parse(rows[3].ToString()),
                                Price = Decimal.Parse(rows[4].ToString()),
                                BookingDate = DateTime.ParseExact(rows[5].ToString(), "dd/MM/yyyy", null),

                                User = new Lazy<User>(() =>
                                {
                                    return Users.Get(Guid.Parse(rows[1].ToString()));
                                }),

                                Movie = new Lazy<Movie>(() =>
                                {
                                    return Movies.Get(Guid.Parse(rows[2].ToString()));
                                }),

                                Cinema = new Lazy<Cinema>(() =>
                               {
                                   return Cinemas.Get(Guid.Parse(rows[3].ToString()));
                               })

                            }).ToList();

                if (bookings == null)
                    return new List<Booking>();

                return bookings;
            }
            catch
            {
                return new List<Booking>();
            }
        }

        #endregion

        #region Add

        public static void Add(Booking item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id))
                    return;

                items.Add(item);
                Add(items);

                var booking = Bookings.Get(item.Id);

                var email = HttpContext.Current.User.Identity.Name;

                var user = Users.Get(email);

                var emailBody = "Hello " + user.FirstName + ", Your Booking Details:<br /><br /><br />";
                emailBody += "Movie: " + booking.Movie.Value.Title + "<br />";
                emailBody += "Cinema: " + booking.Cinema.Value.Name + "<br />";
                emailBody += "Address: " + booking.Cinema.Value.Address + "<br />";
                emailBody += "Price: " + booking.Price.ToString("C") + "<br /><br />";
                emailBody += "Thank you for choosing HnJ Cinemas. We value our customers and we hope you enjoy the movie. <br />";
                emailBody += "Send your feedbacks or suggestions to HnJCinemas@gmail.com <br />";

                Utility.SendEmail(email, emailBody);

            }
            catch { }
        }

        public static void Add(List<Booking> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.Booking))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Bookings");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("Booking");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("UserId", item.UserId.ToString());
                        writer.WriteElementString("MovieId", item.MovieId.ToString());
                        writer.WriteElementString("CinemaId", item.CinemaId.ToString());
                        writer.WriteElementString("Price", (20M).ToString());
                        writer.WriteElementString("BookingDate", item.BookingDate.ToString("dd/MM/yyyy"));

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch { }
        }

        #endregion

        #region Update

        public static void Update(Booking item)
        {
            try
            {
                var items = GetAll();

                var oldItem = items.Where(p => p.Id == item.Id).FirstOrDefault();

                items.Remove(oldItem);
                items.Add(item);

                Add(items);
            }
            catch { }
        }

        #endregion

        #region Delete

        public static void Delete(Booking item)
        {
            try
            {
                var items = GetAll();
                var itemToDelete = items.Where(p => p.Id == item.Id).FirstOrDefault();
                items.Remove(itemToDelete);
                Add(items);
            }
            catch { }

        }

        public static void Delete(Guid id)
        {
            try
            {
                var items = GetAll();
                var itemToDelete = items.Where(p => p.Id == id).FirstOrDefault();
                Delete(itemToDelete);
            }
            catch { }
        }

        #endregion
    }
}
