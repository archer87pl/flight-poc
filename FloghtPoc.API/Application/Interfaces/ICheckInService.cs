
using FlightPoc.API.Application.Commands;

namespace FlightPoc.API.Application.Interfaces
{
    public interface ICheckInService
    {
        Task CheckIn(CheckInPassengerCommand command);
    }
}