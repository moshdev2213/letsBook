// File name: AdminTravelerUpdate.cs
// <summary>
// Description: Data transfer model to update traveler account, for admin.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminTravelerUpdate
    {
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Password { get; set; }
    }
}
