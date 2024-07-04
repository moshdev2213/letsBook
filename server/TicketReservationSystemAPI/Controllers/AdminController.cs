// File name: AdminController.cs
// <summary>
// Description: Controller class for back-office related operations
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;
using TicketReservationSystemAPI.Services.AdminService;

namespace TicketReservationSystemAPI.Controllers
{
    /// <permission cref="ClaimTypes.Role">
    /// Admin (Back-Office) has access to the following endpoints
    /// </permission>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminTravelerService _adminTravelerService;
        private readonly IAdminTrainService _adminTrainService;
        private readonly IAdminReservationService _adminReservationService;

        public AdminController(
            IAdminService adminService, 
            IAdminTravelerService adminTravelerService, 
            IAdminTrainService adminTrainService,
            IAdminReservationService adminReservationService)
        {
            _adminService = adminService;
            _adminTravelerService = adminTravelerService;
            _adminTrainService = adminTrainService;
            _adminReservationService = adminReservationService;
        }

        /// <summary>
        /// Back-Office login endpoint
        /// </summary>
        /// <Permission>
        /// Anyone can access this endpoint
        /// </Permission>
        /// <param name="data">Login data</param>
        /// <returns>
        /// <see cref="ServiceResponse{}"/> with authorization token
        /// </returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] AdminLogin data)
        {
            ServiceResponse<string> response = await _adminService.Login(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Back-Office registration endpoint
        /// </summary>
        /// <Permission>
        /// Anyone can access this endpoint
        /// </Permission>
        /// <param name="data">Registration data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with null
        /// </returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register([FromBody] AdminRegistration data)
        {
            ServiceResponse<string> response = await _adminService.Register(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get Back-Office account endpoint
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> With back-office account
        /// </returns>
        [HttpGet("account")]
        public async Task<ActionResult<ServiceResponse<AdminReturn>>> GetAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<AdminReturn> response = await _adminService.GetAccount(userId);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update Back-Office account endpoint
        /// </summary>
        /// <param name="data">Update data></param>
        /// <returns>
        /// <see cref="ServiceResponse{}"/> with Updated account
        /// </returns>
        [HttpPut("account")]
        public async Task<ActionResult<ServiceResponse<AdminReturn>>> UpdateAccount([FromBody] AdminUpdate data)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<AdminReturn> response = await _adminService.UpdateAccount(userId, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Delete Back-Office account endpoint
        /// </summary>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with NIC of the deleted account
        /// </returns>
        [HttpDelete("account")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteAccount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ServiceResponse<string> response = await _adminService.DeleteAccount(userId);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Traveler endpoints

        /// <summary>
        /// Create traveler account endpoint
        /// </summary>
        /// <param name="data">Traveler creation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with NIC of the created account
        /// </returns>
        [HttpPost("traveler/account")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateTravelAccount([FromBody] AdminTravelerRegistration data)
        {
            ServiceResponse<string> response = await _adminTravelerService.CreateAccount(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get traveler accounts endpoint
        /// </summary>
        /// <param name="status">Active status of the account</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with a list of traveler accounts
        /// </returns>
        [HttpGet("traveler/accounts")]
        public async Task<ActionResult<ServiceResponse<List<AdminGetTraveler>>>> GetTravelerAccounts([FromQuery] bool? status)
        {
            ServiceResponse<List<AdminGetTraveler>> response = await _adminTravelerService.GetAccounts(status);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get traveler account endpoint
        /// </summary>
        /// <param name="nic">NIC of the traveler account</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with traveler account
        /// </returns>
        [HttpGet("traveler/account/{nic}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTravelerWithReservations>>> GetTravelerAccount(string nic)
        {
            ServiceResponse<AdminGetTravelerWithReservations> response = await _adminTravelerService.GetAccount(nic);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update traveler account endpoint
        /// </summary>
        /// <param name="nic">NIC of the traveler account</param>
        /// <param name="data">Update data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated traveler account
        /// </returns>
        [HttpPut("traveler/account/{nic}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTraveler>>> UpdateTravelerAccount(string nic, [FromBody] AdminTravelerUpdate data)
        {
            ServiceResponse<AdminGetTraveler> response = await _adminTravelerService.UpdateAccount(nic, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update traveler account active status endpoint
        /// </summary>
        /// <param name="nic">NIC of the traveler account</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated traveler account
        /// </returns>
        [HttpPut("traveler/account/{nic}/status")]
        public async Task<ActionResult<ServiceResponse<AdminGetTraveler>>> UpdateTravelerActiveStatus(string nic)
        {
            ServiceResponse<AdminGetTraveler> response = await _adminTravelerService.UpdateActiveStatus(nic);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Delete traveler account endpoint
        /// </summary>
        /// <param name="nic">NIC of the traveler account</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with NIC of the deleted account
        /// </returns>
        [HttpDelete("traveler/account/{nic}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteTravelerAccount(string nic)
        {
            ServiceResponse<string> response = await _adminTravelerService.DeleteAccount(nic);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Train endpoints

        /// <summary>
        /// Create train endpoint
        /// </summary>
        /// <param name="data">Create train data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with train ID of the created train
        /// </returns>
        [HttpPost("train")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateTrain([FromBody] AdminCreateTrain data)
        {
            ServiceResponse<string> response = await _adminTrainService.CreateTrain(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get trains endpoint
        /// </summary>
        /// <param name="activeStatus">Active status of the train (default: true)</param>
        /// <param name="publishStatus">Publish status of the train (default: true)</param>
        /// <param name="departureStation">Departure station of the train (default: null)</param>
        /// <param name="arrivalStation">Arrival station of the train (default: null)</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with a list of trains
        /// </returns>
        [HttpGet("trains")]
        public async Task<ActionResult<ServiceResponse<List<AdminGetTrain>>>> GetTrains(
            [FromQuery] bool activeStatus = true,
            [FromQuery] bool publishStatus = true,
            [FromQuery] string? departureStation = null,
            [FromQuery] string? arrivalStation = null,
            [FromQuery] string? date = null)
        {
            ServiceResponse<List<AdminGetTrain>> response = await _adminTrainService.GetTrains(activeStatus, publishStatus, departureStation, arrivalStation, date);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get train endpoint
        /// </summary>
        /// <param name="id">train ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with train
        /// </returns>
        [HttpGet("train/{id}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTrainWithSchedules>>> GetTrain(string id)
        {
            ServiceResponse<AdminGetTrainWithSchedules> response = await _adminTrainService.GetTrain(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        //[HttpPut("train/{id}")]
        //public async Task<ActionResult<ServiceResponse<string>>> UpdateTrain(
        //    string id,
        //    [FromBody] AdminUpdateTrain data)
        //{
        //    ServiceResponse<string> response = await _adminTrainService.UpdateTrain(data);

        //    if (!response.Success)
        //    {
        //        return BadRequest(response);
        //    }

        //    return Ok(response);
        //}

        /// <summary>
        /// Update train active status endpoint
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <param name="status">Active status</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated active status of the train
        /// </returns>
        [HttpPut("train/{id}/active")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateTrainActiveStatus(
            string id,
            [FromBody] bool status)
        {
            ServiceResponse<bool> response = await _adminTrainService.UpdateActiveStatus(id, status);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update train publish status endpoint
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <param name="status">Active status</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated publish status of the train
        /// </returns>
        [HttpPut("train/{id}/publish")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateTrainPublishStatus(
            string id,
            [FromBody] bool status)
        {
            ServiceResponse<bool> response = await _adminTrainService.UpdatePublishStatus(id, status);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Cancel train endpoint
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <param name="data">Cancel train data (date)</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with ID of the cancelled train
        /// </returns>
        [HttpPut("train/{id}/cancel")]
        public async Task<ActionResult<ServiceResponse<string>>> CancelTrain(
            string id,
            [FromBody] AdminCancelTrain data)
        {
            ServiceResponse<string> response = await _adminTrainService.CancelTrain(id, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Train schedule endpoints

        /// <summary>
        /// Create train schedule endpoint
        /// </summary>
        /// <param name="data">Add schedule data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with ID of the created schedule
        /// </returns>
        [HttpPost("train/schedule")]
        public async Task<ActionResult<ServiceResponse<string>>> AddSchedule([FromBody] AdminAddSchedule data)
        {
            ServiceResponse<string> response = await _adminTrainService.AddSchedule(data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Get schedule endpoint
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with schedule
        /// </returns>
        [HttpGet("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTrainSchedule>>> GetSchedule(string id)
        {
            ServiceResponse<AdminGetTrainSchedule> response = await _adminTrainService.GetSchedule(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update schedule endpoint
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <param name="data">Schedule update data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated schedule
        /// </returns>
        [HttpPut("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<AdminGetTrainSchedule>>> UpdateSchedule(
            string id,
            [FromBody] AdminUpdateSchedule data)
        {
            ServiceResponse<AdminGetTrainSchedule> response = await _adminTrainService.UpdateSchedule(id, data);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Delete schedule endpoint
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with deleted schedule ID
        /// </returns>
        [HttpDelete("train/schedule/{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteSchedule(string id)
        {
            ServiceResponse<string> response = await _adminTrainService.DeleteSchedule(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Reservation endpoints

        /// <summary>
        /// Create reservation for traveler endpoint
        /// </summary>
        /// <param name="data">Create reservation data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with ID of the created reservation
        /// </returns>
        [HttpPost("reservation")]
        public async Task<ActionResult<ServiceResponse<string>>> CreateReservation([FromBody] AdminCreateReservation data)
        {
            ServiceResponse<string> response = await _adminReservationService.CreateReservation(data);

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
        public async Task<ActionResult<ServiceResponse<List<AdminGetReservation>>>> GetReservation(string id)
        {
            ServiceResponse<AdminGetReservation> response = await _adminReservationService.GetReservation(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Update reservation endpoint
        /// </summary>
        /// <param name="id">Reservation ID</param>
        /// <param name="data">Reservation update data</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with updated reservation
        /// </returns>
        [HttpPut("reservation/{id}")]
        public async Task<ActionResult<ServiceResponse<AdminGetReservation>>> UpdateReservation(
            string id,
            [FromBody] AdminUpdateReservation data)
        {
            ServiceResponse<AdminGetReservation> response = await _adminReservationService.UpdateReservation(id, data);

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
            ServiceResponse<string> response = await _adminReservationService.CancelReservation(id);

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}