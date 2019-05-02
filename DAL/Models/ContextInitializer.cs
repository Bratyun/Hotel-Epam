using DAL.Account;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    internal class ContextInitializer : DropCreateDatabaseIfModelChanges<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            ApplicationRole roleOwner = new ApplicationRole { Name = "Owner" };
            ApplicationRole roleAdmin = new ApplicationRole { Name = "Admin" };
            ApplicationRole roleUser = new ApplicationRole { Name = "User" };

            context.Roles.Add(roleOwner);
            context.Roles.Add(roleAdmin);
            context.Roles.Add(roleUser);
            context.SaveChanges();
        }
    }
}
