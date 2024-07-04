// File name: AdminRegistration.cs
// <summary>
// Description: Data transfer model to register, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using System.ComponentModel.DataAnnotations;

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminRegistration
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
