// File name: IAdminTrainService.cs
// <summary>
// Description: Interface of the service class for admin train related operations.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using TicketReservationSystemAPI.Models.Other;
using TicketReservationSystemAPI.Models.Other.Admin;

namespace TicketReservationSystemAPI.Services.AdminService
{
    public interface IAdminTrainService
    {
        Task<ServiceResponse<string>> CreateTrain(AdminCreateTrain data);
        Task<ServiceResponse<List<AdminGetTrain>>> GetTrains(bool activeStatus, bool publishStatus, string? departureStation, string? arrivalStation, string? date);
        Task<ServiceResponse<AdminGetTrainWithSchedules>> GetTrain(string id);
        Task<ServiceResponse<bool>> UpdateActiveStatus(string id, bool status);
        Task<ServiceResponse<bool>> UpdatePublishStatus(string id, bool status);    
        Task<ServiceResponse<string>> CancelTrain(string id, AdminCancelTrain data);
        Task<ServiceResponse<string>> AddSchedule(AdminAddSchedule data);
        Task<ServiceResponse<AdminGetTrainSchedule>> GetSchedule(string id);
        Task<ServiceResponse<AdminGetTrainSchedule>> UpdateSchedule(string id, AdminUpdateSchedule data);
        Task<ServiceResponse<string>> DeleteSchedule(string id);
    }
}
