// File name: AdminTravelerService.cs
// <summary>
// Description: Service class for admin traveler related operations
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using System.Security.Cryptography;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;
using MongoDB.Driver;
using AutoMapper;
using System.Text.RegularExpressions;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public class AdminTravelerService : IAdminTravelerService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminTravelerService> _logger;

        public AdminTravelerService(
            DataContext context,
            IMapper mapper,
            ILogger<AdminTravelerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create account for a traveler
        /// </summary>
        /// <param name="data">Create account data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account NIC
        /// </returns>
        public async Task<ServiceResponse<string>> CreateAccount(AdminTravelerRegistration data)
        {
            ServiceResponse<string> response = new();

            try
            {
                if (await UserExistsNIC(data.NIC))
                    return CreateErrorResponse(response, "Account with this NIC already exists");

                if (!IsEmailValid(data.Email))
                    return CreateErrorResponse(response, "Invalid email address");
                if (await UserExistsEmail(data.Email))
                    return CreateErrorResponse(response, "Account with this email already exists");

                if (string.IsNullOrWhiteSpace(data.Name))
                    return CreateErrorResponse(response, "Name is required");

                if (string.IsNullOrWhiteSpace(data.ContactNo))
                    return CreateErrorResponse(response, "Contact number is required");
                if (data.ContactNo.Length != 10)
                    return CreateErrorResponse(response, "Invalid contact number");
                
                if (!IsPasswordValid(data.Password))
                    return CreateErrorResponse(response, "Invalid password");

                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                Traveler traveler = new()
                {
                    NIC = data.NIC,
                    Name = data.Name,
                    Email = data.Email,
                    ContactNo = data.ContactNo,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IsActive = true,
                };

                await _context.Travelers.InsertOneAsync(traveler);

                _logger.LogInformation($"Traveler account created. Traveler email: {traveler.Email}");

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
        /// Get all accounts
        /// </summary>
        /// <param name="status">Account status</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of accounts
        /// </returns>
        public async Task<ServiceResponse<List<AdminGetTraveler>>> GetAccounts(bool? status)
        {
            ServiceResponse<List<AdminGetTraveler>> response = new();

            List<Traveler> travelers;

            if (status == null)
            {
                travelers = await _context.Travelers
                    .Find(t => true)
                    .ToListAsync();
            }
            else
            {
                travelers = await _context.Travelers
                    .Find(t => t.IsActive == status)
                    .ToListAsync();
            }

            if (travelers.Count == 0)
            {
                response.Success = true;
                response.Message = "No travelers found";

                return response;
            }

            response.Data = _mapper.Map<List<AdminGetTraveler>>(travelers);
            response.Success = true;

            return response;
        }

        /// <summary>
        /// Get single account
        /// </summary>
        /// <param name="nic">Traveler NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account
        /// </returns>
        public async Task<ServiceResponse<AdminGetTravelerWithReservations>> GetAccount(string nic)
        {
            ServiceResponse<AdminGetTravelerWithReservations> response = new();

            try
            {
                Traveler traveler = await _context.Travelers
                    .Find(x => x.NIC.ToLower() == nic.ToLower())
                    .FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "Account not found");

                AdminGetTravelerWithReservations travelerWithReservations =
                    _mapper.Map<AdminGetTravelerWithReservations>(traveler);

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                var filterBuilder = Builders<Reservation>.Filter;
                var filter = filterBuilder.Empty;

                filter &= filterBuilder.Eq(reservation => reservation.TravelerId, nic);
                filter &= filterBuilder.Eq(reservation => reservation.IsCancelled, false);
                filter &= filterBuilder.Gte(reservation => reservation.ReservationDate, currentDate);
                filter &= filterBuilder.Gte(reservation => reservation.DepartureTime, currentTime);

                List<Reservation> reservations = await _context.Reservations
                    .Find(filter)
                    .SortBy(r => r.ReservationDate)
                    .ThenBy(r => r.DepartureTime)
                    .ToListAsync();

                List<AdminGetReservation> adminGetReservations = _mapper.Map<List<AdminGetReservation>>(reservations);

                travelerWithReservations.Reservations = adminGetReservations;

                response.Data = travelerWithReservations;
                response.Success = true;
                response.Message = "Account retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while getting traveler account");
            }
        }

        /// <summary>
        /// Update account
        /// </summary>
        /// <param name="nic">Traveler NIC</param>
        /// <param name="data">Account update data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated account
        /// </returns>
        public async Task<ServiceResponse<AdminGetTraveler>> UpdateAccount(string nic, AdminTravelerUpdate data)
        {
            ServiceResponse<AdminGetTraveler> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            if (data.Email != null)
            {
                traveler.Email = data.Email;
            }

            if (data.ContactNo != null)
            {
                traveler.ContactNo = data.ContactNo;
            }

            if (data.Password != null)
            {
                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                traveler.PasswordHash = passwordHash;
                traveler.PasswordSalt = passwordSalt;
            }

            await _context.Travelers.ReplaceOneAsync(x => x.NIC.ToLower() == nic.ToLower(), traveler);

            response.Data = _mapper.Map<AdminGetTraveler>(traveler);
            response.Success = true;
            response.Message = "Account updated successfully";

            return response;
        }

        /// <summary>
        /// Update account active status
        /// </summary>
        /// <param name="nic">Traveler NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated account
        /// </returns>
        public async Task<ServiceResponse<AdminGetTraveler>> UpdateActiveStatus(string nic)
        {
            ServiceResponse<AdminGetTraveler> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            traveler.IsActive = !traveler.IsActive;

            await _context.Travelers.ReplaceOneAsync(x => x.NIC.ToLower() == nic.ToLower(), traveler);

            response.Data = _mapper.Map<AdminGetTraveler>(traveler);
            response.Success = true;
            response.Message = "Account " + (traveler.IsActive ? "activated" : "deactivated") + " successfully";

            return response;
        }

        /// <summary>
        /// Delete account
        /// </summary>
        /// <param name="userId">Traveler NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with deleted account NIC
        /// </returns>
        public async Task<ServiceResponse<string>> DeleteAccount(string userId)
        {
            ServiceResponse<string> response = new();

            Traveler traveler = await _context.Travelers.Find(x => x.NIC.ToLower() == userId.ToLower()).FirstOrDefaultAsync();

            if (traveler == null)
            {
                response.Success = false;
                response.Message = "Account not found";
                return response;
            }

            await _context.Travelers.DeleteOneAsync(x => x.NIC.ToLower() == userId.ToLower());

            response.Success = true;
            response.Message = "Account deleted successfully";

            return response;
        }

        /// <summary>
        /// Helper method to validate email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Boolean result</returns>
        private static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string emailPattern = @"^\S+@\S+\.\S+$";

            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Helper method to validate password
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>Boolean result</returns>
        private static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // At least 8 characters
            // At least one uppercase letter
            // At least one lowercase letter
            // At least one digit
            // At least one special character (e.g., !@#$%^&*)
            string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&*!]).{8,}$";

            return Regex.IsMatch(password, passwordPattern);
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
