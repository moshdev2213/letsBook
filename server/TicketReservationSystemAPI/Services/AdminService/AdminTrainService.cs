// File name: AdminTrainService.cs
// <summary>
// Description: Service class for admin train related operations
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using AutoMapper;
using MongoDB.Driver;
using System;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Enums;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public class AdminTrainService : IAdminTrainService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminTrainService> _logger;

        public AdminTrainService(
            DataContext context, 
            IMapper mapper, 
            ILogger<AdminTrainService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a train
        /// </summary>
        /// <param name="data">Create train data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with created train ID
        /// </returns>
        public async Task<ServiceResponse<string>> CreateTrain(AdminCreateTrain data)
        {
            ServiceResponse<string> response = new();

            try
            {
                if (string.IsNullOrWhiteSpace(data.Name))
                    return CreateErrorResponse(response, "Name is required");

                if (data.Type < 0 || data.Type > Enum.GetNames(typeof(TrainType)).Length)
                    return CreateErrorResponse(response, $"Train type can be from 0 to {Enum.GetNames(typeof(TrainType)).Length}");
                
                if (data.Seats <= 0)
                    return CreateErrorResponse(response, "Seats should be a positive number");

                if (string.IsNullOrWhiteSpace(data.DepartureStation))
                    return CreateErrorResponse(response, "Departure station is required");

                if (string.IsNullOrWhiteSpace(data.ArrivalStation))
                    return CreateErrorResponse(response, "Arrival station is required");

                Train train = new()
                {
                    Name = data.Name,
                    Type = (TrainType)data.Type,
                    Seats = data.Seats,
                    DepartureStation = data.DepartureStation.ToLower(),
                    ArrivalStation = data.ArrivalStation.ToLower()
                };

                await _context.Trains.InsertOneAsync(train);

                _logger.LogInformation($"Train created successfully. Train ID: {train.Id}");

                response.Data = train.Id.ToString();
                response.Success = true;
                response.Message = "Train created successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while creating the train");
            }
        }

        /// <summary>
        /// Get all trains
        /// </summary>
        /// <param name="activeStatus">Active status of the train</param>
        /// <param name="publishStatus">Publish status of the train</param>
        /// <param name="departureStation">Departure station</param>
        /// <param name="arrivalStation">Arrival station</param>
        /// <param name="date">Date of the train</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of trains
        /// </returns>
        public async Task<ServiceResponse<List<AdminGetTrain>>> GetTrains(
            bool activeStatus, 
            bool publishStatus, 
            string? departureStation, 
            string? arrivalStation,
            string? date)
        {
            ServiceResponse<List<AdminGetTrain>> response = new();

            try
            {
                var filterBuilder = Builders<Train>.Filter;
                var filter = filterBuilder.Empty;

                if (!activeStatus)
                {
                    filter &= filterBuilder.Eq(train => train.IsActive, false);
                }
                else
                {
                    filter &= filterBuilder.Eq(train => train.IsActive, true);

                    if (publishStatus)
                    {
                        // Active and Published Trains
                        filter &= filterBuilder.Eq(train => train.IsPublished, true);
                    }
                    else
                    {
                        // Active and Unpublished Trains
                        filter &= filterBuilder.Eq(train => train.IsPublished, false);
                    }
                }

                // TODO : Case insensitive search
                if (!string.IsNullOrWhiteSpace(departureStation))
                {
                    filter &= filterBuilder.Eq(train => train.DepartureStation, departureStation.ToLower());
                }

                if (!string.IsNullOrWhiteSpace(arrivalStation))
                {
                    filter &= filterBuilder.Eq(train => train.ArrivalStation, arrivalStation.ToLower());
                }

                if (!string.IsNullOrWhiteSpace(date))
                {
                    DateOnly scheduleDate = DateOnly.Parse(date);

                    filter &= filterBuilder.AnyEq(train => train.AvailableDates, scheduleDate);
                }

                var filteredTrains = await _context.Trains
                    .Find(filter)
                    .ToListAsync();

                response.Data = _mapper.Map<List<AdminGetTrain>>(filteredTrains);
                response.Success = true;
                response.Message = "Trains retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while getting the trains");
            }
        }

        /// <summary>
        /// Get a single train with schedules
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with train and schedules
        /// </returns>
        public async Task<ServiceResponse<AdminGetTrainWithSchedules>> GetTrain(string id)
        {
            ServiceResponse<AdminGetTrainWithSchedules> response = new();

            try
            {
                Guid trainId = new(id);

                Train train = await _context.Trains
                    .Find(t => t.Id == trainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                AdminGetTrainWithSchedules trainWithSchedules = _mapper.Map<AdminGetTrainWithSchedules>(train);

                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                List<TrainSchedule> schedules = await _context.TrainSchedules
                    .Find(t => t.TrainId == trainId && t.Date >= today)
                    .SortBy(t => t.Date)
                    .ThenBy(t => t.DepartureTime)
                    .ToListAsync();

                trainWithSchedules.Schedules = _mapper.Map<List<AdminGetTrainSchedule>>(schedules);

                response.Data = trainWithSchedules;
                response.Success = true;

                response.Message = (schedules.Count == 0) ? "No active schedules found" : "Train retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while getting the train");
            }
        }

        /// <summary>
        /// Update train active status
        /// </summary>
        /// <remarks>
        /// When train becomes inactive, all the active schedules are deleted
        /// </remarks>
        /// <param name="id">Train ID</param>
        /// <param name="status">Status, true (active) / false (inactive)</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated active status
        /// </returns>
        public async Task<ServiceResponse<bool>> UpdateActiveStatus(string id, bool status)
        {
            ServiceResponse<bool> response = new();

            try
            {
                Guid trainId = new(id);

                Train train = await _context.Trains
                    .Find(t => t.Id == trainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                if (!status)
                {
                    DateTime current = DateTime.Now;
                    DateOnly currentDay = DateOnly.FromDateTime(current);
                    TimeOnly currentTime = TimeOnly.FromDateTime(current);

                    List<TrainSchedule> schedules = await _context.TrainSchedules
                        .Find(s => s.TrainId == trainId && s.Date >= currentDay && s.DepartureTime >= currentTime)
                        .ToListAsync();

                    if (schedules.Count > 0)
                    {
                        // check for reservations in schedules from current day onwards
                        foreach (TrainSchedule schedule in schedules)
                        {
                            if (schedule.ReservationIDs.Any())
                                return CreateErrorResponse(response, "Cannot deactivate train with active reservations");
                        }

                        // from train.availableDates hash set, remove the dates from current day onwards
                        foreach (DateOnly date in train.AvailableDates)
                        {
                            if (date >= currentDay) train.AvailableDates.Remove(date);
                        }

                        // delete schedules
                        await _context.TrainSchedules.DeleteManyAsync(s => schedules.Contains(s));

                        // remove schedules IDs from train
                        foreach (TrainSchedule schedule in schedules)
                        {
                            train.ScheduleIDs.Remove(schedule.Id);
                        }
                    }

                    train.IsActive = false;
                    train.IsPublished = false;
                }
                else
                {
                    train.IsActive = true;
                }

                await _context.Trains.ReplaceOneAsync(x => x.Id == trainId, train);

                _logger.LogInformation($"Train {(train.IsActive ? "activated " : "deactivated ")} successfully. Train ID: {train.Id}");

                response.Data = train.IsActive;
                response.Success = true;
                response.Message = "Train " + (train.IsActive ? "activated " : "deactivated ") + "successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while updating the train status");
            }
        }

        /// <summary>
        /// Update IsPublished property of the train
        /// </summary>
        /// <remarks>
        /// All the active schedules are deleted when train is unpublished
        /// </remarks>
        /// <param name="id">Train ID</param>
        /// <param name="status">Status, true (publish) / false (unpublish)</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated publish status
        /// </returns>
        public async Task<ServiceResponse<bool>> UpdatePublishStatus(string id, bool status)
        {
            ServiceResponse<bool> response = new();

            try
            {
                Guid trainId = new(id);

                Train train = await _context.Trains
                    .Find(t => t.Id == trainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                if (status)
                {
                    if (!train.IsActive)
                        return CreateErrorResponse(response, "Cannot publish inactive train");

                    train.IsPublished = status;
                }
                else
                {
                    DateTime current = DateTime.Now;
                    DateOnly currentDay = DateOnly.FromDateTime(current);
                    TimeOnly currentTime = TimeOnly.FromDateTime(current);

                    List<TrainSchedule> schedules = await _context.TrainSchedules
                        .Find(s => s.TrainId == trainId && s.Date >= currentDay && s.DepartureTime >= currentTime)
                        .ToListAsync();

                    if (schedules.Count > 0)
                    {
                        foreach (TrainSchedule schedule in schedules)
                        {
                            if (schedule.ReservationIDs.Any())
                                return CreateErrorResponse(response, "Cannot unpublish train with active reservations");
                        }

                        // Remove all the active schedules
                        await _context.TrainSchedules.DeleteManyAsync(s => schedules.Contains(s));

                        foreach (TrainSchedule schedule in schedules)
                        {
                            train.ScheduleIDs.Remove(schedule.Id);
                        }
                    }

                    train.IsPublished = status;
                }

                await _context.Trains.ReplaceOneAsync(x => x.Id == trainId, train);

                _logger.LogInformation($"Train {(train.IsPublished ? "published " : "unpublished ")} successfully. Train ID: {train.Id}");

                response.Data = train.IsPublished;
                response.Success = true;
                response.Message = "Train " + (train.IsPublished ? "published " : "unpublished ") + "successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while updating the train publish status");
            }
        }

        /// <summary>
        /// Cancel train for a specific date
        /// </summary>
        /// <remarks>
        /// Can be cancelled only if no reservations are made for the train on the given date
        /// </remarks>
        /// <param name="id">Train ID</param>
        /// <param name="date">Date of cancellation</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with cancelled train ID
        /// </returns>
        public async Task<ServiceResponse<string>> CancelTrain(string id, AdminCancelTrain data)
        {
            ServiceResponse<string> response = new();

            try
            {
                Guid trainId = new(id);

                Train train = await _context.Trains
                    .Find(t => t.Id == trainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                {
                    response.Success = false;
                    response.Message = "Train not found";
                    return response;
                }

                DateOnly scheduleDate = DateOnly.Parse(data.Date);

                List<TrainSchedule> schedules = await _context.TrainSchedules
                    .Find(x => x.TrainId == trainId && x.Date == scheduleDate)
                    .ToListAsync();

                if (schedules.Count > 0)
                {
                    foreach (TrainSchedule schedule in schedules)
                    {
                        if (schedule.ReservationIDs.Any())
                            return CreateErrorResponse(response, "Cannot cancel train with active reservations");
                    }

                    await _context.TrainSchedules.DeleteManyAsync(t => schedules.Contains(t));

                    foreach (TrainSchedule schedule in schedules)
                    {
                        train.ScheduleIDs.Remove(schedule.Id);
                    }

                    train.AvailableDates.Remove(scheduleDate);

                    await _context.Trains.ReplaceOneAsync(x => x.Id == trainId, train);
                }

                _logger.LogInformation($"Train cancelled successfully. Train ID: {train.Id}");

                response.Data = train.Id.ToString();
                response.Success = true;
                response.Message = "Train cancelled successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while cancelling the train");
            }
        }

        /// <summary>
        /// Add a new schedule to a train
        /// </summary>
        /// <remarks>
        /// Unpublished trains become published automatically when a schedule is added
        /// </remarks>
        /// <param name="data">Schedule data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with created schedule ID
        /// </returns>
        public async Task<ServiceResponse<string>> AddSchedule(AdminAddSchedule data)
        {
            ServiceResponse<string> response = new();

            try
            {
                Guid trainId = new(data.TrainId);

                Train train = await _context.Trains
                    .Find(t => t.Id == trainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                if (string.IsNullOrWhiteSpace(data.Date)) 
                    return CreateErrorResponse(response, "Date is required");

                if (string.IsNullOrWhiteSpace(data.DepartureTime))
                    return CreateErrorResponse(response, "Departure time is required");

                if (string.IsNullOrWhiteSpace(data.ArrivalTime))
                    return CreateErrorResponse(response, "Arrival time is required");

                if (decimal.Parse(data.Price) <= 0)
                    return CreateErrorResponse(response, "Price should be a positive value");
                    
                TrainSchedule schedule = new()
                {
                    TrainId = trainId,
                    Date = DateOnly.Parse(data.Date),
                    DepartureTime = TimeOnly.Parse(data.DepartureTime),
                    ArrivalTime = TimeOnly.Parse(data.ArrivalTime),
                    AvailableSeats = train.Seats,
                    Price = decimal.Parse(data.Price)
                };

                train.ScheduleIDs.Add(schedule.Id);
                train.AvailableDates.Add(schedule.Date);

                if (!train.IsPublished) train.IsPublished = true;

                await _context.TrainSchedules.InsertOneAsync(schedule);
                await _context.Trains.ReplaceOneAsync(t => t.Id == trainId, train);

                _logger.LogInformation($"Schedule added successfully. Schedule ID: {schedule.Id}");

                response.Data = schedule.Id.ToString();
                response.Success = true;
                response.Message = "Schedule added successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while creating the schedule");
            }
        }

        /// <summary>
        /// Get a single train schedule using schedule ID
        /// </summary>
        /// <param name="id">schedule ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with train schedule
        /// </returns>
        public async Task<ServiceResponse<AdminGetTrainSchedule>> GetSchedule(string id)
        {
            ServiceResponse<AdminGetTrainSchedule> response = new();

            try
            {
                Guid scheduleId = new(id);

                TrainSchedule schedule = await _context.TrainSchedules
                    .Find(t => t.Id == scheduleId)
                    .FirstOrDefaultAsync();

                if (schedule == null)
                    return CreateErrorResponse(response, "Schedule not found");

                response.Data = _mapper.Map<AdminGetTrainSchedule>(schedule);
                response.Success = true;
                response.Message = "Schedule retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while getting the schedule");
            }
        }

        /// <summary>
        /// Update a train schedule
        /// </summary>
        /// <remarks>
        /// Can update only if schedule has no reservations
        /// </remarks>
        /// <param name="id">Schedule ID</param>
        /// <param name="data">Update schedule data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated schedule
        /// </returns>
        public async Task<ServiceResponse<AdminGetTrainSchedule>> UpdateSchedule(string id, AdminUpdateSchedule data)
        {
            ServiceResponse<AdminGetTrainSchedule> response = new();

            try
            {
                Guid scheduleId = new(id);

                TrainSchedule schedule =
                    await _context.TrainSchedules
                        .Find(t => t.Id == scheduleId)
                        .FirstOrDefaultAsync();

                if (schedule == null)
                    return CreateErrorResponse(response, "Schedule not found");

                // check if the schedule has reservations
                if (schedule.ReservationIDs.Any())
                    return CreateErrorResponse(response, "Cannot update schedule with active reservations");

                if (data.AvailableSeats < 0)
                    return CreateErrorResponse(response, "Available seats should be a positive value");

                Train train = await _context.Trains.Find(x => x.Id == schedule.TrainId).FirstOrDefaultAsync();

                if (data.AvailableSeats > train.Seats)
                    return CreateErrorResponse(response, $"Train has only {train.Seats} seats");

                if (data.Price != null && decimal.Parse(data.Price) <= 0)
                    return CreateErrorResponse(response, "Price should be a positive value");

                schedule.DepartureTime = data.DepartureTime != null ? TimeOnly.Parse(data.DepartureTime) : schedule.DepartureTime;
                schedule.ArrivalTime = data.ArrivalTime != null ? TimeOnly.Parse(data.ArrivalTime) : schedule.ArrivalTime;
                schedule.AvailableSeats = data.AvailableSeats ?? schedule.AvailableSeats;
                schedule.Price = data.Price != null ? decimal.Parse(data.Price) : schedule.Price;

                await _context.TrainSchedules.ReplaceOneAsync(x => x.Id == scheduleId, schedule);

                _logger.LogInformation($"Schedule updated successfully. Schedule ID: {schedule.Id}");

                response.Data = _mapper.Map<AdminGetTrainSchedule>(schedule);
                response.Success = true;
                response.Message = "Schedule updated successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while updating the schedule");
            }
        }

        /// <summary>
        /// Delete a train schedule
        /// </summary>
        /// <remarks>
        /// Can delete only if schedule has no reservations
        /// If no active schedules are present, train is unpublished automatically
        /// </remarks>
        /// <param name="id">Train ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with deleted schedule ID
        /// </returns>
        public async Task<ServiceResponse<string>> DeleteSchedule(string id)
        {
            ServiceResponse<string> response = new();

            try
            {
                Guid scheduleId = Guid.Parse(id);

                TrainSchedule schedule =
                    await _context.TrainSchedules
                        .Find(t => t.Id == scheduleId)
                        .FirstOrDefaultAsync();

                if (schedule == null)
                    return CreateErrorResponse(response, "Schedule not found");

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                // check if the schedule is in the past
                if (schedule.Date < currentDate || (schedule.Date == currentDate && schedule.DepartureTime < currentTime))
                    return CreateErrorResponse(response, "Cannot delete a passed schedule");

                // check if the schedule has reservations
                if (schedule.ReservationIDs.Any())
                    return CreateErrorResponse(response, "Cannot delete schedule with active reservations");

                Train train = await _context.Trains.Find(x => x.Id == schedule.TrainId).FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                train.ScheduleIDs.Remove(scheduleId);

                List<TrainSchedule> schedules = await _context.TrainSchedules
                    .Find(s => s.TrainId == train.Id && s.Date >= currentDate && s.DepartureTime >= currentTime)
                    .ToListAsync();

                if (schedules.Count == 0)
                {
                    train.IsPublished = false;
                }
                else
                {
                    List<TrainSchedule> schedulesForDeletedDate = schedules
                        .Where(s => s.Date == schedule.Date)
                        .ToList();

                    if (schedulesForDeletedDate.Count == 0) train.AvailableDates.Remove(schedule.Date);
                }

                await _context.TrainSchedules.DeleteOneAsync(x => x.Id == scheduleId);
                await _context.Trains.ReplaceOneAsync(x => x.Id == train.Id, train);

                _logger.LogInformation($"Schedule deleted successfully. Schedule ID: {schedule.Id}");

                response.Success = true;
                response.Message = "Schedule deleted successfully";
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while deleting the schedule");
            }
        }

        //private async Task<bool> HasActiveReservations(Guid trainId, List<TrainSchedule> schedules)
        //{
        //    if (schedules.Count > 0)
        //    {
        //        foreach (TrainSchedule schedule in schedules)
        //        {
        //            if (schedule.Reservations != null && schedule.Reservations.Any())
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}
        //private async Task<bool> HasActiveReservations(Guid trainId, List<TrainSchedule> schedules, DateOnly date)
        //{

        //    List<TrainSchedule> schedules = await _context.TrainSchedules.Find(x => x.TrainId == trainId && x.Date >= today).ToListAsync();

        //    if (schedules.Count > 0)
        //    {
        //        foreach (TrainSchedule schedule in schedules)
        //        {
        //            if (schedule.Reservations != null && schedule.Reservations.Any())
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// Helper method to create error response
        /// </summary>
        /// <typeparam name="T">Response.Data type</typeparam>
        /// <param name="response">Response</param>
        /// <param name="message">Error message</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with null and error message
        /// </returns>
        private static ServiceResponse<T> CreateErrorResponse<T>(ServiceResponse<T> response, string message)
        {
            response.Success = false;
            response.Message = message;
            return response;
        }
    }
}
