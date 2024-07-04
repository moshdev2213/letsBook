// File name: AgentCreateReservation.cs
// <summary>
// Description: Data transfer model to create reservation, for travel agent.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentCreateReservation
    {
        public string NIC { get; set; }
        public string ScheduleId { get; set; }
        public int Seats { get; set; }
    }
}
