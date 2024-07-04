// File name: AgentGetTravelerWithReservations.cs
// <summary>
// Description: Data transfer model to get traveler with reservations, for travel agent.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentGetTravelerWithReservations
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public List<AgentGetReservation> Reservations { get; set; } = new();
    }
}
