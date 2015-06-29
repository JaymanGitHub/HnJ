using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HnJ.Helper
{
    public class Reviews
    {
        #region Get

        public static Review Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new Review();

                return item;
            }
            catch
            {
                return new Review();
            }
        }

        public static List<Review> GetByMovieId(Guid movieId)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.MovieId == movieId).ToList();

                if (item == null)
                    return new List<Review>();

                return item;
            }
            catch
            {
                return new List<Review>();
            }
        }

        public static List<Review> GetAll()
        {
            try
            {
                string data = Path.Review;

                if (!File.Exists(Path.Review))
                    return new List<Review>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var reviews = new List<Review>();
                reviews = (from rows in ds.Tables[0].AsEnumerable()
                           select new Review
                           {
                               Id = Guid.Parse(rows[0].ToString()),

                               UserId = Guid.Parse(rows[1].ToString()),
                               MovieId = Guid.Parse(rows[2].ToString()),
                               Rating = int.Parse(rows[3].ToString()),
                               Comment = rows[4].ToString(),
                               DateCreated = DateTime.ParseExact(rows[5].ToString(), "dd/MM/yyyy", null),

                               User = new Lazy<User>(() =>
                               {
                                   return Users.Get(Guid.Parse(rows[1].ToString()));
                               }),

                               Movie = new Lazy<Movie>(() =>
                               {
                                   return Movies.Get(Guid.Parse(rows[2].ToString()));
                               }),
                           }).ToList();

                if (reviews == null)
                    return new List<Review>();

                return reviews.OrderByDescending(x => x.DateCreated).ToList();
            }
            catch
            {
                return new List<Review>();
            }
        }

        #endregion

        #region Add

        public static void Add(Review item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || (p.UserId == item.UserId && p.Comment == item.Comment)))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        public static void Add(List<Review> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.Review))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Reviews");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("Review");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("UserId", item.UserId.ToString());
                        writer.WriteElementString("MovieId", item.MovieId.ToString());
                        writer.WriteElementString("Rating", item.Rating.ToString());
                        writer.WriteElementString("Comment", item.Comment.ToString());
                        writer.WriteElementString("DateCreated", item.DateCreated.ToString("dd/MM/yyyy"));

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

        public static void Update(Review item)
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

        public static void Delete(Review item)
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
