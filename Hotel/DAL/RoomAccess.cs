using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.EntityClasses;
using Hotel.Models;
using NLog;

namespace Hotel.DAL
{
    public static class RoomAccess
    {
        private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        internal static Room Add(RoomRegisterViewModel model)
        {
            Logger.Debug("Add Room to database");
            Room room = new Room(model.RoomSize, model.Comfort, model.Image, model.Price, model.Status);
            Room roomWithId = Room.Add(room);
            if (roomWithId != null)
            {
                Logger.Debug("Room added to database");
                return roomWithId;
            }
            else
            {
                Logger.Error("Room not added to database");
            }
            return null;
        }

        internal static List<Room> GetRooms(RoomSortBy orderBy, bool desc)
        {
            Logger.Debug("Get all Rooms from database");
            List<Room> result = Room.GetAll(orderBy, desc);
            if (result == null || result.Count == 0)
            {
                Logger.Info("Rooms not found in database");
            }
            else
            {
                Logger.Debug("Rooms found in database");
            }
            return result;
        }

        internal static List<Room> GetRoomByComfortAndSize(int comfort, int size)
        {
            Logger.Debug("Get all Rooms from database");   
            List<Room> result = Room.GetByComfortAndSize(comfort, size);
            if (result == null)
            {
                Logger.Info("Rooms not found in database");
            }
            else if (result.Count == 0)
            {
                result = GetRooms(RoomSortBy.None, false);
            }
            else
            {
                Logger.Debug("Rooms found in database");
            }
            return result;
        }

        internal static bool Delete(int roomId)
        {
            Logger.Debug("Start delete Room from database");
            bool result = Room.Delete(roomId);
            if (!result)
            {
                Logger.Info("Room not deleted from database");
            }
            else
            {
                Logger.Debug("Room deleted from database");
            }
            return result;
        }

        internal static Room GetById(int id)
        {
            Logger.Debug("Get Room from database by id");
            Room result = Room.GetById(id);
            if (result == null)
            {
                Logger.Info("Room not found in database");
            }
            else
            {
                Logger.Debug("Room found in database");
            }
            return result;
        }

        internal static bool Update(Room model)
        {
            Logger.Debug("Update Room in database");
            bool result = Room.Update(model);
            if (!result)
            {
                Logger.Info("Room not updated in database");
            }
            else
            {
                Logger.Debug("Room updated in database");
            }
            return result;
        }

        /// <summary>
        /// Convert object of RoomReserveViewModel and return exist or new Room object
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static Room ReserveModelToRoom(RoomReserveViewModel model)
        {
            Logger.Debug("Convert RoomReserveViewModel to Room");
            Room room = Room.GetById(model.Id);
            if (room == null)
            {
                room = new Room();
                room.Id = model.Id;
            }
            room.RoomSize = model.RoomSize;
            room.Comfort = model.Comfort;
            room.Price = model.Price;
            room.Image = model.Image;
            room.StartDate = model.StartDate;
            room.EndDate = model.EndDate;
            Logger.Debug("End of converting RoomReserveViewModel to Room");
            return room;
        }

        /// <summary>
        /// Sets room free if date of reserving ended 
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="e"></param>
        internal static void RoomTimeOver(object sourse, System.Timers.ElapsedEventArgs e)
        {
            List<Room> rooms = GetRooms(RoomSortBy.None, false);
            if (rooms != null)
            {
                foreach (var item in rooms)
                {
                    if (item.EndDate.Date < DateTime.Now.Date)
                    {
                        SetRoomFree(item.Id);
                    }
                }
            }

        }

        /// <summary>
        /// Sets status of room in free and delet user data
        /// </summary>
        /// <param name="id"></param>
        internal static void SetRoomFree(int id)
        {
            Logger.Debug("Set room free");
            Room room = RoomAccess.GetById(id);
            room.User = null;
            room.UserId = null;
            room.Status = RoomStatus.Free;
            room.StartDate = default(DateTime);
            room.EndDate = default(DateTime);
            RoomAccess.Update(room);
            Logger.Debug("Set room free");
        }

        internal static Room RoomEditModelToRoom(RoomEditViewModel model)
        {
            Logger.Debug("Convert RoomEditViewModel to Room");
            Room room = Room.GetById(model.Id);
            if (room == null)
            {
                room = new Room();
                room.Id = model.Id;
            }
            room.RoomSize = model.RoomSize;
            room.Comfort = model.Comfort;
            room.Price = model.Price;
            room.Image = model.Image;
            room.StartDate = model.StartDate;
            room.EndDate = model.EndDate;
            room.User = model.User;
            room.UserId = model.UserId;
            room.Status = model.Status;
            Logger.Debug("End of converting RoomEditViewModel to Room");
            return room;
        }
    }
}