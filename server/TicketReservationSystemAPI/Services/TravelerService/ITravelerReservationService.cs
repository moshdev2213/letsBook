// File name: ITravelerReservationService.cs
// <summary>
// Description: Interface of the service class for traveler's reservation related operations
// </summary>
// <author> MulithaBM </author>
// <created>12/10/2023</created>
// <modified>12/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public interface ITravelerReservationService
    {
        Task<ServiceResponse<string>> CreateReservation(string nic, TravelerCreateReservation data);
        Task<ServiceResponse<List<TravelerGetReservation>>> GetReservations(string nic, bool past);
        Task<ServiceResponse<TravelerGetReservation>> GetReservation(string id);
        Task<ServiceResponse<TravelerGetReservation>> UpdateReservation(string id, TravelerUpdateReservation data);
        Task<ServiceResponse<string>> CancelReservation(string id);
    }
}
