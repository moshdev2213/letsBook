// File name: ITravelerService.cs
// <summary>
// Description: Interface of the service class for traveler related operations
// </summary>
// <author> MulithaBM </author>
// <created>23/09/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI.Services.TravelerService
{
    public interface ITravelerService
    {
        Task<ServiceResponse<string>> Register(TravelerRegistration traveler);
        Task<ServiceResponse<string>> Login(TravelerLogin traveler);
        Task<ServiceResponse<TravelerReturn>> GetAccount(string userId);
        Task<ServiceResponse<TravelerReturn>> UpdateAccount(string userId, TravelerUpdate traveler);
        Task<ServiceResponse<bool>> DeactivateAccount(string userId);
    }
}
