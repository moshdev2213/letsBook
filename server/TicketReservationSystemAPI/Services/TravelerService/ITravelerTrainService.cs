// File name: ITravelerTrainService.cs
// <summary>
// Description: Interface of the service class for traveler's train related operations
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>12/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public interface ITravelerTrainService
    {
        Task<ServiceResponse<List<TravelerGetTrain>>> GetTrains(string departureStation, string arrivalStation, string date);
        Task<ServiceResponse<TravelerGetTrainWithSchedules>> GetTrain(string id);
        Task<ServiceResponse<TravelerGetTrainSchedule>> GetSchedule(string id);
    }
}
