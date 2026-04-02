using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_Dev_IT.Service
{
    public class RoomService : IRoom
    {
        private readonly BookingContext _context;
        private readonly ILogger<RoomService> _logger;
        public RoomService(BookingContext context,
               ILogger<RoomService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Room> Create(Room room)
        {
            try
            {
                var rooms = await _context.room
                            .FirstOrDefaultAsync(x => 
                            x.RoomID == room.RoomID);

                if (rooms?.RoomID == room.RoomID)
                {
                    _logger.LogError($"{room} already exist");
                    throw new InvalidOperationException(
                              "Room already exist");
                }

                var insert = new Room
                {
                    RoomNumber = room.RoomNumber,
                    RoomType = room.RoomType,
                    PricePerNight = room.PricePerNight,
                    IsAvailable = room.IsAvailable = false,
                };

                _context.room.Add(insert);
                await _context.SaveChangesAsync();

                return insert;

            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }

        public async Task<Room> Update(Room room, int id)
        {
            try
            {
                var rooms = await _context.room
                            .FirstOrDefaultAsync(x => 
                            x.RoomID == id);

                if (rooms?.RoomID != room.RoomID)
                {
                    _logger.LogError($"{room} Invalid Room");
                    throw new InvalidOperationException(
                              "Invalid Room");
                }

                rooms.RoomNumber = room.RoomNumber;
                rooms.RoomType = room.RoomType;
                rooms.PricePerNight = room.PricePerNight;
                rooms.IsAvailable = room.IsAvailable;

                _context.Update(rooms);
                await _context.SaveChangesAsync();

                return rooms;
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }

        public async Task<Room> Delete(int id)
        {
            try
            {
                var room = await _context.room
                           .FirstOrDefaultAsync(x =>
                           x.RoomID == id);

                if (room is null || room.RoomID != id)
                {
                    _logger.LogError($"{room} Room doesnt exist");
                    throw new InvalidOperationException(
                              "Room doesnt exist");
                }

                _context.room.Remove(room);
                await _context.SaveChangesAsync();

                return room;
            }
            catch (InvalidOperationException x)
            {
                _logger.LogError(x.Message);
                throw new Exception(x.Message);
            }
        }
    }
}
