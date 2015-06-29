using HnJ.Helper;
using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HnJ.Web.Controllers
{
    public class GenreController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var items = Genres.GetAll();
            return View(items);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Manage(Guid? id)
        {
            var item = new Genre();
            ViewBag.Mode = "Add New";
            ViewBag.IsEditMode = false;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                item = Helper.Genres.Get(id.Value);
                ViewBag.Mode = "Edit";
                ViewBag.IsEditMode = true;
            }

            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Manage(Genre item)
        {
            if (item.Id != Guid.Empty)
            {
                Helper.Genres.Update(item);
            }
            else
            {
                item.Id = Guid.NewGuid();
                Helper.Genres.Add(item);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Helper.Genres.Delete(id.Value);
            }

            return RedirectToAction("Index");
        }
    }
}