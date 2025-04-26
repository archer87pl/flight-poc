using FlightPoc.Models;
using FlightPoc.API.Domain.Services;
using FlightPoc.API.Application.Interfaces;
using FlightPoc.API.Application.Commands;
namespace FlightPoc.API.Application.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInDomainService _checkInDomainService;

        public CheckInService(ICheckInDomainService checkInDomainService)
        {
            _checkInDomainService = checkInDomainService;
        }

        public async Task CheckIn(CheckInPassengerCommand command)
        {
            var passenger = new Passenger(command.PassengerName, command.PassengerUniqueId);
            foreach (var bag in command.BaggageWeights)
            {
                passenger.AddBaggage(new Baggage(bag));
            }

            await _checkInDomainService.CheckInPassenger(command.FlightId, passenger);
        }
    }
}