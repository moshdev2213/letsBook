// File name: IAdminService.cs
// <summary>
// Description: Interface of the service class for admin related operations.
// </summary>
// <author> MulithaBM </author>
// <created>09/10/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminService
    {
        Task<ServiceResponse<string>> Register(AdminRegistration data);
        Task<ServiceResponse<string>> Login(AdminLogin data);
        Task<ServiceResponse<AdminReturn>> GetAccount(string userId);
        Task<ServiceResponse<AdminReturn>> UpdateAccount(string userId, AdminUpdate data);
        Task<ServiceResponse<string>> DeleteAccount(string userId);
    }
}
