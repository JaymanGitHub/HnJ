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
    public class GenreMovies
    {
        #region Get

        public static GenreMovie Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new GenreMovie();

                return item;
            }
            catch
            {
                return new GenreMovie();
            }
        }

        public static List<GenreMovie> GetByMovieId(Guid movieId)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.MovieId == movieId).ToList();

                if (item == null)
                    return new List<GenreMovie>();

                return item;
            }
            catch
            {
                return new List<GenreMovie>();
            }
        }

        public static List<GenreMovie> GetAll()
        {
            try
            {
                string data = Path.GenreMovie;

                if (!File.Exists(Path.GenreMovie))
                    return new List<GenreMovie>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var genreMovies = new List<GenreMovie>();
                genreMovies = (from rows in ds.Tables[0].AsEnumerable()
                               select new GenreMovie
                               {
                                   Id = Guid.Parse(rows[0].ToString()),
                                   GenreId = Guid.Parse(rows[1].ToString()),
                                   MovieId = Guid.Parse(rows[2].ToString()),

                                   Genre = new Lazy<Genre>(() =>
                                   {
                                       return Genres.Get(Guid.Parse(rows[1].ToString()));
                                   }),

                                   Movie = new Lazy<Movie>(() =>
                                   {
                                       return Movies.Get(Guid.Parse(rows[2].ToString()));
                                   }),
                               }).ToList();

                if (genreMovies == null)
                    return new List<GenreMovie>();

                return genreMovies;
            }
            catch
            {
                return new List<GenreMovie>();
            }
        }

        #endregion

        #region Add

        public static void Add(GenreMovie item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || (p.GenreId == item.GenreId && p.MovieId == item.MovieId)))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        public static void Add(List<GenreMovie> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.GenreMovie))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("GenreMovies");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("GenreMovie");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("GenreId", item.GenreId.ToString());
                        writer.WriteElementString("MovieId", item.MovieId.ToString());

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

        public static void Update(GenreMovie item)
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

        public static void Delete(GenreMovie item)
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

        public static void DeleteByMovieId(Guid id)
        {
            try
            {
                var items = GetAll();
                var itemsToDelete = items.Where(p => p.MovieId == id);

                foreach (var itemToDelete in itemsToDelete)
                {
                    Delete(itemToDelete);
                }
            }
            catch { }
        }

        #endregion
    }
}
