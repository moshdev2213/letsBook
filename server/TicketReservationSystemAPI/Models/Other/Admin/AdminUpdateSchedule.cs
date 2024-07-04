// File name: AdminUpdateSchedule.cs
// <summary>
// Description: Data transfer model to update train schedule, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminUpdateSchedule
    {
        public string? DepartureTime { get; set; }
        public string? ArrivalTime { get; set; }
        public int? AvailableSeats { get; set; }
        public string? Price { get; set; }
    }
}
