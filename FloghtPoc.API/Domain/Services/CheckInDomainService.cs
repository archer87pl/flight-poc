using FlightPoc.Exceptions;
using FlightPoc.Models;
using FlightPoc.API.Domain.Repository;
using FlightPoc.API.Application.Exceptions;

namespace FlightPoc.API.Domain.Services
{
    public class CheckInDomainService : ICheckInDomainService
    {
        private readonly IFlightRepository _flightRepository;
        public CheckInDomainService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        public async Task CheckInPassenger(Guid flightId, Passenger passenger)
        {

            const int maxRetries = 15; 
            int retryCount = 0;
            TimeSpan delay = TimeSpan.FromMilliseconds(1); 

            while (retryCount < maxRetries)
            {
                try
                {
                    var flight = await _flightRepository.GetById(flightId);
                     
                    if (flight == null)
                    {
                        throw new KeyNotFoundException($"Flight with ID {flightId} was not found.");
                    }

                    var totalBaggageWeight = passenger.BaggageItems.Sum(b => b.WeightKg);
                    if (totalBaggageWeight > flight.MaxBaggagePerPassenger)
                    {
                        throw new BusinessRuleException("Total baggage weight exceeds limit.");
                    }

                    flight.CheckIn(passenger);

                    await _flightRepository.Save(flight);

                    return;
                }
                catch (DbRetryNeededException ex)
                {
                    retryCount++;

                    if (retryCount >= maxRetries)
                    {
                        throw new BusinessRuleException("Concurrency conflict occurred too many times. Please try again later.");
                    }

                    // Wait before retrying
                    await Task.Delay(delay);

                    // Exponential backoff: increase the delay after each retry
                    delay = delay.Add(delay); // Double the delay for the next retry
                }
            }
        }
    }
}
