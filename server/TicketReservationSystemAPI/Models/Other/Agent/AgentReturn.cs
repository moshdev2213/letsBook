// File name: AgentReturn.cs
// <summary>
// Description: Data transfer model to get travel agent account.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentReturn
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
    }
}
