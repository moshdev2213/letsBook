// File name: Agent.cs
// <summary>
// Description: Model class for travel agents.
// </summary>
// <author> MulithaBM </author>
// <created>22/09/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models
{
    public class Agent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
