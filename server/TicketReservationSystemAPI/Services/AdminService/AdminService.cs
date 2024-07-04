// File name: AdminService.cs
// <summary>
// Description: Service class for back-office operations.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.enums;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            DataContext context, 
            IMapper mapper, 
            ILogger<AdminService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Back-Office login
        /// </summary>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with token or null
        /// </returns>
        public async Task<ServiceResponse<string>> Login(AdminLogin data)
        {
            ServiceResponse<string> response = new();

            try
            {
                if (string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.Password))
                    return CreateErrorResponse(response, "Email and password are required");

                Admin admin = await _context.Admins
                    .Find(a => a.Email.ToLower() == data.Email.ToLower())
                    .FirstOrDefaultAsync();

                if (admin == null || !VerifyPasswordHash(data.Password, admin.PasswordHash, admin.PasswordSalt))
                    return CreateErrorResponse(response, "Invalid email or password");

                response.Data = CreateToken(admin);
                response.Success = true;
                response.Message = "Login successful";

                _logger.LogInformation($"Admin login. Account email: {admin.Email}");

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while logging in");
            }
        }

        /// <summary>
        /// Back-Office registration
        /// </summary>
        /// <param name="data">Registration data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with null
        /// </returns>
        public async Task<ServiceResponse<string>> Register(AdminRegistration data)
        {
            ServiceResponse<string> response = new();

            try
            {
                if (!IsEmailValid(data.Email))
                    return CreateErrorResponse(response, "Invalid email");
                if (await UserExistsEmail(data.Email))
                    return CreateErrorResponse(response, "Email already exists");

                if (!IsPasswordValid(data.Password))
                    return CreateErrorResponse(response, "Invalid password");

                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                if (string.IsNullOrWhiteSpace(data.Name))
                    return CreateErrorResponse(response, "Name is required");

                if (string.IsNullOrWhiteSpace(data.ContactNo))
                    return CreateErrorResponse(response, "Contact number is required");
                if (data.ContactNo.Length != 10)
                    return CreateErrorResponse(response, "Invalid contact number");

                Admin admin = new()
                {
                    Name = data.Name,
                    Email = data.Email,
                    ContactNo = data.ContactNo,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                };

                await _context.Admins.InsertOneAsync(admin);

                _logger.LogInformation($"Admin registration. Account email: {admin.Email}");

                response.Success = true;
                response.Message = "Account created successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred during registration");
            }
        }

        /// <summary>
        /// Get back-office account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account or null
        /// </returns>
        public async Task<ServiceResponse<AdminReturn>> GetAccount(string userId)
        {
            ServiceResponse<AdminReturn> response = new();

            try
            {
                Admin admin = await _context.Admins
                    .Find(a => a.Id.ToString() == userId)
                    .FirstOrDefaultAsync();

                if (admin == null)
                    return CreateErrorResponse(response, "User not found");

                AdminReturn adminReturn = _mapper.Map<AdminReturn>(admin);

                response.Data = adminReturn;
                response.Success = true;
                response.Message = "Account retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving account");
            }
        }

        /// <summary>
        /// Update back-office account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="data">Account update data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated account or null
        /// </returns>
        public async Task<ServiceResponse<AdminReturn>> UpdateAccount(string userId, AdminUpdate data)
        {
            ServiceResponse<AdminReturn> response = new();

            try
            {
                Admin admin = await _context.Admins
                    .Find(a => a.Id.ToString() == userId)
                    .FirstOrDefaultAsync();

                if (admin == null)
                    return CreateErrorResponse(response, "User not found");

                if (!string.IsNullOrWhiteSpace(data.Name)) admin.Name = data.Name;
                if (!string.IsNullOrWhiteSpace(data.ContactNo)) admin.ContactNo = data.ContactNo;

                if (!string.IsNullOrWhiteSpace(data.PreviousPassword) && !string.IsNullOrWhiteSpace(data.Password))
                {
                    if (!VerifyPasswordHash(data.PreviousPassword, admin.PasswordHash, admin.PasswordSalt))
                        return CreateErrorResponse(response, "Invalid previous password");

                    CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    admin.PasswordHash = passwordHash;
                    admin.PasswordSalt = passwordSalt;
                }

                await _context.Admins.ReplaceOneAsync(x => x.Id == admin.Id, admin);

                _logger.LogInformation($"Account updated successfully. Account email: {admin.Email}");

                AdminReturn adminReturn = _mapper.Map<AdminReturn>(admin);

                response.Data = adminReturn;
                response.Success = true;
                response.Message = "Account updated successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while updating account");
            }
        }

        /// <summary>
        /// Delete back-office account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with deleted user iD
        /// </returns>
        public async Task<ServiceResponse<string>> DeleteAccount(string userId)
        {
            ServiceResponse<string> response = new();

            try
            {
                Admin admin = await _context.Admins.Find(x => x.Id.ToString() == userId).FirstOrDefaultAsync();

                if (admin == null)
                    return CreateErrorResponse(response, "User not found");

                await _context.Admins.DeleteOneAsync(a => a.Id == admin.Id);

                response.Success = true;
                response.Message = "Account deleted successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while deleting account");
            }
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

            // email must contain @ once, TLD, and no spaces
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
        /// Helper method to verify email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Boolean result</returns>
        private async Task<bool> UserExistsEmail(string email)
        {
            if (await _context.Admins.Find(x => x.Email.ToLower() == email.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to create password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>
        /// password hash and password salt of type byte array
        /// </returns>
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
        /// Helper method to create JWT token
        /// </summary>
        /// <param name="admin">Admin object</param>
        /// <returns>JWT Token</returns>
        private string CreateToken(Admin admin)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Email),
                new Claim(ClaimTypes.Role, SystemRole.Admin.ToString())
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
