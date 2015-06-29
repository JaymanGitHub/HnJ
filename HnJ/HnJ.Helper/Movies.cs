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
    public class Movies
    {
        #region Get

        public static Movie Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new Movie();

                return item;
            }
            catch
            {
                return new Movie();
            }
        }

        public static List<Movie> GetByGenreId(Guid id)
        {
            try
            {
                var genreMovies = GenreMovies.GetAll().Where(x => x.GenreId == id).ToList();

                var items = new List<Movie>();

                foreach (var genreMovie in genreMovies)
                {
                    items.Add(genreMovie.Movie.Value);
                }

                if (items == null)
                    return new List<Movie>();

                return items;
            }
            catch
            {
                return new List<Movie>();
            }
        }

        public static List<Movie> Search(string query)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Title.ToLower().Trim().Contains(query.ToLower().Trim()) || p.Actors.ToLower().Contains(query.ToLower().Trim()) || p.Directors.ToLower().Contains(query.ToLower().Trim())).ToList();

                if (item == null)
                    return new List<Movie>();

                return item;
            }
            catch
            {
                return new List<Movie>();
            }
        }

        public static List<Movie> GetAll()
        {
            try
            {
                string data = Path.Movie;

                if (!File.Exists(Path.Movie))
                    return new List<Movie>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var movies = new List<Movie>();
                movies = (from rows in ds.Tables[0].AsEnumerable()
                          select new Movie
                          {
                              Id = Guid.Parse(rows[0].ToString()),
                              Title = rows[1].ToString(),
                              PosterName = rows[2].ToString(),
                              Actors = rows[3].ToString(),
                              Directors = rows[4].ToString(),
                              ReleaseDate = DateTime.ParseExact(rows[5].ToString(), "MM/dd/yyyy", null),

                              GenreMovies = new Lazy<List<GenreMovie>>(() =>
                              {
                                  return GenreMovies.GetByMovieId(Guid.Parse(rows[0].ToString()));
                              }),

                              Reviews = new Lazy<List<Review>>(() =>
                               {
                                   return Reviews.GetByMovieId(Guid.Parse(rows[0].ToString()));
                               })

                          }).ToList();

                if (movies == null)
                    return new List<Movie>();

                return movies;
            }
            catch
            {
                return new List<Movie>();
            }
        }

        #endregion

        #region Add

        public static void Add(Movie item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || p.Title == item.Title))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        public static void Add(List<Movie> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.Movie))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Movies");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("Movie");

                        var id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id;
                        var title = string.IsNullOrEmpty(item.Title) ? "" : item.Title;
                        var posterName = string.IsNullOrEmpty(item.PosterName) ? "" : item.PosterName;
                        var actors = string.IsNullOrEmpty(item.Actors) ? "" : item.Actors;
                        var directors = string.IsNullOrEmpty(item.Directors) ? "" : item.Directors;
                        var releaseDate = item.ReleaseDate == null ? DateTime.Now : item.ReleaseDate;

                        writer.WriteElementString("Id", id.ToString());
                        writer.WriteElementString("Title", title.ToString());
                        writer.WriteElementString("PosterName", posterName.ToString());
                        writer.WriteElementString("Actors", actors.ToString());
                        writer.WriteElementString("Directors", directors.ToString());
                        writer.WriteElementString("ReleaseDate", releaseDate.ToString("MM/dd/yyyy"));

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

        public static void Update(Movie item)
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

        public static void Delete(Movie item)
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
