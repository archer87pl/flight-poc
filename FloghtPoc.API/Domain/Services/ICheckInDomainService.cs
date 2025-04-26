using FlightPoc.Models;

namespace FlightPoc.API.Domain.Services
{
    public interface ICheckInDomainService
    {
        Task CheckInPassenger(Guid flightId, Passenger passenger);
    }
}