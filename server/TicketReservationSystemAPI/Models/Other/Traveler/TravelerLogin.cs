// File name: TravelerLogin.cs
// <summary>
// Description: Data transfer model for traveler login.
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
