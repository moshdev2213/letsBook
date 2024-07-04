// File name: AdminUpdate.cs
// <summary>
// Description: Data transfer model to update admin account.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminUpdate
    {
        public string? Name { get; set; }
        public string? ContactNo { get; set; }
        public string? PreviousPassword { get; set; }
        public string? Password { get; set; }
    }
}
