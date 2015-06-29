using HnJ.Helper.Enums;
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
    public class Users
    {
        #region Get

        public static User Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new User();

                return item;
            }
            catch
            {
                return new User();
            }
        }

        public static User Get(string email)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Email == email).FirstOrDefault();

                if (item == null)
                    return new User();

                return item;
            }
            catch
            {
                return new User();
            }
        }

        public static User Authenticate(string email, string password)
        {
            var items = GetAll();
            var item = items.Where(p => p.Email == email && p.Password == password).FirstOrDefault();

            if (item == null)
                return new User();

            return item;
        }

        public static List<User> GetAll()
        {
            try
            {
                string data = Path.User;

                if (!File.Exists(Path.User))
                    return new List<User>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var users = new List<User>();
                users = (from rows in ds.Tables[0].AsEnumerable()
                         select new User
                         {
                             Id = Guid.Parse(rows[0].ToString()),

                             FirstName = rows[1].ToString(),
                             LastName = rows[2].ToString(),
                             Username = rows[3].ToString(),
                             Email = rows[4].ToString(),
                             Password = rows[5].ToString(),
                             Role = (Role)int.Parse(rows[6].ToString()),

                             Bookings = new Lazy<List<Booking>>(() =>
                             {
                                 return Bookings.GetByUserId(Guid.Parse(rows[0].ToString()));
                             })
                         }).ToList();

                if (users == null)
                    return new List<User>();

                return users;
            }
            catch
            {
                return new List<User>();
            }
        }

        #endregion

        #region Add

        public static void Add(User item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || p.Username == item.Username || p.Email == item.Email))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        public static void Add(List<User> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.User))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Users");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("User");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("FirstName", item.FirstName.ToString());
                        writer.WriteElementString("LastName", item.LastName.ToString());
                        writer.WriteElementString("Username", item.Username.ToString());
                        writer.WriteElementString("Email", item.Email.ToString());
                        writer.WriteElementString("Password", item.Password.ToString());
                        writer.WriteElementString("Role", ((int)item.Role).ToString());

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

        public static void Update(User item)
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

        public static void Delete(User item)
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
