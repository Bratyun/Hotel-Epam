using DAL.Account;
using DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EntityClasses
{
    public enum RoomStatus
    {
        None,
        Free,
        Booked,
        Busy,
        Closed
    }

    public enum RoomSortBy
    {
        None,
        RoomSize,
        Comfort,
        Status,
        Price
    }

    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Range(0, 10)]
        public int RoomSize { get; set; }
        [Range(0, 5)]
        public int Comfort { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public RoomStatus Status { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime EndDate { get; set; }
        public byte[] Image { get; set; }
        [Required]
        public double Price { get; set; }

        public Room()
        {
            RoomSize = 0;
            Comfort = 0;
            UserId = null;
            User = null;
            Status = RoomStatus.None;
            StartDate = default(DateTime);
            EndDate = default(DateTime);
            Image = null;
            Price = 0;
        }

        public Room(int roomSize, int comfort, byte[] image, double price) : this(roomSize, comfort, image, price, RoomStatus.None)
        { }

        public Room(int roomSize, int comfort, byte[] image, double price, RoomStatus roomStatus) : this(roomSize, comfort, image, price, roomStatus, null, default(DateTime), default(DateTime))
        { }

        public Room(int roomSize, int comfort, byte[] image, double price, RoomStatus roomStatus, ApplicationUser user, DateTime start, DateTime end)
        {
            RoomSize = roomSize;
            Comfort = comfort;
            UserId = user?.Id;
            User = user;
            Status = roomStatus;
            StartDate = start;
            EndDate = end;
            Image = image;
            Price = price;
        }

        public static Room Add(Room room)
        {
            if (!room.IsValidDates())
            {
                return null;
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault();
                if (c == null)
                {
                    db.Rooms.Add(room);
                    db.SaveChanges();
                    return room;
                }
                return null;
            }
        }

        public bool IsValidDates()
        {
            if (StartDate == default(DateTime) && EndDate == default(DateTime))
            {
                return true;
            }
            return (StartDate.Date >= EndDate.Date || EndDate.Date < DateTime.Now) ? false : true;
        }

        public static bool Update(Room room)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault();
                if (c != null)
                {
                    c.RoomSize = room.RoomSize;
                    c.Comfort = room.Comfort;
                    c.Status = room.Status;
                    c.StartDate = room.StartDate;
                    c.EndDate = room.EndDate;
                    c.Image = room.Image;
                    c.Price = room.Price;
                    db.SaveChanges();
                    if (c.UserId != room.UserId)
                    {
                        var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                        var userOld = userManager.FindById(c.UserId);
                        if (userOld != null)
                        {
                            db.SaveChanges();
                        }
                        var newUser = userManager.FindById(room.UserId);
                        if (newUser != null)
                        {
                            c.User = newUser;
                            c.UserId = newUser.Id;
                        }
                        else
                        {
                            c.User = null;
                            c.UserId = null;
                        }
                        db.SaveChanges();
                    }
                    return true;
                }
                return false;
            }
        }

        public static Room GetById(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Rooms.Where(x => x.Id == id).FirstOrDefault();
                if (c != null)
                {
                    return c;
                }
                return null;
            }
        }

        public static List<Room> GetByUser(string userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    List<Room> rooms = db.Rooms.Where(x => x.UserId == userId).ToList();
                    if (rooms != null)
                    {
                        return rooms;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static List<Room> GetAll(RoomSortBy sortBy = RoomSortBy.None, bool desc = false)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    List<Room> rooms = new List<Room>();
                    switch (sortBy)
                    {
                        case RoomSortBy.None:
                            rooms = db.Rooms.ToList();
                            break;
                        case RoomSortBy.RoomSize:
                            rooms = db.Rooms.OrderBy(x => x.RoomSize).ToList();
                            break;
                        case RoomSortBy.Comfort:
                            rooms = db.Rooms.OrderBy(x => x.Comfort).ToList();
                            break;
                        case RoomSortBy.Status:
                            rooms = db.Rooms.OrderBy(x => x.Status).ToList();
                            break;
                        case RoomSortBy.Price:
                            rooms = db.Rooms.OrderBy(x => x.Price).ToList();
                            break;
                        default:
                            break;
                    }
                    if (rooms != null && desc)
                    {
                        rooms.Reverse();
                    }
                    return rooms;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static bool Delete(Room room)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Rooms.Where(x => x.Id == room.Id).FirstOrDefault();
                if (c != null)
                {
                    db.Rooms.Remove(c);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
