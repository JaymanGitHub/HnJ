using HnJ.Helper;
using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HnJ.Web.Controllers
{
    public class BookingController : Controller
    {
        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Index()
        {
            var bookings = Helper.Bookings.GetAll();

            if (User.IsInRole("Customer"))
            {
                var user = Helper.Users.Get(User.Identity.Name);

                if (user.Id == Guid.Empty)
                {
                    return RedirectToAction("Index", "Home");
                }

                bookings = Helper.Bookings.GetByUserId(user.Id);
            }

            return View(bookings);
        }

        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Manage(Guid? id)
        {
            var item = new Booking();
            ViewBag.Mode = "Add New";
            ViewBag.IsEditMode = false;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                item = Helper.Bookings.Get(id.Value);
                ViewBag.Mode = "Edit";
                ViewBag.IsEditMode = true;
            }

            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Manage(Booking item)
        {
            if (item.Id != Guid.Empty)
            {
                Helper.Bookings.Update(item);
            }
            else
            {
                var user = Users.Get(User.Identity.Name);

                item.Id = Guid.NewGuid();
                item.UserId = user.Id;
                Helper.Bookings.Add(item);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                Helper.Bookings.Delete(id.Value);
            }

            return RedirectToAction("Index");
        }
    }
}