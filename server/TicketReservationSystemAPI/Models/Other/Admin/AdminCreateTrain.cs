// File name: AdminCreateTrain.cs
// <summary>
// Description: Data transfer model to create train, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using System.ComponentModel.DataAnnotations;

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminCreateTrain
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int Seats { get; set; }

        [Required]
        public string DepartureStation { get; set; }

        [Required]
        public string ArrivalStation { get; set; }
    }
}
