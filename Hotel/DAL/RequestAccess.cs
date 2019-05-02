using DAL.Account;
using DAL.EntityClasses;
using DAL.Models;
using Hotel.Models;
using NLog;
using System;
using System.Collections.Generic;

namespace Hotel.DAL
{
    public static class RequestAccess
    {
        private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Return all Requests with status New or Refused
        /// </summary>
        /// <returns></returns>
        internal static List<Request> GetRequestsForAdmin()
        {
            Logger.Debug("Get Requests from database to admin");
            List<Request> result = Request.GetNewAndRefused();
            if (result == null || result.Count == 0)
            {
                Logger.Info("Requests not found in database");
            }
            else
            {
                Logger.Debug("Requests found in database");
            }
            return result;
        }

        /// <summary>
        /// Convert list of Requests to list of RequestViewModels
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public static List<RequestViewModel> RequestsToModels(List<Request> requests)
        {
            Logger.Debug("Convert list of Requests to list of RequestViewModels");
            List<RequestViewModel> models = new List<RequestViewModel>();
            foreach (var item in requests)
            {
                models.Add(new RequestViewModel(item));
            }
            Logger.Debug("Finish of converting list of Requests to list of RequestViewModels");
            return models;
        }

        internal static List<Request> GetRequestsByUserName(string userName, ApplicationUserManager manager)
        {
            Logger.Debug("Get Request by user name");
            if (manager == null)
            {
                Logger.Error("Invalid user manager");
                return null;
            }
            ApplicationUser user = manager.FindByNameAsync(userName).Result;
            if (user != null)
            { 
                List<Request> result = Request.GetByUser(user.Id);
                if (result == null || result.Count == 0)
                {
                    Logger.Info("Requests not found in database");
                }
                else
                {
                    Logger.Debug("Requests found in database");
                }
                return result;
            }
            else
            {
                Logger.Debug("Get Request by user name. User not found");
            }
            return null;
        }

        /// <summary>
        /// Convert object of RequestViewModel and return exist or new Request object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static Request ModelToRequest(RequestViewModel model)
        {
            Logger.Debug("Convert RequestViewModel to Request");
            Request request = GetById(model.Id);
            if (request == null)
            {
                request = new Request();
                request.Id = model.Id;
            }
            request.RoomSize = model.RoomSize;
            request.Status = model.Status;
            request.Comfort = model.Comfort;
            request.EndDate = model.EndDate;
            request.StartDate = model.StartDate;
            request.UserId = model.UserId;
            Logger.Debug("Finish of converting RequestViewModel to Request");
            return request;
        }

        internal static Request GetById(int id)
        {
            Logger.Debug("Get user by id");
            Request result = Request.GetById(id);
            if (result == null)
            {
                Logger.Error("Request not found in database");
            }
            else
            {
                Logger.Debug("Request found in database");
            }
            return result;
        }

        internal static Request Add(Request request)
        {
            Logger.Debug("Add Request in database start");
            Request result = Request.Add(request);
            if (result == null)
            {
                Logger.Error("Request not added in database");
            }
            else
            {
                Logger.Debug("Request added in database");
            }
            return result;
        }

        internal static bool Update(Request request)
        {
            Logger.Debug("Update Request in database start");
            bool result = Request.Update(request);
            if (!result)
            {
                Logger.Error("Request not updated in database");
            }
            else
            {
                Logger.Debug("Request updated in database");
            }
            return result;
        }

        /// <summary>
        /// Returns Requests with the specified username and status
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="waiting"></param>
        /// <returns></returns>
        internal static List<Request> GetRequestsByUserNameAndStatus(string userName, RequestStatus waiting)
        {
            Logger.Debug("Get Requests by user name and Request status");
            ApplicationUser user = ApplicationUser.GetUserByName(userName);
            if (user != null)
            {
                List<Request> result = Request.GetByUserAndStatus(user.Id, waiting);
                if (result == null || result.Count == 0)
                {
                    Logger.Error("Requests not found in database");
                }
                else
                {
                    Logger.Debug("Requests found in database");
                }
            }
            else
            {
                Logger.Error("Get Request by user name and Request status. User not found");
            }
            return null;
        }

        internal static bool Delete(int id)
        {
            Logger.Debug("Delete Request from database start");
            bool result = Request.Delete(id);
            if (!result)
            {
                Logger.Error("Request not deleted from database");
            }
            else
            {
                Logger.Debug("Request deleted from database");
            }
            return result;
        }
    }
}