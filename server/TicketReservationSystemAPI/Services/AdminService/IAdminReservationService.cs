// File name: IAdminReservationService.cs
// <summary>
// Description: Interface of the service class for admin reservation related operations.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminReservationService
    {
        Task<ServiceResponse<string>> CreateReservation(AdminCreateReservation data);
        Task<ServiceResponse<AdminGetReservation>> GetReservation(string id);
        Task<ServiceResponse<AdminGetReservation>> UpdateReservation(string id, AdminUpdateReservation data);
        Task<ServiceResponse<string>> CancelReservation(string id);
    }
}
