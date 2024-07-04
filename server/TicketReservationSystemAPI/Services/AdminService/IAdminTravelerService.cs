// File name: IAdminTravelerService.cs
// <summary>
// Description: Interface of the service class for admin traveler related operations.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminTravelerService
    {
        Task<ServiceResponse<string>> CreateAccount(AdminTravelerRegistration data);
        Task<ServiceResponse<AdminGetTravelerWithReservations>> GetAccount(string userId);
        Task<ServiceResponse<List<AdminGetTraveler>>> GetAccounts(bool? status);
        Task<ServiceResponse<AdminGetTraveler>> UpdateAccount(string userId, AdminTravelerUpdate data);
        Task<ServiceResponse<AdminGetTraveler>> UpdateActiveStatus(string userId);
        Task<ServiceResponse<string>> DeleteAccount(string userId);
    }
}
