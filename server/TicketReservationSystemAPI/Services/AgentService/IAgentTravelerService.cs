// File name: IAgentTravelerService.cs
// <summary>
// Description: Interface of the service class for travel agent's traveler related operations.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using TicketReservationSystemAPI.Models.Other.Agent;
using TicketReservationSystemAPI.Models.Other;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public interface IAgentTravelerService
    {
        Task<ServiceResponse<string>> CreateAccount(AgentTravelerRegistration data);
        Task<ServiceResponse<AgentGetTravelerWithReservations>> GetAccount(string travelerId);
        Task<ServiceResponse<AgentGetTraveler>> UpdateAccount(string travelerId, AgentTravelerUpdate data);
        Task<ServiceResponse<string>> DeleteAccount(string travelerId);
    }
}
