using HnJ.Helper;
using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HnJ.Web.Controllers
{
    public class MovieController : Controller
    {
        // GET: Movie
        public ActionResult Index(Guid? id, Guid? genreId)
        {
            //var firstGenre = HnJ.Helper.Genres.GetAll().OrderBy(x => x.Name).FirstOrDefault();

            //var movies = Movies.GetByGenreId(firstGenre.Id);

            var movies = Movies.GetAll();

            if (genreId.HasValue && genreId.Value != Guid.Empty)
            {
                movies = Movies.GetByGenreId(genreId.Value);
            }

            return View(movies);
        }

        public ActionResult Search(string keyword)
        {
            var movies = Movies.GetAll();

            ViewBag.Query = "All";

            if (!string.IsNullOrEmpty(keyword))
            {
                movies = Movies.Search(keyword);

                ViewBag.Query = keyword;
            }

            return View(movies);
        }

        public ActionResult Create(Guid? id)
        {
            var item = new Movie();
            ViewBag.Mode = "Add New";
            ViewBag.IsEditMode = false;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                item = Helper.Movies.Get(id.Value);
                ViewBag.Mode = "Edit";
                ViewBag.IsEditMode = true;
            }

            return View(item);
        }

        [HttpPost]
        public ActionResult Create(HnJ.Helper.Models.Movie movie, HttpPostedFileBase poster)
        {
            if (ModelState.IsValid)
            {
                if (movie.Id != Guid.Empty) // Update
                {
                    if (poster != null && poster.ContentLength > 0)
                    {
                        string guid = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(poster.FileName);

                        var filePath = System.IO.Path.Combine(Server.MapPath("~/Content/Posters/"), guid);

                        poster.SaveAs(filePath);

                        movie.PosterName = guid;
                    }
                    else
                    {
                        var movieToUpdate = Movies.Get(movie.Id);

                        movie.PosterName = movieToUpdate.PosterName;
                    }

                    Movies.Update(movie);

                    return RedirectToAction("Create", new { @id = movie.Id });
                }
                else
                {
                    if (poster != null && poster.ContentLength > 0)
                    {
                        string guid = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(poster.FileName);

                        var filePath = System.IO.Path.Combine(Server.MapPath("~/Content/Posters/"), guid);

                        poster.SaveAs(filePath);

                        movie.Id = Guid.NewGuid();
                        movie.PosterName = guid;

                        Movies.Add(movie);

                        return RedirectToAction("Create", new { @id = movie.Id });
                    }
                }
            }

            return RedirectToAction("Create");
        }

        public ActionResult Detail(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            Movie movie = Movies.Get(id.Value);
            return View(movie);
        }
        [HttpPost]
        public ActionResult Booking(Guid movieId, DateTime bookingDate, Guid CinemaId)
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = Helper.Users.Get(User.Identity.Name).Id,
                MovieId = movieId,
                CinemaId = CinemaId,
                BookingDate = bookingDate
            };
            Bookings.Add(booking);

            return RedirectToAction("Index", "Booking");
        }

        [HttpPost]
        public ActionResult AddReview(string comment, int rating, Guid movieId)
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                Comment = comment,
                MovieId = movieId,
                UserId = Helper.Users.Get(User.Identity.Name).Id,
                DateCreated = DateTime.Now,
                Rating = rating
            };

            Reviews.Add(review);

            return RedirectToAction("Detail", new { @id = movieId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ManageGenres(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var genres = Genres.GetAll();

                var movieGenres = Genres.GetByMovieId(id.Value);

                var items = genres.Select(x => new CheckboxViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsChecked = movieGenres.Any(p => p.Id == x.Id && p.Name == x.Name)
                });

                ViewBag.Movie = Movies.Get(id.Value);
                ViewBag.MovieId = id.Value;

                return View(items);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ManageGenres(IEnumerable<CheckboxViewModel> checkboxes, Guid movieId)
        {
            var selectedItems = checkboxes.Where(x => x.IsChecked);
            var format = string.Join(",", selectedItems.Select(x => x.Name));

            GenreMovies.DeleteByMovieId(movieId);

            foreach (var item in selectedItems)
            {
                var movieGenre = new GenreMovie
                {
                    Id = Guid.NewGuid(),
                    MovieId = movieId,
                    GenreId = item.Id,
                };

                GenreMovies.Add(movieGenre);
            }

            return RedirectToAction("ManageGenres", new { @id = movieId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Helper.Movies.Delete(id.Value);
            }

            return RedirectToAction("Index");
        }
    }
}