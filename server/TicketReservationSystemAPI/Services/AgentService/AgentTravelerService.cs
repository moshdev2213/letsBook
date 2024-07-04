// File name: AgentTravelerService.cs
// <summary>
// Description: Service class for travel agent's traveler related operations.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using AutoMapper;
using MongoDB.Driver;
using System.Security.Cryptography;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;
using TicketReservationSystemAPI.Models.Other.Agent;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public class AgentTravelerService : IAgentTravelerService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentTravelerService> _logger;

        public AgentTravelerService(
            DataContext context, 
            IMapper mapper, 
            ILogger<AgentTravelerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create traveler account
        /// </summary>
        /// <param name="data">Create account data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with created account ID or null
        /// </returns>
        public async Task<ServiceResponse<string>> CreateAccount(AgentTravelerRegistration data)
        {
            ServiceResponse<string> response = new();

            try
            {
                if (await UserExistsNIC(data.NIC))
                    return CreateErrorResponse(response, "Account with this NIC already exists");

                if (await UserExistsEmail(data.Email))
                    return CreateErrorResponse(response, "Account with this email already exists");

                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                Traveler traveler = new()
                {
                    NIC = data.NIC,
                    Name = data.Name,
                    Email = data.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IsActive = true,
                };

                await _context.Travelers.InsertOneAsync(traveler);

                response.Success = true;
                response.Message = "Registration successful";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while creating traveler account");
            }
        }

        /// <summary>
        /// Get traveler account with reservations
        /// </summary>
        /// <param name="travelerId">Traveler NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account and reservations or null
        /// </returns>
        public async Task<ServiceResponse<AgentGetTravelerWithReservations>> GetAccount(string travelerId)
        {
            ServiceResponse<AgentGetTravelerWithReservations> response = new();

            try
            {
                Traveler traveler = await _context.Travelers
                    .Find(t => t.NIC.ToLower() == travelerId.ToLower())
                    .FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "Account not found");

                AgentGetTravelerWithReservations travelerWithReservations = _mapper.Map<AgentGetTravelerWithReservations>(traveler);

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                var filterBuilder = Builders<Reservation>.Filter;
                var filter = filterBuilder.Empty;

                filter &= filterBuilder.Eq(reservation => reservation.TravelerId, travelerId);
                filter &= filterBuilder.Eq(reservation => reservation.IsCancelled, false);
                filter &= filterBuilder.Gte(reservation => reservation.ReservationDate, currentDate);
                filter &= filterBuilder.Gte(reservation => reservation.DepartureTime, currentTime);

                List<Reservation> reservations = await _context.Reservations
                    .Find(filter)
                    .SortBy(r => r.ReservationDate)
                    .ThenBy(r => r.DepartureTime)
                    .ToListAsync();

                List<AdminGetReservation> adminGetReservations = _mapper.Map<List<AdminGetReservation>>(reservations);

                response.Data = travelerWithReservations;
                response.Success = true;

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving traveler account");
            }
        }

        /// <summary>
        /// Update traveler account
        /// </summary>
        /// <param name="travelerId">Traveler NIC</param>
        /// <param name="data">Update account data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated account or null
        /// </returns>
        public async Task<ServiceResponse<AgentGetTraveler>> UpdateAccount(string travelerId, AgentTravelerUpdate data)
        {
            ServiceResponse<AgentGetTraveler> response = new();

            try
            {
                Traveler traveler = await _context.Travelers
                    .Find(x => x.NIC.ToLower() == travelerId.ToLower())
                    .FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "Account not found");

                if (!string.IsNullOrEmpty(data.Email)) traveler.Email = data.Email;
                if (!string.IsNullOrEmpty(data.ContactNo)) traveler.ContactNo = data.ContactNo;

                if (!string.IsNullOrEmpty(data.Password))
                {
                    CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    // check if new password is same as previous password
                    if (VerifyPasswordHash(data.Password, traveler.PasswordHash, traveler.PasswordSalt))
                        return CreateErrorResponse(response, "New password cannot be the same as the previous password");

                    traveler.PasswordHash = passwordHash;
                    traveler.PasswordSalt = passwordSalt;
                }

                await _context.Travelers.ReplaceOneAsync(x => x.NIC.ToLower() == travelerId.ToLower(), traveler);

                response.Data = _mapper.Map<AgentGetTraveler>(traveler);
                response.Success = true;
                response.Message = "Account updated successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while updating traveler account");
            }
        }

        /// <summary>
        /// Delete traveler account
        /// </summary>
        /// <param name="userId">Traveler ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with deleted account ID or null
        /// </returns>
        public async Task<ServiceResponse<string>> DeleteAccount(string userId)
        {
            ServiceResponse<string> response = new();

            try
            {
                Traveler traveler = await _context.Travelers
                    .Find(x => x.NIC.ToLower() == userId.ToLower())
                    .FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "Account not found");

                await _context.Travelers.DeleteOneAsync(x => x.NIC.ToLower() == userId.ToLower());

                response.Data = traveler.NIC;
                response.Success = true;
                response.Message = "Account deleted successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while deleting traveler account");
            }
        }

        /// <summary>
        /// Helper method to verify NIC
        /// </summary>
        /// <param name="nic">Traveler NIC</param>
        /// <returns>Boolean result</returns>
        public async Task<bool> UserExistsNIC(string nic)
        {
            if (await _context.Travelers.Find(t => t.NIC.ToLower() == nic.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to verify email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Boolean result</returns>
        public async Task<bool> UserExistsEmail(string email)
        {
            if (await _context.Travelers.Find(t => t.Email.ToLower() == email.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to create password hash and salt
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="passwordHash">Password hash</param>
        /// <param name="passwordSalt">Password salt</param>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// Helper method to verify password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="passwordHash">Password hash</param>
        /// <param name="passwordSalt">Password salt</param>
        /// <returns></returns>
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(passwordHash);
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
