using HnJ.Helper;
using HnJ.Helper.Enums;
using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HnJ.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #region Making sure ther is a default Admin available

            var adminEmail = "admin@hnj.com";

            var adminUser = Users.Get(adminEmail);

            if (adminUser.Id == Guid.Empty)
            {
                adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "Admin",
                    Username = "admin",
                    Email = adminEmail,
                    Password = "123456",
                    Role = Role.Admin
                };

                Helper.Users.Add(adminUser);
            }

            #endregion
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context.Request.IsAuthenticated)
            {
                string[] roles = LookupRolesForUser(context.User.Identity.Name);
                var newUser = new GenericPrincipal(context.User.Identity, roles);
                context.User = Thread.CurrentPrincipal = newUser;
            }
        }

        private string[] LookupRolesForUser(string email)
        {
            string[] roles = new string[1];

            var user = Users.Get(email);
            if (user != null)
            {
                roles[0] = Enum.GetName(typeof(Role), user.Role);
                return roles;
            }

            return new string[0];  // Alternatively throw an exception
        }
    }
}
