using DAL.Account;
using DAL.Models;
using Hotel.Models;
using Microsoft.Owin.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Hotel.DAL
{
    public static class UserAccess
    {
        private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        internal static bool AddToRole(string userId, string roleName)
        {
            Logger.Debug("Add user to role");
            bool result = ApplicationUser.AddToRole(userId, roleName);
            if (result)
            {
                Logger.Debug("Add user to role. Success");
            }
            else
            {
                Logger.Error("User not added to role");
            }
            return result;
        }

        internal static bool RemoveFromRole(string userId, string roleId)
        {
            Logger.Debug("Remove user from role");
            bool result = ApplicationUser.RemoveFromRole(userId, roleId);
            if (result)
            {
                Logger.Debug("Remove user from role. Success");
            }
            else
            {
                Logger.Error("User not removed from role");
            }
            return result;
        }

        internal static List<ApplicationUser> GetUsers()
        {
            Logger.Debug("Get users from database");
            List<ApplicationUser> result = ApplicationUser.GetUsers();
            if (result == null || result.Count == 0)
            {
                Logger.Info("Users not found in database");
            }
            else
            {
                Logger.Debug("Users found in database");
            }
            return result;
        }

        /// <summary>
        /// Convert from UserWithRoleViewModel to ApplicationUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static ApplicationUser ModelToApplicationUser(UserWithRoleViewModel user)
        {
            Logger.Debug("Convert UserWithRoleViewModel to User");
            return new ApplicationUser()
            {
                Id = user?.Id,
                UserName = user?.UserName,
                PhoneNumber = user?.PhoneNumber,
                Email = user?.Email,
            };
        }

        /// <summary>
        /// Return objects of UserWithRoleViewModels
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <returns></returns>
        internal static List<UserWithRoleViewModel> GetUsersWithRole(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            try
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    var usersWithRoles = (from user in context.Users
                        select new
                        {
                            UserId = user.Id,
                            Username = user.UserName,
                            Email = user.Email,
                            RoleNames = (from userRole in user.Roles
                                join role in context.Roles on userRole.RoleId
                                    equals role.Id
                                select role.Name).ToList(),
                            RoleId = (from userRole in user.Roles
                                join role in context.Roles on userRole.RoleId
                                    equals role.Id
                                select role.Id).ToList().FirstOrDefault()
                        }).ToList().Select(p => new UserWithRoleViewModel

                    {
                        Id = p.UserId,
                        UserName = p.Username,
                        Email = p.Email,
                        RoleName = p.RoleNames.FirstOrDefault(),
                        RoleId = p.RoleId
                    });
                    return usersWithRoles.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error from UserAccess.GetUsersWithRole: " + ex.ToString());
                return null;
            }
        }

        internal static ApplicationUser GetUserByName(string name)
        {
            Logger.Debug("Get User from database by name");
            ApplicationUser result = ApplicationUser.GetUserByName(name);
            if (result == null)
            {
                Logger.Info("User not found");
            }
            else
            {
                Logger.Info("User found");
            }
            return result;
        }

        internal static UserWithRoleViewModel GetUserWithRole(ApplicationUserManager userManager, ApplicationRoleManager roleManager, string id)
        {
            ApplicationUser user = userManager.Users.Include(x => x.Roles).Where(x => x.Id == id).FirstOrDefault();
            if (user != null)
            {
                ApplicationRole role = RoleAccess.GetRoleByUser(user);
                UserWithRoleViewModel us = new UserWithRoleViewModel
                {
                    Id = user.Id,
                    RoleName = role.Name,
                    RoleId = role.Id,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email
                };
                if (us != null)
                {
                    return us;
                }
            }
            return null;
        }
    }
}