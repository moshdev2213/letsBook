// File name: AgentUpdate.cs
// <summary>
// Description: Data transfer model to update travel agent account.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentUpdate
    {
        public string? Name { get; set; }
        public string? ContactNo { get; set; }
        public string? PreviousPassword { get; set; }
        public string? Password { get; set; }
    }
}
