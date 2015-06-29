using HnJ.Helper;
using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HnJ.Web.Controllers
{
    public class CinemaController : Controller
    {
        public ActionResult Index()
        {
            var cinemas = Helper.Cinemas.GetAll();
            return View(cinemas);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Manage(Guid? id)
        {
            var item = new Cinema();
            ViewBag.Mode = "Add New";
            ViewBag.IsEditMode = false;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                item = Helper.Cinemas.Get(id.Value);
                ViewBag.Mode = "Edit";
                ViewBag.IsEditMode = true;
            }

            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Manage(Cinema item, string[] amenities)
        {
            if (item.Id != Guid.Empty)
            {
                Helper.Cinemas.Update(item);
            }
            else
            {
                item.Id = Guid.NewGuid();
                Helper.Cinemas.Add(item);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ManageAmenities(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var amenities = Amenities.GetAll();

                var cinemaAmenities = Amenities.GetByCinemaId(id.Value);

                var items = amenities.Select(x => new CheckboxViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsChecked =  cinemaAmenities.Any(p => p.Id == x.Id && p.Name == x.Name)
                });

                ViewBag.Cinema = Cinemas.Get(id.Value);
                ViewBag.CinemaId = id.Value;

                return View(items);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ManageAmenities(IEnumerable<CheckboxViewModel> checkboxes, Guid cinemaId)
        {
            var selectedItems = checkboxes.Where(x => x.IsChecked);
            var format = string.Join(",", selectedItems.Select(x => x.Name));

            CinemaAmenities.DeleteByCinemaId(cinemaId);

            foreach (var item in selectedItems)
            {
                var cinemaAmenity = new CinemaAmenity
                {
                    Id = Guid.NewGuid(),
                    CinemaId = cinemaId,
                    AmenityId = item.Id,
                };

                CinemaAmenities.Add(cinemaAmenity);
            }

            return RedirectToAction("ManageAmenities", new { @id = cinemaId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Helper.Cinemas.Delete(id.Value);
            }

            return RedirectToAction("Index");
        }
    }
}