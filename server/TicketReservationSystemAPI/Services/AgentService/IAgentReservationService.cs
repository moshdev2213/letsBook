// File name: IAgentReservationService.cs
// <summary>
// Description: Interface of the service class for agent's reservation related operations
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using TicketReservationSystemAPI.Models.Other.Agent;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public interface IAgentReservationService
    {
        Task<ServiceResponse<string>> CreateReservation(AgentCreateReservation data);
        Task<ServiceResponse<AgentGetReservation>> GetReservation(string id);
        Task<ServiceResponse<AgentGetReservation>> UpdateReservation(string id, AgentUpdateReservation data);
        Task<ServiceResponse<string>> CancelReservation(string id);
    }
}
