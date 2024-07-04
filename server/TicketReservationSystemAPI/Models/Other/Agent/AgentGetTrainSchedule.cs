// File name: AgentGetTrainSchedule.cs
// <summary>
// Description: Data transfer model to get train schedule, for travel agent.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentGetTrainSchedule
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
    }
}
