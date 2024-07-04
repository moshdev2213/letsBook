// File name: TravelerReturn.cs
// <summary>
// Description: Data transfer model to get traveler account.
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerReturn
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
        public List<TravelerGetReservation> Reservations { get; set; } = new();
    }
}
