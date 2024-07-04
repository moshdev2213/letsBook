// File name: TravelerController.cs
// <summary>
// Description: Controller class for traveler related operations
// </summary>
// <author> MulithaBM </author>
// <created>12/09/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Traveler;
using TicketReservationSystemAPI.Services.TravelerService;

namespace TicketReservationSystemAPI.Controllers
{
    /// <permission cref="ClaimTypes.Role">
    /// Traveler has access to the following endpoints
    /// </permission>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Traveler")]
    public class TravelerController : ControllerBase
    {
        private readonly ITravelerService _travelerService;
        private readonly ITravelerTrainService _travelerTrainService;
        private readonly ITravelerReservationService _travelerReservationService;

        public TravelerController(
            ITravelerService travelerService, 
            ITravelerTrainService travelerTrainService, 
            ITravelerReservationService travelerReservationService)
        {
            _travelerService = travelerService;
            _travelerTrainService = travelerTrainService;
            _travelerReservationService = travelerReservationService;
        }

        /// <summary>
        /// Traveler login endpoint
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
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] TravelerLogin data)
        {
            ServiceResponse<string> response = await _travelerService.Login(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Traveler registration endpoint
        /// </summary>
        /// <Permission>
        /// Anyone can access this endpoint
        /// </Permission>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with null
        /// </returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register([FromBody] TravelerRegistration data)
        {
            ServiceResponse<string> response = await _travelerService.Register(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get traveler account endpoint
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with traveler account
        /// </returns>
        [HttpGet("account")]
        public async Task<ActionResult<ServiceResponse<TravelerReturn>>> GetAccount()
        {
            string travelerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<TravelerReturn> response = await _travelerService.GetAccount(travelerId);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update account endpoint
        /// </summary>
        /// <param name="data">Update traveler data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated traveler account
        /// </returns>
        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponse<TravelerReturn>>> UpdateAccount([FromBody] TravelerUpdate data)
        {
            string travelerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<TravelerReturn> response = await _travelerService.UpdateAccount(travelerId, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Deactivate account endpoint
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with active status
        /// </returns>
        [HttpPut("deactivate")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeactivateAccount()
        {
            string travelerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<bool> response = await _travelerService.DeactivateAccount(travelerId);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Train endpoints

        /// <summary>
        /// Get trains 
        /// </summary>
        /// <param name="departureStation">Departure station</param>
        /// <param name="arrivalStation">Arrival station</param>
        /// <param name="date">Date</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of trains
        /// </returns>
        [HttpGet("trains")]
        public async Task<ActionResult<ServiceResponse<List<TravelerGetTrain>>>> GetTrains(
            [FromQuery] string? departureStation, 
            [FromQuery] string? arrivalStation, 
            [FromQuery] string? date)
        {
            ServiceResponse<List<TravelerGetTrain>> response = await _travelerTrainService.GetTrains(departureStation, arrivalStation, date);

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
        public async Task<ActionResult<ServiceResponse<TravelerGetTrainWithSchedules>>> GetTrain(string id)
        {
            ServiceResponse<TravelerGetTrainWithSchedules> response = await _travelerTrainService.GetTrain(id);

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
        public async Task<ActionResult<ServiceResponse<TravelerGetTrainSchedule>>> GetSchedule(string id)
        {
            ServiceResponse<TravelerGetTrainSchedule> response = await _travelerTrainService.GetSchedule(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Reservation endpoints

        /// <summary>
        /// Create reservation endpoint
        /// </summary>
        /// <param name="data">Create reservation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with reservation ID
        /// </returns>
        [HttpPost("reservation")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateReservation([FromBody] TravelerCreateReservation data)
        {
            string nic = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<string> response = await _travelerReservationService.CreateReservation(nic, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get reservations endpoint
        /// </summary>
        /// <param name="past">Reservation types (current reservations=> past = false, Past reservations => past = true</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of reservations
        /// </returns>
        [HttpGet("reservations")]
        public async Task<ActionResult<ServiceResponse<List<TravelerGetReservation>>>> GetReservations([FromQuery] bool past = false)
        {
            string nic = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<List<TravelerGetReservation>> response = await _travelerReservationService.GetReservations(nic, past);

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
        public async Task<ActionResult<ServiceResponse<TravelerGetReservation>>> GetReservation(string id)
        {
            ServiceResponse<TravelerGetReservation> response = await _travelerReservationService.GetReservation(id);

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
        public async Task<ActionResult<ServiceResponse<TravelerGetReservation>>> UpdateReservation(
            string id,
            [FromBody] TravelerUpdateReservation data)
        {
            ServiceResponse<TravelerGetReservation> response = await _travelerReservationService.UpdateReservation(id, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Cancel reservation endpoint
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with cancelled reservation ID
        /// </returns>
        [HttpPut("reservation/{id}/cancel")]
        public async Task<ActionResult<ServiceResponse<string>>> CancelReservation(string id)
        {
            ServiceResponse<string> response = await _travelerReservationService.CancelReservation(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
