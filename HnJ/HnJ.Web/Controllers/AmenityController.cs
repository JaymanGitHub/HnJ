using HnJ.Helper;
using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HnJ.Web.Controllers
{
    public class AmenityController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var items = Amenities.GetAll();
            return View(items);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Manage(Guid? id)
        {
            var item = new Amenity();
            ViewBag.Mode = "Add New";
            ViewBag.IsEditMode = false;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                item = Helper.Amenities.Get(id.Value);
                ViewBag.Mode = "Edit";
                ViewBag.IsEditMode = true;
            }

            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Manage(Amenity item)
        {
            if (item.Id != Guid.Empty)
            {
                Helper.Amenities.Update(item);
            }
            else
            {
                item.Id = Guid.NewGuid();
                Helper.Amenities.Add(item);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Helper.Amenities.Delete(id.Value);
            }

            return RedirectToAction("Index");
        }
    }
}