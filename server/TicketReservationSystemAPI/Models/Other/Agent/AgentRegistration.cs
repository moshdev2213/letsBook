// File name: AgentRegistration.cs
// <summary>
// Description: Data transfer model for travel agent registration.
// </summary>
// <author> MulithaBM </author>
// <created>13/10/2023</created>
// <modified>13/10/2023</modified>

using System.ComponentModel.DataAnnotations;

namespace TicketReservationSystemAPI.Models.Other.Agent
{
    public class AgentRegistration
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string ContactNo { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
