using DAL.Account;
using DAL.EntityClasses;
using DAL.Models;
using Hotel.DAL;
using Hotel.Models;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        [AllowAnonymous]
        public ActionResult List(RoomSortBy orderBy = RoomSortBy.None, bool desc = false)
        {
            Logger.Debug("Get list of rooms");
            List<Room> rooms = RoomAccess.GetRooms(orderBy, desc);
            Logger.Debug("End of getting list of rooms");
            return View(rooms);
        }

        [Authorize(Roles = "Owner, Admin")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RoomRegisterViewModel model, HttpPostedFileBase uploadImage)
        {
            Logger.Debug("Start of creating new room");
            if (ModelState.IsValid && uploadImage != null)
            {
                string type = uploadImage.ContentType;
                if (!type.StartsWith("image"))
                {
                    Logger.Debug("It is not image");
                    ModelState.AddModelError("Image", "It is not image");
                    return View(model);
                }

                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                model.Image = imageData;

                Room room = RoomAccess.Add(model);
                Logger.Debug("End of creating new room. Success");
                return RedirectToAction("List");
            }
            if (uploadImage == null)
            {
                Logger.Debug("Not valied file of image");
                ModelState.AddModelError("", "Not valied file");
            }
            Logger.Debug("End of creating new room. Not success");
            return View(model);
        }

        [Authorize(Roles = "Owner, Admin")]
        public ActionResult Delete(int id)
        {
            Logger.Debug("Start of deleting new room");
            RoomAccess.Delete(id);
            Logger.Debug("End of deleting new room");
            return RedirectToAction("List", ModelState);
        }

        [Authorize(Roles = "Owner, Admin")]
        public ActionResult Edit(int id)
        {
            Logger.Debug("Get room for update");
            Room room = RoomAccess.GetById(id);
            RoomEditViewModel model = new RoomEditViewModel();
            if (room != null)
            {
                model = new RoomEditViewModel(room);
            }
            Logger.Debug("End of getting room for update");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoomEditViewModel model, HttpPostedFileBase uploadImage)
        {
            Logger.Debug("Start of updating room");
            Room room = RoomAccess.GetById(model.Id);
            if (room == null)
            {
                Logger.Error("Room not found");
                ModelState.AddModelError("", "Room not found");
            }

            if (!Tools.IsDefaultDate(model.StartDate, model.EndDate) && Tools.IsInvalidDate(model.StartDate, model.EndDate))
            {
                Logger.Debug("Long period of reserving or invalid date period");
                ModelState.AddModelError("StartDate", "Long period of reserving or invalid date period");
                ModelState.AddModelError("EndDate", "Long period of reserving or invalid date period");
            }

            if (uploadImage != null)
            {
                string type = uploadImage.ContentType;
                if (!type.StartsWith("image"))
                {
                    ModelState.AddModelError("Image", "It is not image");
                }

                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                model.Image = imageData;
            }

            if (ModelState.IsValid)
            {
                if (model.UserId == null && room.UserId != null)
                {
                    Check check = CheckAccess.GetByUserAndRoomId(room.UserId, room.Id);
                    if (check != null)
                    {
                        check.Status = CheckStatus.Failed;
                        CheckAccess.Update(check);
                    }
                    model.User = null;
                    model.UserId = null;
                }
                else if (model.UserId != null)
                {
                    ApplicationUser user = UserManager.FindByIdAsync(model.UserId).Result;
                    if (user == null || user.Id != model.UserId)
                    {
                        Logger.Debug("User not found");
                        ModelState.AddModelError("UserId", "User not found with same id");
                        return View(model);
                    }
                    else
                    {
                        Logger.Debug("User found");
                        if (model.UserId != room.UserId)
                        {
                            Check check = CheckAccess.GetByUserAndRoomId(room.UserId, room.Id);
                            if (check != null)
                            {
                                check.Status = CheckStatus.Failed;
                                CheckAccess.Update(check);
                                Check newCheck = new Check(user?.Id, model.Id, model.Price, DateTime.Now, CheckStatus.New);
                                newCheck = CheckAccess.Add(newCheck);
                            }
                            else if (room.UserId == null)
                            {
                                Check newCheck = new Check(user?.Id, model.Id, model.Price, DateTime.Now, CheckStatus.New);
                                newCheck = CheckAccess.Add(newCheck);
                            }
                        }
                        model.User = user;
                        model.UserId = user.Id;
                    }
                }

                Logger.Debug("Model is valid");
                Room toUpdate = RoomAccess.RoomEditModelToRoom(model);
                RoomAccess.Update(toUpdate);
                Logger.Debug("End of updating room");
                return RedirectToAction("List");
            }
            else
            {
                Logger.Debug("Model is not valid");
            }
            Logger.Debug("End of updating room");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult Reserve(int id)
        {
            Logger.Debug("Start of getting model to reserv room");
            Room room = RoomAccess.GetById(id);
            RoomReserveViewModel model = new RoomReserveViewModel(room);
            Logger.Debug("End of getting model to reserv room");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult Reserve(RoomReserveViewModel model)
        {
            Logger.Debug("Start of reserving room");
            if (Tools.IsInvalidDate(model.StartDate, model.EndDate))
            {
                Logger.Debug("Long period of reserving or invalid date period");
                ModelState.AddModelError("StartDate", "Long period of reserving or invalid date period");
                ModelState.AddModelError("EndDate", "Long period of reserving or invalid date period");
            }

            if (ModelState.IsValid)
            {
                Logger.Debug("Model is valid");
                Room room = RoomAccess.ReserveModelToRoom(model);
                room.Status = RoomStatus.Booked;
                ApplicationUser user = UserManager.FindByNameAsync(User.Identity.Name).Result;
                room.User = user;
                room.UserId = user?.Id;
                RoomAccess.Update(room);
                Check check = new Check(user?.Id, room.Id, room.Price, DateTime.Now, CheckStatus.New);
                check = CheckAccess.Add(check);
                Logger.Debug("Start of reserving room");
                return RedirectToAction("List");
            }
            else
            {
                Logger.Debug("Model is not valid");
            }
            Logger.Debug("Start of reserving room");
            return View(model);
        }
    }
}