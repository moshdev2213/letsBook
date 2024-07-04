// File name: TravelerTrainService.cs
// <summary>
// Description: Service class for traveler's train related operations
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>12/10/2023</modified>

using AutoMapper;
using MongoDB.Driver;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public class TravelerTrainService : ITravelerTrainService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TravelerTrainService> _logger;

        public TravelerTrainService(
            DataContext context, 
            IMapper mapper, 
            ILogger<TravelerTrainService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get train by id
        /// </summary>
        /// <param name="id">Train ID</param>
        /// <returns>
        /// <see cref="TravelerGetTrainWithSchedules"/> with train or null
        /// </returns>
        public async Task<ServiceResponse<TravelerGetTrainWithSchedules>> GetTrain(string id)
        {
            ServiceResponse<TravelerGetTrainWithSchedules> response = new();

            try
            {
                Guid trainId = new(id);

                Train train = await _context.Trains
                    .Find(t => t.Id == trainId)
                    .FirstOrDefaultAsync();

                if (train == null)
                    return CreateErrorResponse(response, "Train not found");

                TravelerGetTrainWithSchedules trainWithSchedules = _mapper.Map<TravelerGetTrainWithSchedules>(train);

                DateTime current = DateTime.Now;
                DateOnly currentDate = DateOnly.FromDateTime(current);
                TimeOnly currentTime = TimeOnly.FromDateTime(current);

                List<TrainSchedule> schedules = await _context.TrainSchedules
                    .Find(s => s.TrainId == trainId && s.Date >= currentDate && s.DepartureTime >= currentTime)
                    .SortBy(s => s.Date)
                    .ThenBy(s => s.DepartureTime)
                    .ToListAsync();

                trainWithSchedules.Schedules = _mapper.Map<List<TravelerGetTrainSchedule>>(schedules);

                response.Data = trainWithSchedules;
                response.Success = true;
                response.Message = "Train retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error retrieving train");
            }
        }

        /// <summary>
        /// Get trains
        /// </summary>
        /// <param name="departureStation">Departure station</param>
        /// <param name="arrivalStation">Arrival station</param>
        /// <param name="date">Date</param>
        /// <returns>
        /// <see cref="List{TravelerGetTrain}"/> with list of trains or null
        /// </returns>
        public async Task<ServiceResponse<List<TravelerGetTrain>>> GetTrains(string departureStation, string arrivalStation, string date)
        {
            ServiceResponse<List<TravelerGetTrain>> response = new();

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

                response.Data = _mapper.Map<List<TravelerGetTrain>>(trains); ;
                response.Success = true;
                response.Message = "Trains retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return (CreateErrorResponse(response, "Error retrieving trains"));
            }
        }

        /// <summary>
        /// Get schedule by id
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>
        /// <see cref="TravelerGetTrainSchedule"/> with schedule or null
        /// </returns>
        public async Task<ServiceResponse<TravelerGetTrainSchedule>> GetSchedule(string id)
        {
            ServiceResponse<TravelerGetTrainSchedule> response = new();

            try
            {
                Guid scheduleId = new(id);

                TrainSchedule schedule = await _context.TrainSchedules
                    .Find(s => s.Id == scheduleId)
                    .FirstOrDefaultAsync();

                if (schedule == null)
                    return CreateErrorResponse(response, "Schedule not found");

                response.Data = _mapper.Map<TravelerGetTrainSchedule>(schedule); ;
                response.Success = true;
                response.Message = "Schedule retrieved successfully";

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return CreateErrorResponse(response, "Error retrieving schedule");
            }
        }

        /// <summary>
        /// Helper method to create error response
        /// </summary>
        /// <typeparam name="T">Response.Data type</typeparam>
        /// <param name="response">Response</param>
        /// <param name="message">Error message</param>
        /// <returns>
        /// <see cref="ServiceResponse{T}"/> with error response
        /// </returns>
        private static ServiceResponse<T> CreateErrorResponse<T>(ServiceResponse<T> response, string message)
        {
            response.Success = false;
            response.Message = message;

            return response;
        }
    }
}
