using HnJ.Helper;
using HnJ.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HnJ.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoLogin()
        {
            string email = Request["email"].ToString();
            string password = Request["password"].ToString();

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = Users.Authenticate(email.Trim(), password.Trim());

                if (user.Id != Guid.Empty)
                {
                    FormsAuthentication.SetAuthCookie(user.Email, true);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewData["loginMessage"] = "Login Failed";
            return View("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoRegister()
        {
            TempData["unknownError"] = "";
            TempData["emailExistError"] = "";
            TempData["passwordError"] = "";

            string firstName = Request["firstName"].ToString();
            string lastName = Request["lastName"].ToString();
            string username = Request["username"].ToString();
            string email = Request["email"].ToString();
            string password = Request["password"].ToString();

            var savedUser = Users.Get(email);

            if (savedUser.Id != Guid.Empty)
            {
                TempData["emailExistError"] = "Email address or username already registered.";
                return RedirectToAction("Register");
            }

            if (password.Length < 6)
            {
                TempData["passwordError"] = "Password must have more than 6 characters.";
                return RedirectToAction("Register");
            }

            var user = new Helper.Models.User 
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Email = email,
                Password = password,
                Role = Role.Customer
            };

            Users.Add(user);

            TempData["success"] = "Registration success. Please login to access HnJ, Thank you.";

            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}