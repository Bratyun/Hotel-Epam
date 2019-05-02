using DAL.Account;
using DAL.Models;
using Hotel.DAL;
using Hotel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Hotel.Controllers
{
    public class AccountController : Controller
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            Logger.Debug("Start of login");
            if (ModelState.IsValid)
            {
                Logger.Debug("Search user in database");
                ApplicationUser user = await UserManager.FindAsync(model.Login, model.Password);
                if (user != null)
                {
                    Logger.Debug("User found");
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(claim);
                    Logger.Debug("End of login, user = " + User.Identity.Name);
                    return RedirectToAction("List", "Room");

                }
                else
                {
                    Logger.Debug("User not found");
                    ModelState.AddModelError("", "User not found");
                }
            }
            Logger.Debug("End of login");
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            Logger.Debug("Registration start");
            if (model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password need consist more than 6 symbols");
            }

            if (model.PhoneNumber != null && !Tools.IsCorrectPhoneNumber(model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number need have form like 000-000-0000");
            }

            Logger.Debug("Check. Is user valid");
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Login, Email = model.Email, PhoneNumber = model.PhoneNumber};
                IdentityResult res = await UserManager.CreateAsync(user, model.Password);
                if (res.Succeeded)
                {
                    UserAccess.AddToRole(user.Id, "User");
                    Logger.Debug("Success, registration end of new user = " + user.UserName);
                    return RedirectToAction("List", "Room");
                }
                else
                {
                    ModelState.AddModelError("", "Sorry, user exists with same login");
                }
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            Logger.Debug("Logout of user = " + User.Identity.Name);
            return RedirectToAction("List", "Room");
        }
    }
}