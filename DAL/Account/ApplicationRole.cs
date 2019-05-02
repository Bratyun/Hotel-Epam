using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Account
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        { }

        public static IdentityRole GetRoleById(string id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                try
                {
                    return db.Roles.Where(x => x.Id == id).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
