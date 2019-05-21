using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.Account.ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "DAL.Account.ApplicationContext";
        }

        protected override void Seed(DAL.Account.ApplicationContext context)
        {
            context.Roles.AddOrUpdate(
                p => p.Name,
                new IdentityRole("Owner"),
                new IdentityRole("Admin"),
                new IdentityRole("User"));
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
