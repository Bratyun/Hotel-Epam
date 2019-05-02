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
    public enum RequestStatus
    {
        None,
        New,
        Waiting,
        Executed,
        Refused
    }

    public class Request
    {
        [Key]
        public int Id { get; set; }
        [Range(0, 10)]
        public int RoomSize { get; set; }
        [Range(0, 5)]
        public int Comfort { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime EndDate { get; set; }
        public string UserId { get; set; }
        public RequestStatus Status { get; set; }
        public int Answer { get; set; }

        public Request()
        {
            RoomSize = 0;
            Comfort = 0;
            StartDate = default(DateTime);
            EndDate = default(DateTime);
            UserId = null;
            Status = RequestStatus.None;
            Answer = 0;
        }

        public Request(int roomSize, int comfort, DateTime startDate, DateTime endDate, string userId, RequestStatus status, int roomsId = 0)
        {
            RoomSize = roomSize;
            Comfort = comfort;
            StartDate = startDate;
            EndDate = endDate;
            UserId = userId;
            Status = status;
            Answer = roomsId;
        }

        public static List<Request> GetByUserAndStatus(string userId, RequestStatus waiting)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    List<Request> requests = db.Requests.Where(x => x.UserId == userId && x.Status == waiting).ToList();
                    if (requests != null)
                    {
                        return requests;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static Request Add(Request request)
        {
            if (!request.IsValidDates())
            {
                return null;
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    var c = db.Requests.Where(x => x.Id == request.Id).FirstOrDefault();
                    if (c == null)
                    {
                        db.Requests.Add(request);
                        db.SaveChanges();
                        return request;
                    }
                    return null;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        public bool IsValidDates()
        {
            return (StartDate.Date >= EndDate.Date || EndDate.Date < DateTime.Now || StartDate.Date < DateTime.Now.Date) ? false : true;
        }

        public static Request GetById(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Requests.Where(x => x.Id == id).FirstOrDefault();
                if (c != null)
                {
                    return c;
                }
                return null;
            }
        }

        public static List<Request> GetByUser(string userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    List<Request> requests = db.Requests.Where(x => x.UserId == userId).ToList();
                    if (requests != null)
                    {
                        return requests;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static List<Request> GetNewAndRefused()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    List<Request> requests = db.Requests.Where(x => x.Status == RequestStatus.New || x.Status == RequestStatus.Refused).ToList();
                    if (requests != null)
                    {
                        return requests;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static bool Delete(int request)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Requests.Where(x => x.Id == request).FirstOrDefault();
                if (c != null)
                {
                    db.Requests.Remove(c);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool DeleteByUser(ApplicationUser user)
        {
            if (user == null)
            {
                return false;
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Requests.Where(x => x.UserId == user.Id).ToList();
                if (c != null)
                {
                    foreach (var item in c)
                    {
                        Delete(item.Id);
                    }
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool Update(Request request)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var c = db.Requests.Where(x => x.Id == request.Id).FirstOrDefault();
                if (c != null)
                {
                    c.Answer = request.Answer;
                    c.RoomSize = request.RoomSize;
                    c.StartDate = request.StartDate;
                    c.Status = request.Status;
                    c.UserId = request.UserId;
                    c.Comfort = request.Comfort;
                    c.EndDate = request.EndDate;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
