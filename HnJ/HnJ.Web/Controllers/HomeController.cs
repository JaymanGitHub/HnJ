using HnJ.Helper;
using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HnJ.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var movies = Movies.GetAll();
            return View(movies);
        }
    }
}