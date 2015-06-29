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
    public class CinemaAmenities
    {
        #region Get

        public static CinemaAmenity Get(Guid id)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.Id == id).FirstOrDefault();

                if (item == null)
                    return new CinemaAmenity();

                return item;
            }
            catch
            {
                return new CinemaAmenity();
            }
        }

        public static List<CinemaAmenity> GetByCinemaId(Guid cinemaId)
        {
            try
            {
                var items = GetAll();
                var item = items.Where(p => p.CinemaId == cinemaId).ToList();

                if (item == null)
                    return new List<CinemaAmenity>();

                return item;
            }
            catch
            {
                return new List<CinemaAmenity>();
            }
        }

        public static List<CinemaAmenity> GetAll()
        {
            try
            {
                string data = Path.CinemaAmenity;

                if (!File.Exists(Path.CinemaAmenity))
                    return new List<CinemaAmenity>();

                DataSet ds = new DataSet();
                ds.ReadXml(data);
                var CinemaAmenities = new List<CinemaAmenity>();
                CinemaAmenities = (from rows in ds.Tables[0].AsEnumerable()
                                   select new CinemaAmenity
                                   {
                                       Id = Guid.Parse(rows[0].ToString()),
                                       CinemaId = Guid.Parse(rows[1].ToString()),
                                       AmenityId = Guid.Parse(rows[2].ToString()),

                                       Cinema = new Lazy<Cinema>(() =>
                                       {
                                           return Cinemas.Get(Guid.Parse(rows[1].ToString()));
                                       }),

                                       Amenity = new Lazy<Amenity>(() =>
                                       {
                                           return Amenities.Get(Guid.Parse(rows[2].ToString()));
                                       })

                                   }).ToList();

                if (CinemaAmenities == null)
                    return new List<CinemaAmenity>();

                return CinemaAmenities;
            }
            catch
            {
                return new List<CinemaAmenity>();
            }
        }

        #endregion

        #region Add

        public static void Add(CinemaAmenity item)
        {
            try
            {
                var items = GetAll();

                if (items.Any(p => p.Id == item.Id || (p.CinemaId == item.CinemaId && p.AmenityId == item.AmenityId)))
                    return;

                items.Add(item);
                Add(items);
            }
            catch { }
        }

        public static void Add(List<CinemaAmenity> items)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(Path.CinemaAmenity))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("CinemaAmenities");

                    foreach (var item in items)
                    {
                        writer.WriteStartElement("CinemaAmenity");

                        writer.WriteElementString("Id", item.Id.ToString());
                        writer.WriteElementString("CinemaId", item.CinemaId.ToString());
                        writer.WriteElementString("AmenityId", item.AmenityId.ToString());

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

        public static void Update(CinemaAmenity item)
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

        public static void Delete(CinemaAmenity item)
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

        public static void DeleteByCinemaId(Guid id)
        {
            try
            {
                var items = GetAll();
                var itemsToDelete = items.Where(p => p.CinemaId == id);

                foreach (var itemToDelete in itemsToDelete)
                {
                    Delete(itemToDelete);
                }
            }
            catch { }
        }

        #endregion
    }
}
