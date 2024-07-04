// File name: IAgentTrainService.cs
// <summary>
// Description: Interface of the service class travel agent's train related operations.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using TicketReservationSystemAPI.Models.Other.Agent;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public interface IAgentTrainService
    {
        Task<ServiceResponse<List<AgentGetTrain>>> GetTrains(string departureStation, string arrivalStation, string date);
        Task<ServiceResponse<AgentGetTrainWithSchedules>> GetTrain(string id);
        Task<ServiceResponse<AgentGetTrainSchedule>> GetSchedule(string id);
    }
}
