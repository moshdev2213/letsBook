// File name: AgentService.cs
// <summary>
// Description: Service class for agent related operations.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models.enums;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Agent;
using TicketReservationSystemAPI.Models;
using MongoDB.Driver;
using AutoMapper;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public class AgentService : IAgentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentService> _logger;

        public AgentService(DataContext context, IMapper mapper, ILogger<AgentService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Agent login
        /// </summary>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with authorization token or null
        /// </returns>
        public async Task<ServiceResponse<string>> Login(AgentLogin data)
        {
            ServiceResponse<string> response = new();

            try
            {
                Agent agent = await _context.Agents
                    .Find(a => a.Email.ToLower() == data.Email.ToLower())
                    .FirstOrDefaultAsync();

                if (agent == null)
                    return CreateErrorResponse(response, "User not found");

                if (!VerifyPasswordHash(data.Password, agent.PasswordHash, agent.PasswordSalt))
                    return CreateErrorResponse(response, "Incorrect password");

                response.Data = CreateToken(agent);
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
        /// Agent registration
        /// </summary>
        /// <param name="data">Registration data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with null
        /// </returns>
        public async Task<ServiceResponse<string>> Register(AgentRegistration data)
        {
            ServiceResponse<string> response = new();

            try
            {
                if (await UserExists(data.Email))
                    return CreateErrorResponse(response, "User already exists");

                CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                Agent newAgent = new()
                {
                    Name = data.Name,
                    Email = data.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    ContactNo = data.ContactNo,
                };

                await _context.Agents.InsertOneAsync(newAgent);

                response.Success = true;
                response.Message = "User created successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while registering");
            }
        }

        /// <summary>
        /// Get agent account
        /// </summary>
        /// <param name="id">Agent ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account or null
        /// </returns>
        public async Task<ServiceResponse<AgentReturn>> GetAccount(string id)
        {
            ServiceResponse<AgentReturn> response = new();

            Guid agentId = new(id);

            try
            {
                Agent agent = await _context.Agents
                    .Find(a => a.Id == agentId)
                    .FirstOrDefaultAsync();

                if (agent == null)
                    return CreateErrorResponse(response, "User not found");

                response.Data = _mapper.Map<AgentReturn>(agent);
                response.Success = true;
                response.Message = "User found";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving the account");
            }
        }

        /// <summary>
        /// Update agent account
        /// </summary>
        /// <param name="id">Agent ID</param>
        /// <param name="data">Update account data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated account or null
        /// </returns>
        public async Task<ServiceResponse<AgentReturn>> UpdateAccount(string id, AgentUpdate data)
        {
            ServiceResponse<AgentReturn> response = new();

            try
            {
                Guid agentId = new(id);

                Agent agent = await _context.Agents
                    .Find(a => a.Id == agentId)
                    .FirstOrDefaultAsync();

                if (agent == null)
                    return CreateErrorResponse(response, "User not found");

                if(!string.IsNullOrEmpty(data.Name)) agent.Name = data.Name;
                if(!string.IsNullOrEmpty(data.ContactNo)) agent.ContactNo = data.ContactNo;

                if (!string.IsNullOrEmpty(data.PreviousPassword) && !string.IsNullOrEmpty(data.Password))
                {
                    if (data.PreviousPassword == data.Password)
                        return CreateErrorResponse(response, "New password cannot be the same as the previous password");

                    if (!VerifyPasswordHash(data.PreviousPassword, agent.PasswordHash, agent.PasswordSalt)) ;

                    CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    agent.PasswordHash = passwordHash;
                    agent.PasswordSalt = passwordSalt;
                }

                await _context.Agents.ReplaceOneAsync(a => a.Id == agentId, agent);

                response.Data = _mapper.Map<AgentReturn>(agent);
                response.Success = true;
                response.Message = "User updated successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while updating the account");
            }
        }

        /// <summary>
        /// Delete agent account
        /// </summary>
        /// <param name="id">Agent ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with deleted account ID or null
        /// </returns>
        public async Task<ServiceResponse<string>> DeleteAccount(string id)
        {
            ServiceResponse<string> response = new();

            try
            {
                Guid agentId = new(id);

                Agent agent = await _context.Agents
                    .Find(a => a.Id == agentId)
                    .FirstOrDefaultAsync();

                if (agent == null)
                    return CreateErrorResponse(response, "User not found");

                await _context.Agents.DeleteOneAsync(a => a.Id == agentId);

                response.Data = agent.Id.ToString();
                response.Success = true;
                response.Message = "User deleted successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while deleting the account");
            }
        }

        /// <summary>
        /// Helper method to verify email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Boolean result</returns>
        public async Task<bool> UserExists(string email)
        {
            if (await _context.Agents.Find(a => a.Email.ToLower() == email.ToLower()).AnyAsync())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper method to create password hash
        /// </summary>
        /// <param name="password">Password</param>
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
        private string CreateToken(Agent agent)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, agent.Id.ToString()),
                new Claim(ClaimTypes.Email, agent.Email),
                new Claim(ClaimTypes.Role, SystemRole.TravelAgent.ToString())
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
