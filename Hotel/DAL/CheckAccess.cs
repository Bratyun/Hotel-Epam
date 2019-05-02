using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Account;
using DAL.EntityClasses;
using Hotel.Models;
using NLog;

namespace Hotel.DAL
{
    public static class CheckAccess
    {
        private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        internal static Check Add(Check check)
        {
            Logger.Debug("Add Check to database");
            Check result = Check.Add(check);
            if (result == null)
            {
                Logger.Error("Check not added to database");
            }
            else
            {
                Logger.Debug("Check added to database");
            }
            return result;
        }

        internal static List<CheckViewModel> GetAllFormAdmin()
        {
            Logger.Debug("Get Checks by user name from database");
            List<Check> checks = Check.GetAll();
            List<CheckViewModel> models = new List<CheckViewModel>();
            foreach (var item in checks)
            {
                Room room = RoomAccess.GetById(item.RoomId);
                if (room != null)
                {
                    models.Add(new CheckViewModel(item, room));
                }
                else
                {
                    Logger.Error("When we get Checks by user name from database. Room from Check not found");
                }
            }
            Logger.Debug("End of getting Checks by user name from database");
            return models;
            
        }

        internal static List<CheckViewModel> GetAllByUser(string userName)
        {
            Logger.Debug("Get Checks by user name from database");
            ApplicationUser user = ApplicationUser.GetUserByName(userName);
            if (user != null)
            {
                List<Check> checks = Check.GetByUser(user.Id);
                List<CheckViewModel> models = new List<CheckViewModel>();
                foreach (var item in checks)
                {
                    Room room = RoomAccess.GetById(item.RoomId);
                    if (room != null)
                    {
                        models.Add(new CheckViewModel(item, room));
                    }
                    else
                    {
                        Logger.Error("When we get Checks by user name from database. Room from Check not found");
                    }
                }
                Logger.Debug("End of getting Checks by user name from database");
                return models;
            }
            else
            {
                Logger.Debug("When we get Checks by user name from database. User not found");
            }
            return null;
        }

        internal static Check GetById(int id)
        {
            Logger.Debug("Get Check from database by id");
            Check result = Check.GetById(id);
            if (result == null)
            {
                Logger.Error("Check not found in database");
            }
            else
            {
                Logger.Debug("Check found in database");
            }
            return result;
        }

        internal static List<Check> GetAll()
        {
            Logger.Debug("Get all Checks from database");
            List<Check> result = Check.GetAll();
            if (result == null || result.Count == 0)
            {
                Logger.Info("Check not found in database");
            }
            else
            {
                Logger.Debug("Check found in database");
            }
            return result;
        }

        /// <summary>
        /// Method which control time pay for users
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="e"></param>
        internal static void CheckTimeOver(object sourse, System.Timers.ElapsedEventArgs e)
        {
            List<Check> checks = CheckAccess.GetAll();
            if (checks == null)
            {
                return;
            }
            foreach (var item in checks)
            {
                if ((DateTime.Now.Date - item.RegisterDate.Date).TotalDays >= 2 && item.Status == CheckStatus.New)
                {
                    RoomAccess.SetRoomFree(item.RoomId);
                    item.Status = CheckStatus.Failed;
                    CheckAccess.Update(item);
                }
            }
        }

        internal static bool Delete(int id)
        {
            Logger.Debug("Start of deleting Check from database");
            bool result = Check.Delete(id);
            if (!result)
            {
                Logger.Error("Check not deleted from database");
            }
            else
            {
                Logger.Debug("Check deleted from database");
            }
            return result;
        }

        public static bool Update(Check item)
        {
            Logger.Debug("Start of updating Check in database");
            bool result = Check.Update(item);
            if (!result)
            {
                Logger.Error("Check not updated in database");
            }
            else
            {
                Logger.Debug("Check updated in database");
            }
            return result;
        }

        internal static Check GetByUserAndRoomId(string userId, int roomId)
        {
            Logger.Debug("Get Check from database by user id and room id");
            List<Check> checks = Check.GetByUser(userId);
            foreach (var item in checks)
            {
                if (item.RoomId == roomId)
                {
                    Logger.Debug("Check found");
                    return item;
                }
            }
            Logger.Debug("Check not found");
            return null;
        }
    }
}