// File name: DbSettings.cs
// <summary>
// Description: MongoDB database settings
// </summary>
// <author> MulithaBM </author>
// <created>22/09/2023</created>
// <modified>11/10/2023</modified>

namespace TicketReservationSystemAPI.Data
{
    public class DbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
