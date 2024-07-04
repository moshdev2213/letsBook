// File name: AgentTravelerUpdate.cs
// <summary>
// Description: Data transfer model to update traveler, for travel agent.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentTravelerUpdate
    {
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Password { get; set; }
    }
}
