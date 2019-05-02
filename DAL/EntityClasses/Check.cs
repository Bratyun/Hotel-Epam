using DAL.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EntityClasses
{
    public enum CheckStatus
    {
        None,
        New,
        Paid,
        Failed,
        Stored
    }

    public class Check
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime2")]
        public DateTime RegisterDate { get; set; }
        public CheckStatus Status { get; set; }

        public Check()
        {
            UserId = null;
            RoomId = 0;
            Price = 0;
            RegisterDate = default(DateTime);
            Status = CheckStatus.None;
        }

        public Check(string userId, int roomId, double price, DateTime registerDate, CheckStatus status)
        {
            UserId = userId;
            RoomId = roomId;
            Price = price;
            RegisterDate = registerDate;
            Status = status;
        }

        public static Check Add(Check check)
        {
            if (!IsValid(check))
            {
                return null;
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Checks.Where(x => x.Id == check.Id).FirstOrDefault();
                if (c == null)
                {
                    db.Checks.Add(check);
                    db.SaveChanges();
                    return check;
                }
                return null;
            }
        }

        public static bool IsValid(Check check)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var user = db.Users.Where(x => x.Id == check.UserId).FirstOrDefault();
                var room = db.Rooms.Where(x => x.Id == check.RoomId).FirstOrDefault();
                if (user != null && room != null && room.UserId == user.Id && room.Price == check.Price)
                {
                    return true;
                }
                return false;
            }

        }

        public static bool Update(Check item)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Checks.Where(x => x.Id == item.Id).FirstOrDefault();
                if (c != null)
                {
                    c.Price = item.Price;
                    c.Status = item.Status;
                    c.RegisterDate = item.RegisterDate;
                    c.RoomId = item.RoomId;
                    c.UserId = item.UserId;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static Check GetById(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Checks.Where(x => x.Id == id).FirstOrDefault();
                if (c != null)
                {
                    return c;
                }
                return null;
            }
        }

        public static List<Check> GetByUser(string userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    List<Check> checks = db.Checks.Where(x => x.UserId == userId).ToList();
                    if (checks != null)
                    {
                        return checks;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static List<Check> GetAll()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    List<Check> checks = db.Checks.ToList();
                    if (checks != null)
                    {
                        return checks;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static bool Delete(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Checks.Where(x => x.Id == id).FirstOrDefault();
                if (c != null)
                {
                    db.Checks.Remove(c);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
