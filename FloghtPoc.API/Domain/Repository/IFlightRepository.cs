using FlightPoc.API.Infrastructure.Entities;
using FlightPoc.Models;

namespace FlightPoc.API.Domain.Repository
{
    public interface IFlightRepository
    {
        Task<Flight?> GetById(Guid flightId);
        Task Save(Flight flight);
    }
}
