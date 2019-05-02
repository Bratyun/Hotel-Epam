using DAL.EntityClasses;
using DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Account
{
    public class ApplicationUser : IdentityUser
    {
        public static List<ApplicationUser> GetUsers()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    return db.Users.Include(x => x.Roles).ToList();
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }

        public ApplicationUser()
        {
        }

        public static bool AddToRole(string userId, string roleName)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                var user = db.Users.Where(x => x.Id == userId).FirstOrDefault();
                var role = db.Roles.Where(x => x.Name == roleName).FirstOrDefault();
                if (user != null && role != null)
                {
                    userManager.AddToRole(user.Id, role.Name);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool RemoveFromRole(string userId, string roleId)
        {
            using(ApplicationContext db = new ApplicationContext())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                var user = db.Users.Where(x => x.Id == userId).FirstOrDefault();
                var role = db.Roles.Where(x => x.Id == roleId).FirstOrDefault();
                if (user != null && role != null)
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }


        public static ApplicationUser GetUserByName(string userName)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                ApplicationUser user = userManager.FindByName(userName);
                return user;
            }
        }

        
    }
}
