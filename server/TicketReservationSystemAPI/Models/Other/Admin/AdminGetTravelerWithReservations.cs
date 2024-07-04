// File name: AdminGetTravelerWithReservations.cs
// <summary>
// Description: Data transfer model to get traveler with reservations, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminGetTravelerWithReservations
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public List<AdminGetReservation> Reservations { get; set; } = new();
        public bool IsActive { get; set; }
    }
}
