// File name: AgentTrainService.cs
// <summary>
// Description: Service class for agent's train related operations
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
    public class AgentTrainService : IAgentTrainService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentTrainService> _logger;

        public AgentTrainService(
            DataContext context, 
            IMapper mapper, 
            ILogger<AgentTrainService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get trains based on departure station, arrival station and date
        /// </summary>
        /// <param name="departureStation">Departure station</param>
        /// <param name="arrivalStation">Arrival station</param>
        /// <param name="date">Date</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with list of trains or null
        /// </returns>
        public async Task<ServiceResponse<List<AgentGetTrain>>> GetTrains(
            string departureStation, 
            string arrivalStation, 
            string date)
        {
            ServiceResponse<List<AgentGetTrain>> response = new();

            try
            {
                var filterBuilder = Builders<Train>.Filter;
                var filter = filterBuilder.Empty;

                if (!string.IsNullOrEmpty(departureStation))
                    filter &= filterBuilder.Eq(t => t.DepartureStation, departureStation.ToLower());

                if (!string.IsNullOrEmpty(arrivalStation))
                    filter &= filterBuilder.Eq(t => t.ArrivalStation, arrivalStation.ToLower());

                if (!string.IsNullOrEmpty(date))
                {
                    DateOnly travelDate = DateOnly.Parse(date);
                    filter &= filterBuilder.AnyEq(t => t.AvailableDates, travelDate);
                }

                List<Train> trains = await _context.Trains
                    .Find(filter)
                    .ToListAsync();

                response.Data = _mapper.Map<List<AgentGetTrain>>(trains); ;
                response.Success = true;
                response.Message = "Trains retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return (CreateErrorResponse(response, "Error occurred while retrieving trains"));
            }
        }

        /// <summary>
        /// Get single train with schedules
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with train and schedules or null
        /// </returns>
        public async Task<ServiceResponse<AgentGetTrainWithSchedules>> GetTrain(string id)
        {
            ServiceResponse<AgentGetTrainWithSchedules> response = new();

            try
            {
                Guid trainId = new(id);

                Train train = await _context.Trains
                    .Find(t => t.Id == trainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                AgentGetTrainWithSchedules trainWithSchedules = _mapper.Map<AgentGetTrainWithSchedules>(train);

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                List<TrainSchedule> schedules = await _context.TrainSchedules
                    .Find(s => s.TrainId == trainId && s.Date >= currentDate && s.DepartureTime >= currentTime)
                    .SortBy(s => s.Date)
                    .ThenBy(s => s.DepartureTime)
                    .ToListAsync();

                trainWithSchedules.Schedules = _mapper.Map<List<AgentGetTrainSchedule>>(schedules);

                response.Data = trainWithSchedules;
                response.Success = true;
                response.Message = "Train retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving train");
            }
        }

        /// <summary>
        /// Get train schedule
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with schedule or null
        /// </returns>
        public async Task<ServiceResponse<AgentGetTrainSchedule>> GetSchedule(string id)
        {
            ServiceResponse<AgentGetTrainSchedule> response = new();

            try
            {
                Guid scheduleId = new(id);

                TrainSchedule schedule = await _context.TrainSchedules
                    .Find(s => s.Id == scheduleId)
                    .FirstOrDefaultAsync();

                if (schedule == null)
                    return CreateErrorResponse(response, "Schedule not found");

                response.Data = _mapper.Map<AgentGetTrainSchedule>(schedule); ;
                response.Success = true;
                response.Message = "Schedule retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error occurred while retrieving schedule");
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
