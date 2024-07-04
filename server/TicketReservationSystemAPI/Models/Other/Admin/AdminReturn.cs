// File name: AdminReturn.cs
// <summary>
// Description: Data transfer model to get admin account.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Models.Other.Admin
{
    public class AdminReturn
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
    }
}
