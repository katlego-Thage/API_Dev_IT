using API_Dev_IT.Context;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;

namespace API_Dev_IT.Service
{
    public class RoomService : IRoom
    {
        private readonly BookingContext _context;
        private readonly ILogger<RoomService> _logger;
        public RoomService(BookingContext context, ILogger<RoomService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<T> Create<T>(Room room)
        {
            try
            {
                var rooms = await _context.room
                            .FirstOrDefaultAsync(x => 
                            x.RoomID == room.RoomID);

                if (rooms?.RoomID == room.RoomID)
                {
                    _logger.LogWarning($"Failed: room {room.RoomID}" +
                                      $" already exists");
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

                _logger.LogInformation(
                $"Tenant {insert.RoomID} created successfully");
                return (T)(object)insert;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during room registration");
                throw;
            }
        }

        public async Task<T> Update<T>(Room room, int id)
        {
            try
            {
                var rooms = await _context.room
                            .FirstOrDefaultAsync(x => 
                            x.RoomID == id);

                if (rooms?.RoomID != room.RoomID)
                {
                    _logger.LogWarning($"Failed: room {room.RoomID}" +
                                      $" already exists");
                    throw new InvalidOperationException(
                              "Invalid Room");
                }

                rooms.RoomNumber = room.RoomNumber;
                rooms.RoomType = room.RoomType;
                rooms.PricePerNight = room.PricePerNight;
                rooms.IsAvailable = room.IsAvailable;

                _context.Update(rooms);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                $"Tenant {id} updated successfully");
                return (T)(object)rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Room error occurred while updating {id}");
                throw;
            }
        }

        public async Task<T> Delete<T>(int id)
        {
            try
            {
                var room = await _context.room
                           .FirstOrDefaultAsync(x =>
                           x.RoomID == id);

                if (room is null || room.RoomID != id)
                {
                    _logger.LogWarning($"delete failed: tenant {id} not found");
                    throw new InvalidOperationException(
                              "Room doesnt exist");
                }

                _context.room.Remove(room);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"room {id} deleted successfully");
                return (T)(object)room;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Tenant error occurred while deleting {id}");
                throw;
            }
        }
    }
}
