// File name: AgentController.cs
// <summary>
// Description: Controller class for travel agent related operations
// </summary>
// <author> MulithaBM </author>
// <created>23/09/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Agent;
using TicketReservationSystemAPI.Services.AgentService;

namespace TicketReservationSystemAPI.Controllers
{
    /// <permission cref="ClaimTypes.Role">
    /// Travel agent has access to the following endpoints
    /// </permission>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Agent")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly IAgentTravelerService _agentTravelerService;
        private readonly IAgentTrainService _agentTrainService;
        private readonly IAgentReservationService _agentReservationService;

        public AgentController(
            IAgentService authService,
            IAgentTravelerService agentTravelerService,
            IAgentTrainService agentTrainService,
            IAgentReservationService agentReservationService) 
        {
            _agentService = authService;
            _agentTravelerService = agentTravelerService;
            _agentTrainService = agentTrainService;
            _agentReservationService = agentReservationService;
        }

        /// <summary>
        /// Travel agent login endpoint
        /// </summary>
        /// <Permission>
        /// Anyone can access this endpoint
        /// </Permission>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with authorization token
        /// </returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] AgentLogin data)
        {
            ServiceResponse<string> response = await _agentService.Login(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Travel agent registration endpoint
        /// </summary>
        /// <Permission>
        /// Anyone can access this endpoint
        /// </Permission>
        /// <param name="data">Registration data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with null data
        /// </returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register([FromBody] AgentRegistration data)
        {
            ServiceResponse<string> response = await _agentService.Register(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get account endpoint
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<AgentReturn>>> GetAccount()
        {
            string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<AgentReturn> response = await _agentService.GetAccount(agentId);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update account endpoint
        /// </summary>
        /// <param name="data">Update data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated account
        /// </returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<AgentReturn>>> UpdateAccount([FromBody] AgentUpdate data)
        {
            string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<AgentReturn> response = await _agentService.UpdateAccount(agentId, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Delete account endpoint
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with account ID
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteAccount()
        {
            string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<string> response = await _agentService.DeleteAccount(agentId);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Traveler endpoints

        /// <summary>
        /// Create traveler account endpoint
        /// </summary>
        /// <param name="data">Create traveler data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with traveler NIC
        /// </returns>
        [HttpPost("traveler")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateTravelerAccount([FromBody] AgentTravelerRegistration data)
        {
            string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<string> response = await _agentTravelerService.CreateAccount(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get traveler account endpoint
        /// </summary>
        /// <param name="id">Traveler NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of traveler account
        /// </returns>
        [HttpGet("traveler/{id}")]
        public async Task<ActionResult<ServiceResponse<AgentGetTravelerWithReservations>>> GetTravelerAccount(string id)
        {
            ServiceResponse<AgentGetTravelerWithReservations> response = await _agentTravelerService.GetAccount(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update traveler account endpoint
        /// </summary>
        /// <param name="id">Traveler NIC</param>
        /// <param name="data">Update traveler data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated traveler account
        /// </returns>
        [HttpPut("traveler/{id}")]
        public async Task<ActionResult<ServiceResponse<AgentGetTraveler>>> UpdateTravelerAccount(
            string id, 
            [FromBody] AgentTravelerUpdate data)
        {
            ServiceResponse<AgentGetTraveler> response = await _agentTravelerService.UpdateAccount(id, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Delete traveler account endpoint
        /// </summary>
        /// <param name="id">Traveler NIC</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with traveler NIC
        /// </returns>
        [HttpDelete("traveler/{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteTravelerAccount(string id)
        {
            ServiceResponse<string> response = await _agentTravelerService.DeleteAccount(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Train endpoints

        /// <summary>
        /// Get trains endpoint
        /// </summary>
        /// <param name="departureStation">Departure station</param>
        /// <param name="arrivalStation">Arrival station</param>
        /// <param name="date">Date</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of trains
        /// </returns>
        [HttpGet("trains")]
        public async Task<ActionResult<ServiceResponse<List<AgentGetTrain>>>> GetTrains(
            [FromQuery] string? departureStation = null,
            [FromQuery] string? arrivalStation = null,
            [FromQuery] string? date = null)
        {
            ServiceResponse<List<AgentGetTrain>> response = await _agentTrainService.GetTrains(departureStation, arrivalStation, date);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get train endpoint
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with train
        /// </returns>
        [HttpGet("train/{id}")]
        public async Task<ActionResult<ServiceResponse<AgentGetTrainWithSchedules>>> GetTrain(string id)
        {
            ServiceResponse<AgentGetTrainWithSchedules> response = await _agentTrainService.GetTrain(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get train schedule endpoint
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with train schedule
        /// </returns>
        [HttpGet("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<AgentGetTrainSchedule>>> GetTrainSchedule(string id)
        {
            ServiceResponse<AgentGetTrainSchedule> response = await _agentTrainService.GetSchedule(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Reservation endpoints

        /// <summary>
        /// Create reservation for traveler endpoint
        /// </summary>
        /// <param name="data">Reservation creation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation ID
        /// </returns>
        [HttpPost("reservation")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateReservation([FromBody] AgentCreateReservation data)
        {

            ServiceResponse<string> response = await _agentReservationService.CreateReservation(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get reservation endpoint
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation
        /// </returns>
        [HttpGet("reservation/{id}")]
        public async Task<ActionResult<ServiceResponse<AgentGetReservation>>> GetReservation(string id)
        {
            ServiceResponse<AgentGetReservation> response = await _agentReservationService.GetReservation(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update reservation endpoint
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <param name="data">Update reservation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated reservation
        /// </returns>
        [HttpPut("reservation/{id}")]
        public async Task<ActionResult<ServiceResponse<AgentGetReservation>>> UpdateReservation(
            string id,
            [FromBody] AgentUpdateReservation data)
        {
            ServiceResponse<AgentGetReservation> response = await _agentReservationService.UpdateReservation(id, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Cancel reservation endpoint
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation ID
        /// </returns>
        [HttpDelete("reservation/{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> CancelReservation(string id)
        {
            ServiceResponse<string> response = await _agentReservationService.CancelReservation(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
