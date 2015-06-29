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
    public class Cinemas
    {
        #region Get

        public static Cinema Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new Cinema();

                return item;
            }
            catch
            {
                return new Cinema();
            }
        }

        public static List<Cinema> GetAll()
        {
            try
            {
                string data = Path.Cinema;

                if (!File.Exists(Path.Cinema))
                    return new List<Cinema>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var cinemas = new List<Cinema>();
                cinemas = (from rows in ds.Tables[0].AsEnumerable()
                           select new Cinema
                           {
                               Id = Guid.Parse(rows[0].ToString()),
                               Name = rows[1].ToString(),
                               Latitude = Convert.ToDecimal(rows[2].ToString()),
                               Longitude = Convert.ToDecimal(rows[3].ToString()),
                               Address = rows[4].ToString(),
                               
                               CinemaAmenities = new Lazy<List<CinemaAmenity>>(() =>
                               {
                                   return CinemaAmenities.GetByCinemaId(Guid.Parse(rows[0].ToString()));
                               })

                           }).ToList();

                if (cinemas == null)
                    return new List<Cinema>();

                return cinemas;
            }
            catch
            {
                return new List<Cinema>();
            }
        }

        #endregion

        #region Add

        public static void Add(Cinema item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || (p.Name == item.Name && p.Address == item.Address)))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        public static void Add(List<Cinema> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.Cinema))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Cinemas");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("Cinema");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("Name", item.Name.ToString());
                        writer.WriteElementString("Latitude", item.Latitude.ToString());
                        writer.WriteElementString("Longitude", item.Longitude.ToString());
                        writer.WriteElementString("Address", item.Address.ToString());

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

        public static void Update(Cinema item)
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

        public static void Delete(Cinema item)
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
