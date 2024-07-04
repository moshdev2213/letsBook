// File name: AgentGetTrain.cs
// <summary>
// Description: Data transfer model to get train, for travel agent.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentGetTrain
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public String Type { get; set; }
        public int Seats { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
    }
}
