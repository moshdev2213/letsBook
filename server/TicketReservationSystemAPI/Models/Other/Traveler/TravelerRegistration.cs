// File name: TravelerRegistration.cs
// <summary>
// Description: Data transfer model for traveler registration.
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerRegistration
    {
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Password { get; set; }
    }
}
