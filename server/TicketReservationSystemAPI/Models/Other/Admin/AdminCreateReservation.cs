// File name: AdminCreateReservation.cs
// <summary>
// Description: Data transfer model to create reservation, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>13/10/2023</modified>

using System.ComponentModel.DataAnnotations;

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminCreateReservation
    {
        [Required]
        public string NIC { get; set; }

        [Required]
        public string ScheduleId { get; set; }

        [Required]
        public int Seats { get; set; }
    }
}
