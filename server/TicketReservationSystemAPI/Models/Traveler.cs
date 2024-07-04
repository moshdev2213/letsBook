// File name: Traveler.cs
// <summary>
// Description: Model class for travelers.
// </summary>
// <author> MulithaBM </author>
// <created>11/09/2023</created>
// <modified>11/10/2023</modified>

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketReservationSystemAPI.Models
{
    public class Traveler
    {
        [BsonId]
        public string NIC { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public IList<Guid> ReservationIDs { get; set; } = new List<Guid>();
    }
}
