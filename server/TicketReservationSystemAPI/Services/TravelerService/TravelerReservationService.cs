﻿// File name: TravelerReservationService.cs
// <summary>
// Description: Service class for traveler's reservation related operations
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>12/10/2023</modified>

using AutoMapper;
using MongoDB.Driver;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public class TravelerReservationService : ITravelerReservationService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TravelerReservationService> _logger;

        public TravelerReservationService(
            DataContext context, 
            IMapper mapper,
            ILogger<TravelerReservationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a new reservation
        /// </summary>
        /// <param name="nic">Traveler NIC</param>
        /// <param name="data">Create reservation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation ID or null
        /// </returns>
        public async Task<ServiceResponse<string>> CreateReservation(string nic, TravelerCreateReservation data)
        {
            ServiceResponse<string> response = new();

            try
            {
                Traveler traveler = await _context.Travelers
                    .Find(t => t.NIC == nic)
                    .FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "Traveler not found");

                Guid scheduleId = new(data.ScheduleId);

                TrainSchedule schedule = await _context.TrainSchedules
                    .Find(s => s.Id == scheduleId).
                    FirstOrDefaultAsync();

                if (schedule == null)
                    return CreateErrorResponse(response, "Schedule not found");

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                // Check if reservation date within 30 days of the booking date
                if (currentDate.AddDays(30) < schedule.Date)
                    return CreateErrorResponse(response, "Reservation can be created only within 30 days of the scheduled date");

                // Check if schedule date has passed
                if (schedule.Date < currentDate)
                    return CreateErrorResponse(response, "Schedule date has passed");

                // Check if schedule time has passed
                if (schedule.Date == currentDate && schedule.DepartureTime <= currentTime)
                    return CreateErrorResponse(response, "Schedule time has passed");

                // Check if seat number is valid
                if (data.Seats < 1 || data.Seats > 4)
                    return CreateErrorResponse(response, "Seat number must be from 1 t0 4");

                Train train = await _context.Trains
                    .Find(t => t.Id == schedule.TrainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                Reservation reservation = new()
                {
                    TravelerId = nic,
                    TrainId = schedule.TrainId,
                    ScheduleId = schedule.Id,
                    Seats = data.Seats,
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    ReservationDate = schedule.Date,
                    DepartureTime = schedule.DepartureTime,
                    ArrivalTime = schedule.ArrivalTime,
                };

                await _context.Reservations.InsertOneAsync(reservation);

                // Update available seats
                schedule.AvailableSeats -= data.Seats;
                schedule.ReservationIDs.Add(reservation.Id);

                await _context.TrainSchedules.ReplaceOneAsync(s => s.Id == schedule.Id, schedule);

                // Add the reservation to the traveler
                traveler.ReservationIDs.Add(reservation.Id);

                await _context.Travelers.ReplaceOneAsync(t => t.NIC == traveler.NIC, traveler);

                response.Data = reservation.Id.ToString();
                response.Success = true;
                response.Message = "Reservation created successfully";

                _logger.LogInformation($"Reservation created. Reservation ID: {reservation.Id}");

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return CreateErrorResponse(response, "Error occurred while creating the reservation");
            }
        }

        /// <summary>
        /// Get all the reservations of a traveler
        /// </summary>
        /// <param name="nic">Traveler NIC</param>
        /// <param name="past"></param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of reservations or null
        /// </returns>
        public async Task<ServiceResponse<List<TravelerGetReservation>>> GetReservations(string nic, bool past)
        {
            ServiceResponse<List<TravelerGetReservation>> response = new();

            try
            {
                var filterBuilder = Builders<Reservation>.Filter;
                var filter = filterBuilder.Eq(r => r.TravelerId, nic);

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                List<Reservation> reservations;

                if (past)
                {
                    filter &= filterBuilder.Eq(r => r.ReservationDate, currentDate);
                    filter &= filterBuilder.Lt(r => r.DepartureTime, currentTime);

                    filter |= filterBuilder.Lt(r => r.ReservationDate, currentDate);

                    reservations = await _context.Reservations
                        .Find(filter)
                        .SortByDescending(r => r.ReservationDate)
                        .ThenByDescending(r => r.DepartureTime)
                        .ToListAsync();
                }
                else
                {
                    filter &= filterBuilder.Eq(r => r.ReservationDate, currentDate);
                    filter &= filterBuilder.Gte(r => r.DepartureTime, currentTime);

                    filter|= filterBuilder.Gt(r => r.ReservationDate, currentDate);

                    reservations = await _context.Reservations
                        .Find(filter)
                        .SortBy(r => r.ReservationDate)
                        .ThenBy(r => r.DepartureTime)
                        .ToListAsync();
                }

                List<TravelerGetReservation> data = _mapper.Map<List<TravelerGetReservation>>(reservations);

                response.Data = data;
                response.Success = true;
                response.Message = "Reservations retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving the reservations");
            }
        }

        /// <summary>
        /// Get single reservation
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation or null
        /// </returns>
        public async Task<ServiceResponse<TravelerGetReservation>> GetReservation(string id)
        {
            ServiceResponse<TravelerGetReservation> response = new();

            try
            {
                Guid reservationId = new(id);

                Reservation reservation = await _context.Reservations
                    .Find(r => r.Id == reservationId)
                    .FirstOrDefaultAsync();

                if (reservation == null)
                    return CreateErrorResponse(response, "Reservation not found");

                TravelerGetReservation data = _mapper.Map<TravelerGetReservation>(reservation);

                response.Data = data;
                response.Success = true;
                response.Message = "Reservation retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving the reservation");
            }
        }

        /// <summary>
        /// Update a reservation
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <param name="data">Update reservation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated reservation or null
        /// </returns>
        public async Task<ServiceResponse<TravelerGetReservation>> UpdateReservation(string id, TravelerUpdateReservation data)
        {
            ServiceResponse<TravelerGetReservation> response = new();

            try
            {
                Guid reservationId = new(id);

                Reservation reservation = await _context.Reservations.Find(r => r.Id == reservationId).FirstOrDefaultAsync();

                if (reservation == null)
                    return CreateErrorResponse(response, "Reservation not found");

                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                // Check if the reservation has passed
                if (reservation.ReservationDate < currentDate)
                    return CreateErrorResponse(response, "Reservation has passed");

                if (reservation.ReservationDate == currentDate)
                {
                    TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);

                    // Check if the reservation time has passed
                    if (reservation.DepartureTime < currentTime)
                        return CreateErrorResponse(response, "Reservation time has passed");
                }

                // Check if the update is made at least 5 days prior to the reservation date
                if (currentDate.AddDays(5) > reservation.ReservationDate)
                    return CreateErrorResponse(response, "Reservations can only be updated at least 5 days prior to reservation date");

                // Check if seat number is valid
                if (data.Seats < 1 || data.Seats > 4)
                    return CreateErrorResponse(response, "Seat number must be from 1 t0 4");

                if (data.Seats != reservation.Seats)
                {
                    TrainSchedule schedule = await _context.TrainSchedules.Find(s => s.Id == reservation.ScheduleId).FirstOrDefaultAsync();

                    if (data.Seats > reservation.Seats)
                    {
                        // check if required additional seats doesn't exceed the available seats for that schedule
                        if ((data.Seats - reservation.Seats) > schedule.AvailableSeats)
                            return CreateErrorResponse(response, $"Only {schedule.AvailableSeats} extra seats can be booked");

                        // Update available seats
                        schedule.AvailableSeats -= (data.Seats - reservation.Seats);
                    }
                    else
                    {
                        // Update available seats
                        schedule.AvailableSeats += (reservation.Seats - data.Seats);
                    }

                    reservation.Seats = data.Seats;

                    await _context.Reservations.ReplaceOneAsync(r => r.Id == reservation.Id, reservation);

                    await _context.TrainSchedules.ReplaceOneAsync(s => s.Id == schedule.Id, schedule);
                }

                TravelerGetReservation travelerGetReservation = _mapper.Map<TravelerGetReservation>(reservation);

                _logger.LogInformation($"Reservation updated. Reservation ID: {reservation.Id}");

                response.Data = travelerGetReservation;
                response.Success = true;
                response.Message = "Reservation updated successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while updating the reservation");
            }
        }

        /// <summary>
        /// Cancel reservation
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with cancelled reservation ID or null
        /// </returns>
        public async Task<ServiceResponse<string>> CancelReservation(string id)
        {
            ServiceResponse<string> response = new();

            try
            {
                Guid reservationId = new(id);

                Reservation reservation =
                    await _context.Reservations.Find(r => r.Id == reservationId).FirstOrDefaultAsync();

                if (reservation == null)
                    return CreateErrorResponse(response, "Reservation not found");

                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                // Check if the reservation has passed
                if (reservation.ReservationDate < currentDate)
                    return CreateErrorResponse(response, "Reservation has passed");

                if (reservation.ReservationDate == currentDate)
                {
                    TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);

                    // Check if the reservation time has passed
                    if (reservation.DepartureTime < currentTime)
                        return CreateErrorResponse(response, "Reservation time has passed");
                }

                // Check if the cancellation is made at least 5 days prior to the reservation date
                if (currentDate.AddDays(5) > reservation.ReservationDate)
                    return CreateErrorResponse(response,
                        "Reservations can only be cancelled at least 5 days prior to reservation date");

                reservation.IsCancelled = true;

                await _context.Reservations.ReplaceOneAsync(r => r.Id == reservation.Id, reservation);

                TrainSchedule schedule = await _context.TrainSchedules
                    .Find(s => s.Id == reservation.ScheduleId)
                    .FirstOrDefaultAsync();

                // Update available seats
                schedule.AvailableSeats += reservation.Seats;
                schedule.ReservationIDs.Remove(reservation.Id);

                await _context.TrainSchedules.ReplaceOneAsync(s => s.Id == schedule.Id, schedule);

                // Remove the reservation from the traveler
                Traveler traveler = await _context.Travelers
                    .Find(t => t.NIC == reservation.TravelerId)
                    .FirstOrDefaultAsync();

                traveler.ReservationIDs.Remove(reservation.Id);

                await _context.Travelers.ReplaceOneAsync(t => t.NIC == traveler.NIC, traveler);

                _logger.LogInformation($"Reservation cancelled. Reservation ID: {reservation.Id}");

                response.Data = reservation.Id.ToString();
                response.Success = true;
                response.Message = "Reservation cancelled successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while cancelling the reservation");
            }
        }

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
