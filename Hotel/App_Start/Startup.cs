using DAL.Account;
using DAL.Models;
using Hotel.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;

[assembly: OwinStartup(typeof(Hotel.App_Start.Startup))]
namespace Hotel.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationContext>(ApplicationContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
            CheckTimerStart();
            RoomTimerStart();
        }

        
        /// <summary>
        /// Start timer which call method of verify valid of check status
        /// </summary>
        private static void CheckTimerStart()
        {
            Timer checkTimer = new Timer(120000);
            checkTimer.Elapsed += CheckAccess.CheckTimeOver;
            checkTimer.AutoReset = true;
            checkTimer.Enabled = true;
        }

        /// <summary>
        /// Start timer which call method of verify valid of room status
        /// </summary>
        private static void RoomTimerStart()
        {
            Timer roomTimer = new Timer(120000);
            
            roomTimer.Elapsed += RoomAccess.RoomTimeOver;
            roomTimer.AutoReset = true;
            roomTimer.Enabled = true;
        }
    }
}