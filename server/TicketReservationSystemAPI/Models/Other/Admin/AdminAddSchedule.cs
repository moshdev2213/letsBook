// File name: AdminAddSchedule.cs
// <summary>
// Description: Data transfer model to add schedule, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using System.ComponentModel.DataAnnotations;

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminAddSchedule
    {
        [Required]
        public string TrainId { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string DepartureTime { get; set; }

        [Required]
        public string ArrivalTime { get; set; }

        [Required]
        public string Price { get; set; }
    }
}
