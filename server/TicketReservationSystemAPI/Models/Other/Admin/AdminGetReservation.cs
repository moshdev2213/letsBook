// File name: AdminGetReservation.cs
// <summary>
// Description: Data transfer model to get reservation, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminGetReservation
    {
        public string Id { get; set; }
        public DateOnly BookingDate { get; set; }
        public DateOnly ReservationDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public int Seats { get; set; }
        public bool IsCancelled { get; set; }
    }
}
