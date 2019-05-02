using DAL.EntityClasses;
using DAL.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Account
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public IDbSet<Request> Requests { get; set; }
        public IDbSet<Room> Rooms { get; set; }
        public IDbSet<Check> Checks { get; set; }

        static ApplicationContext()
        {
            Database.SetInitializer(new ContextInitializer());
        }

        public ApplicationContext() : base("DBConnection")
        { }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }
}
