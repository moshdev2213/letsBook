// File name: IAgentService.cs
// <summary>
// Description: Interface of the service class for travel agent operations.
// </summary>
// <author> MulithaBM </author>
// <created>28/09/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Agent;

namespace TicketReservationSystemAPI.Services.AgentService
{
    public interface IAgentService
    {
        Task<ServiceResponse<string>> Register(AgentRegistration traveler);
        Task<ServiceResponse<string>> Login(AgentLogin traveler);
        Task<ServiceResponse<AgentReturn>> GetAccount(string id);
        Task<ServiceResponse<AgentReturn>> UpdateAccount(string id, AgentUpdate data);
        Task<ServiceResponse<string>> DeleteAccount(string id);
    }
}
