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
    public class Genres
    {
        #region Get

        public static Genre Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new Genre();

                return item;
            }
            catch
            {
                return new Genre();
            }
        }

        public static List<Genre> GetByMovieId(Guid cinemaId)
        {
            var genreMovies = GenreMovies.GetByMovieId(cinemaId);

            var genres = genreMovies.Select(p => p.Genre.Value).ToList();

            return genres;
        }

        public static List<Genre> GetAll()
        {
            try
            {
                string data = Path.Genre;

                if (!File.Exists(Path.Genre))
                    return new List<Genre>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var genres = new List<Genre>();
                genres = (from rows in ds.Tables[0].AsEnumerable()
                          select new Genre
                          {
                              Id = Guid.Parse(rows[0].ToString()),
                              Name = rows[1].ToString(),
                          }).ToList();

                if (genres == null)
                    return new List<Genre>();

                return genres.OrderBy(x => x.Name).ToList();
            }
            catch
            {
                return new List<Genre>();
            }
        }

        #endregion

        #region Add

        public static void Add(Genre item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || p.Name == item.Name))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        public static void Add(List<Genre> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.Genre))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Genres");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("Genre");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("Name", item.Name.ToString());

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

        public static void Update(Genre item)
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

        public static void Delete(Genre item)
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
