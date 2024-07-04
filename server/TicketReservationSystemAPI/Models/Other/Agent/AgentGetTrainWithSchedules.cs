// File name: AgentGetTrainWithSchedules.cs
// <summary>
// Description: Data transfer model to get train with schedules, for travel agent.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentGetTrainWithSchedules
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public List<AgentGetTrainSchedule> Schedules { get; set; } = new();
    }
}
