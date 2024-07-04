// File name: TravelerService.cs
// <summary>
// Description: Service class for traveler related operations
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.enums;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public class TravelerService : ITravelerService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TravelerService> _logger;

        public TravelerService(
            DataContext context, 
            IMapper mapper, 
            ILogger<TravelerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Traveler login
        /// </summary>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with authorization token or null
        /// </returns>
        public async Task<ServiceResponse<string>> Login(TravelerLogin data)
        {
            ServiceResponse<string> response = new();

            try
            {
                Traveler traveler = await _context.Travelers
                    .Find(x => x.Email.ToLower() == data.Email.ToLower())
                    .FirstOrDefaultAsync();

                if (traveler == null || !VerifyPasswordHash(data.Password, traveler.PasswordHash, traveler.PasswordSalt))
                    return CreateErrorResponse(response, "Invalid email or password");

                if (!traveler.IsActive)
                    return CreateErrorResponse(response, "User account is deactivated");

                response.Data = CreateToken(traveler);
                response.Success = true;
                response.Message = "Login successful";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while logging in");
            }
        }

        /// <summary>
        /// Traveler register
        /// </summary>
        /// <param name="data">Register data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with null
        /// </returns>
        public async Task<ServiceResponse<string>> Register(TravelerRegistration data)
        {
            ServiceResponse<string> response = new();

            try
            {
                if (await UserExistsNIC(data.NIC))
                    return CreateErrorResponse(response, "Account with the NIC already exists");

                if (await UserExistsEmail(data.Email))
                    return CreateErrorResponse(response, "Account with the email already exists");

                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                Traveler traveler = new()
                {
                    NIC = data.NIC,
                    Name = data.Name,
                    Email = data.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    ContactNo = data.ContactNo,
                    IsActive = true
                };

                await _context.Travelers.InsertOneAsync(traveler);

                response.Success = true;
                response.Message = "User created successfully";

                _logger.LogInformation($"User registered. User ID { traveler.NIC }");

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred during registration");
            }
        }

        /// <summary>
        /// Get traveler account
        /// </summary>
        /// <param name="userId">User NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account or null
        /// </returns>
        public async Task<ServiceResponse<TravelerReturn>> GetAccount(string userId)
        {
            ServiceResponse<TravelerReturn> response = new();

            try
            {
                Traveler traveler = await _context.Travelers.Find(x => x.NIC == userId).FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "User not found");

                TravelerReturn travelerReturn = _mapper.Map<TravelerReturn>(traveler);

                // Get the list of active reservations
                List<Reservation> reservations = await _context.Reservations
                    .Find(x => x.TravelerId == userId && x.IsCancelled == false)
                    .ToListAsync();

                List<TravelerGetReservation> travelerReservations = _mapper.Map<List<TravelerGetReservation>>(reservations);

                travelerReturn.Reservations = travelerReservations;

                response.Data = travelerReturn;
                response.Success = true;

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Internal server error");
            }
        }

        /// <summary>
        /// Update traveler account
        /// </summary>
        /// <param name="userId">User NIC</param>
        /// <param name="data">Update data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated account or null
        /// </returns>
        public async Task<ServiceResponse<TravelerReturn>> UpdateAccount(string userId, TravelerUpdate data)
        {
            ServiceResponse<TravelerReturn> response = new();

            try
            {
                Traveler traveler = await _context.Travelers.Find(x => x.NIC == userId).FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "User not found");

                if (!string.IsNullOrEmpty(data.Name)) traveler.Name = data.Name;
                if (!string.IsNullOrEmpty(data.ContactNo)) traveler.ContactNo = data.ContactNo;

                if (!string.IsNullOrEmpty(data.PreviousPassword) && !string.IsNullOrEmpty(data.Password))
                {
                    if (data.PreviousPassword == data.Password)
                        return CreateErrorResponse(response, "New password cannot be the same as the previous password");

                    if (!VerifyPasswordHash(data.PreviousPassword, traveler.PasswordHash, traveler.PasswordSalt)) ;

                    CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    traveler.PasswordHash = passwordHash;
                    traveler.PasswordSalt = passwordSalt;
                }

                await _context.Travelers.ReplaceOneAsync(x => x.NIC == userId, traveler);

                TravelerReturn travelerReturn = _mapper.Map<TravelerReturn>(traveler);

                response.Data = travelerReturn;
                response.Success = true;
                response.Message = "User updated successfully";

                _logger.LogInformation($"Traveler updated. Traveler ID { traveler.NIC }");

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Internal server error");
            }
        }

        /// <summary>
        /// Deactivate traveler account
        /// </summary>
        /// <param name="userId">User NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with boolean status of account or null
        /// </returns>
        public async Task<ServiceResponse<bool>> DeactivateAccount(string userId)
        {
            ServiceResponse<bool> response = new();

            try
            {
                Traveler traveler = await _context.Travelers.Find(x => x.NIC == userId).FirstOrDefaultAsync();

                if (traveler == null)
                    return CreateErrorResponse(response, "User not found");

                traveler.IsActive = false;

                await _context.Travelers.ReplaceOneAsync(x => x.NIC == userId, traveler);

                response.Success = true;
                response.Message = "User account deactivated successfully";

                _logger.LogInformation($"User deactivated. User ID {traveler.NIC}");

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Internal server error");
            }
        }

        /// <summary>
        /// Helper method to check if the user exists using NIC
        /// </summary>
        /// <param name="nic">User NIC</param>
        /// <returns>Boolean status</returns>
        private async Task<bool> UserExistsNIC(string nic)
        {
            if (await _context.Travelers.Find(x => x.NIC.ToLower() == nic.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to check if the user exists using email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>Boolean status</returns>
        private async Task<bool> UserExistsEmail(string email)
        {
            if (await _context.Travelers.Find(x => x.Email.ToLower() == email.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to create password hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// Helper method to verify password hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns>Boolean status</returns>
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// Helper method to create JWT token
        /// </summary>
        /// <param name="traveler"></param>
        /// <returns>JWT token</returns>
        private string CreateToken(Traveler traveler)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, traveler.NIC),
                new Claim(ClaimTypes.Name, traveler.Email),
                new Claim(ClaimTypes.Role, SystemRole.Traveler.ToString())
            };
            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(JWTSettings.Token));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
