// File name: TrainSchedule.cs
// <summary>
// Description: Model class for train schedules.
// </summary>
// <author> MulithaBM </author>
// <created>11/09/2023</created>
// <modified>11/10/2023</modified>

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class TrainSchedule
    {
        public Guid Id { get; set; }
        public Guid TrainId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
        public IList<Guid> ReservationIDs { get; set; } = new List<Guid>();
    }
}
