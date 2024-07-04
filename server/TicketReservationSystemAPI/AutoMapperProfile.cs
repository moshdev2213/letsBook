// File name: AutoMapperProfile.cs
// <summary>
// Description: Profile declaration for AutoMapper.
// </summary>
// <author> MulithaBM </author>
// <created>11/10/2023</created>
// <modified>11/10/2023</modified>

using AutoMapper;
using TicketReservationSystemAPI.Models;
using TicketReservationSystemAPI.Models.Other.Admin;
using TicketReservationSystemAPI.Models.Other.Agent;
using TicketReservationSystemAPI.Models.Other.Traveler;

namespace TicketReservationSystemAPI
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Back-Office
            CreateMap<Admin, AdminReturn>();
            CreateMap<Train, AdminGetTrain>();
            CreateMap<Train, AdminGetTrainWithSchedules>();
            CreateMap<TrainSchedule, AdminGetTrainSchedule>();
            CreateMap<Traveler, AdminGetTraveler>();
            CreateMap<Traveler, AdminGetTravelerWithReservations>();
            CreateMap<Reservation, AdminGetReservation>();

            // Travel agent
            CreateMap<Agent, AgentReturn>();
            CreateMap<Train, AgentGetTrain>();
            CreateMap<Train, AgentGetTrainWithSchedules>();
            CreateMap<TrainSchedule, AgentGetTrainSchedule>();
            CreateMap<Traveler, AgentGetTraveler>();
            CreateMap<Traveler, AgentGetTravelerWithReservations>();
            CreateMap<Reservation, AgentGetReservation>();

            // Traveler
            CreateMap<Traveler, TravelerReturn>();
            CreateMap<Train, TravelerGetTrain>();
            CreateMap<Train, TravelerGetTrainWithSchedules>();
            CreateMap<TrainSchedule, TravelerGetTrainSchedule>();
            CreateMap<Reservation, TravelerGetReservation>();
        }
    }
}
