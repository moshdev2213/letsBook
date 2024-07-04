// File name: AgentGetTraveler.cs
// <summary>
// Description: Data transfer model to get traveler, for travel agent.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentGetTraveler
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
    }
}
