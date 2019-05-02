using DAL.Account;
using DAL.Models;
using Hotel.DAL;
using Hotel.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    [Authorize]
    [Authorize(Roles = "Owner")]
    public class UserController : Controller
    {
        private Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        
        public ActionResult List()
        {
            Logger.Debug("Get list of users");
            List<UserWithRoleViewModel> users = UserAccess.GetUsersWithRole(UserManager, RoleManager);
            Logger.Debug("End of getting list of users");
            return View(users);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            Logger.Debug("Load user for update");
            ViewBag.Roles = RoleManager.Roles.ToList();
            ApplicationUser user = UserManager.FindByIdAsync(id).Result;
            UserWithRoleViewModel vm = new UserWithRoleViewModel(user);
            Logger.Debug("End of loading user for update");
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(UserWithRoleViewModel user)
        {
            Logger.Debug("Start of updating user");
            ApplicationUser ap = UserManager.FindByIdAsync(user.Id).Result;
            ApplicationRole role = RoleManager.FindByIdAsync(user.RoleId).Result;

            if (user.UserName == null)
            {
                Logger.Debug("Entered invalid login");
                ModelState.AddModelError("UserName", "Invalid Login");
            }
            else
            {
                ApplicationUser existing = UserManager.FindByNameAsync(user.UserName).Result;
                if (existing?.Id == user.Id)
                {
                    Logger.Debug("Entered invalid login. User exists with same login");
                    ModelState.AddModelError("UserName", "Sorry but user exists with same login");
                }
                else
                {
                    ap.UserName = user.UserName;
                }
            }

            if (user.PhoneNumber != null && !Tools.IsCorrectPhoneNumber(user.PhoneNumber))
            {
                Logger.Debug("Entered invalid phone number");
                ModelState.AddModelError("PhoneNumber", "Phone number need have form like 000-000-0000");
            }
            else
            {
                ap.PhoneNumber = user.PhoneNumber;
            }
            
            if (ModelState.IsValid)
            {
                if (role?.Name != user.RoleName)
                {
                    Logger.Debug("Start of changing user role");
                    UserAccess.RemoveFromRole(ap.Id, user.RoleId);
                    UserAccess.AddToRole(ap.Id, user.RoleName);
                    Logger.Debug("End of changing user role");
                }
                ap.Email = user.Email;
                var res = UserManager.UpdateAsync(ap).Result;
                Logger.Debug("End of updating user");
                return RedirectToAction("List");
            }
            else
            {
                Logger.Debug("Invalid model of updating");
            }

            Logger.Debug("End of updating user");
            ViewBag.Roles = RoleManager.Roles.ToList();
            return View(user);
        }

        public ActionResult Delete(string id)
        {
            Logger.Debug("Start of deleting user");
            ApplicationUser apUser = UserManager.FindByNameAsync(id).Result;
            UserWithRoleViewModel user = UserAccess.GetUserWithRole(UserManager, RoleManager, apUser?.Id);

            if (user.RoleName == "Owner")
            {
                return RedirectToAction("List");
            }
            else
            {
                UserManager.DeleteAsync(apUser);
            }

            if (User.Identity.Name == user.UserName)
            {
                return RedirectToAction("Logout", "Account");
            }

            Logger.Debug("End of deleting user");
            return RedirectToAction("List");
        }
    }
}