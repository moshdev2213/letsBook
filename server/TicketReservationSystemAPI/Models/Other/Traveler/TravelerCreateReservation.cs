// File name: TravelerCreateReservation.cs
// <summary>
// Description: Data transfer model to create reservation, for traveler.
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerCreateReservation
    {
        public string ScheduleId { get; set; }
        public int Seats { get; set; }
    }
}
