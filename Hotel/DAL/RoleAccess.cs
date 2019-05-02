using DAL.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hotel.DAL
{
    public static class RoleAccess
    {
        internal static ApplicationRole GetRoleByUser(ApplicationUser user)
        {
            if (user == null)
            {
                return null;
            }
            string roleId = user.Roles.Where(x => x.UserId == user.Id).FirstOrDefault().RoleId;
            return ApplicationRole.GetRoleById(roleId) as ApplicationRole;
        }
    }
}