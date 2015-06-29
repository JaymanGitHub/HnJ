using HnJ.Helper.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

namespace HnJ.Helper
{
    public class Amenities
    {
        #region Get

        public static Amenity Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new Amenity();

                return item;
            }
            catch
            {
                return new Amenity();
            }
        }

        public static List<Amenity> GetByCinemaId(Guid cinemaId)
        {
            var cinemaAmenities = CinemaAmenities.GetByCinemaId(cinemaId);

            var amenities = cinemaAmenities.Select(p => p.Amenity.Value).ToList();

            return amenities;
        }

        public static List<Amenity> GetAll()
        {
            try
            {
                string data = Path.Amenity;

                if (!File.Exists(Path.Amenity))
                    return new List<Amenity>();

                DataSet ds = new DataSet();

                ds.ReadXml(data);

                var amenities = new List<Amenity>();

                amenities = (from rows in ds.Tables[0].AsEnumerable()
                             select new Amenity
                             {
                                 Id = Guid.Parse(rows[0].ToString()),
                                 Name = rows[1].ToString(),
                             }).ToList();

                if (amenities == null)
                    return new List<Amenity>();

                return amenities;
            }
            catch
            {
                return new List<Amenity>();
            }
        }

        #endregion

        #region Add

        public static void Add(Amenity item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || p.Name == item.Name))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        private static void Add(List<Amenity> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.Amenity))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Amenities");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("Amenity");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("Name", item.Name.ToString());

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

        public static void Update(Amenity item)
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

        public static void Delete(Amenity item)
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
