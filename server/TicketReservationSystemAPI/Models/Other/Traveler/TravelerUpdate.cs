// File name: TravelerUpdate.cs
// <summary>
// Description: Data transfer model to update traveler account.
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>13/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Traveler
{
    public class TravelerUpdate
    {
        public string? Name { get; set; }
        public string? ContactNo { get; set; }
        public string? PreviousPassword { get; set; }
        public string? Password { get; set; }
    }
}
