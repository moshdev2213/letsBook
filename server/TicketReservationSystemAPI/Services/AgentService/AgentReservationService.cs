// File name: AgentReservationService.cs
// <summary>
// Description: Service class for agent's reservation related operations
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using AutoMapper;
using MongoDB.Driver;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Agent;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public class AgentReservationService : IAgentReservationService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentReservationService> _logger;

        public AgentReservationService(
            DataContext context, 
            IMapper mapper, 
            ILogger<AgentReservationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create a reservation for traveler
        /// </summary>
        /// <param name="data">Create reservation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation ID or null
        /// </returns>
        public async Task<ServiceResponse<string>> CreateReservation(AgentCreateReservation data)
        {
            ServiceResponse<string> response = new();

            try
            {
                Traveler traveler = await _context.Travelers
                    .Find(t => t.NIC == data.NIC)
                    .FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "Traveler not found");

                Guid scheduleId = new(data.ScheduleId);

                TrainSchedule schedule = await _context.TrainSchedules
                    .Find(s => s.Id == scheduleId)
                    .FirstOrDefaultAsync();

                if (schedule == null)
                    return CreateErrorResponse(response, "Schedule not found");

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                // Check if schedule date has passed
                if (schedule.Date < currentDate)
                    return CreateErrorResponse(response, "Schedule date has passed");

                // Check if schedule time has passed
                if (schedule.Date == currentDate && schedule.DepartureTime <= currentTime)
                    return CreateErrorResponse(response, "Schedule time has passed");

                // Check if reservation date within 30 days of the booking date
                if (currentDate.AddDays(30) < schedule.Date)
                    return CreateErrorResponse(response, "Reservation can be created only within 30 days of the scheduled date");

                // Check if seat number is valid
                if (data.Seats < 1 || data.Seats > 4)
                    return CreateErrorResponse(response, "Seat number must be from 1 t0 4");

                Train train = await _context.Trains.Find(t => t.Id == schedule.TrainId).FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                // Check if enough seats are available for reservation
                if (data.Seats > schedule.AvailableSeats)
                    return CreateErrorResponse(response, $"Only {schedule.AvailableSeats} are available");

                Reservation reservation = new()
                {
                    TravelerId = traveler.NIC,
                    TrainId = schedule.TrainId,
                    ScheduleId = schedule.Id,
                    Seats = data.Seats,
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    ReservationDate = schedule.Date,
                    DepartureTime = schedule.DepartureTime,
                    ArrivalTime = schedule.ArrivalTime,
                };

                // Update available seats
                schedule.AvailableSeats -= data.Seats;
                schedule.ReservationIDs.Add(reservation.Id);

                // Add the reservation to the traveler
                traveler.ReservationIDs.Add(reservation.Id);

                await _context.Reservations.InsertOneAsync(reservation);

                await _context.TrainSchedules.ReplaceOneAsync(s => s.Id == schedule.Id, schedule);
                await _context.Travelers.ReplaceOneAsync(t => t.NIC == traveler.NIC, traveler);

                _logger.LogInformation($"Reservation created. Reservation ID: {reservation.Id}");

                response.Data = reservation.Id.ToString();
                response.Success = true;
                response.Message = "Reservation created successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while creating the reservation");
            }
        }

        /// <summary>
        /// Get single reservation
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation or null
        /// </returns>
        public async Task<ServiceResponse<AgentGetReservation>> GetReservation(string id)
        {
            ServiceResponse<AgentGetReservation> response = new();

            try
            {
                Guid reservationId = new(id);

                Reservation reservation = await _context.Reservations
                    .Find(r => r.Id == reservationId)
                    .FirstOrDefaultAsync();

                if (reservation == null)
                    return CreateErrorResponse(response, "Reservation not found");

                AgentGetReservation agentGetReservation = _mapper.Map<AgentGetReservation>(reservation);

                response.Data = agentGetReservation;
                response.Success = true;

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving the account");
            }
        }

        /// <summary>
        /// Update reservation
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <param name="data">Update reservation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated reservation or null
        /// </returns>
        public async Task<ServiceResponse<AgentGetReservation>> UpdateReservation(string id, AgentUpdateReservation data)
        {
            ServiceResponse<AgentGetReservation> response = new();

            try
            {
                Guid reservationId = new(id);

                Reservation reservation = await _context.Reservations
                    .Find(r => r.Id == reservationId)
                    .FirstOrDefaultAsync();

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
                        return CreateErrorResponse(response, "Reservation has passed");
                }

                // Check if the update is made at least 5 days prior to the reservation date
                if (currentDate.AddDays(5) > reservation.ReservationDate)
                    return CreateErrorResponse(response, "Reservations can only be updated at least 5 days prior to reservation date");

                // Check if seat number is valid
                if (data.Seats < 1 || data.Seats > 4)
                    return CreateErrorResponse(response, "Seat number must be from 1 t0 4");

                if (data.Seats != reservation.Seats)
                {
                    TrainSchedule schedule = await _context.TrainSchedules
                        .Find(s => s.Id == reservation.ScheduleId)
                        .FirstOrDefaultAsync();

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

                AgentGetReservation agentGetReservation = _mapper.Map<AgentGetReservation>(reservation);

                _logger.LogInformation($"Reservation updated. Reservation ID: {reservation.Id}");

                response.Data = agentGetReservation;
                response.Success = true;
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

                Reservation reservation = await _context.Reservations
                        .Find(r => r.Id == reservationId)
                        .FirstOrDefaultAsync();

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

                TrainSchedule schedule = await _context.TrainSchedules
                    .Find(s => s.Id == reservation.ScheduleId)
                    .FirstOrDefaultAsync();

                // Update available seats
                schedule.AvailableSeats += reservation.Seats;
                schedule.ReservationIDs.Remove(reservation.Id);

                // Remove the reservation from the traveler
                Traveler traveler = await _context.Travelers
                    .Find(t => t.NIC == reservation.TravelerId)
                    .FirstOrDefaultAsync();

                traveler.ReservationIDs.Remove(reservation.Id);

                await _context.Reservations.ReplaceOneAsync(r => r.Id == reservation.Id, reservation);
                await _context.TrainSchedules.ReplaceOneAsync(s => s.Id == schedule.Id, schedule);
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
        /// Response with error message
        /// </returns>
        private static ServiceResponse<T> CreateErrorResponse<T>(ServiceResponse<T> response, string message)
        {
            response.Success = false;
            response.Message = message;

            return response;
        }
    }
}
