// File name: Reservation.cs
// <summary>
// Description: Model class for reservations.
// </summary>
// <author> MulithaBM </author>
// <created>11/09/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public string TravelerId { get; set; }
        public Guid TrainId { get; set; }
        public Guid ScheduleId { get; set; }
        public int Seats { get; set; }
        public DateOnly BookingDate { get; set; }
        public DateOnly ReservationDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public bool IsCancelled { get; set; } = false;
    }
}
