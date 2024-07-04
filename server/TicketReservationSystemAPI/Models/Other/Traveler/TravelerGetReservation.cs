// File name: TravelerGetReservation.cs
// <summary>
// Description: Data transfer model to get reservation, for traveler.
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerGetReservation
    {
        public Guid Id { get; set; }
        public int Seats { get; set; }
        public DateOnly BookingDate { get; set; }
        public DateOnly ReservationDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
    }
}
