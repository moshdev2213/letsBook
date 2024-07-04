// File name: AdminGetTrainSchedule.cs
// <summary>
// Description: Data transfer model to get train schedule, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>10/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminGetTrainSchedule
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
    }
}
