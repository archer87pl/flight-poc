using FlightPoc.API.Infrastructure.Entities;
using FlightPoc.Exceptions;

namespace FlightPoc.Models
{
    public class Flight
    {
        private readonly List<Passenger> _checkedInPassengers = new();

        public Guid Id { get; }
        public string FlightNumber { get; }
        public int SeatCapacity { get; }
        public double MaxBaggagePerPassenger { get; }

        public IReadOnlyCollection<Passenger> CheckedInPassengers => _checkedInPassengers.AsReadOnly();

        public bool IsOverbooked => _checkedInPassengers.Count() >= SeatCapacity;

        public Flight(Guid id, string flightNumber, int seatCapacity, double maxBaggagePerPassenger, IEnumerable<Passenger> passangers)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                throw new ArgumentException("Flight number cannot be empty.", nameof(flightNumber));
            if (seatCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(seatCapacity), "Seat capacity must be positive.");
            if (maxBaggagePerPassenger <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxBaggagePerPassenger), "Max baggage must be positive.");
            if (passangers == null)
            {
                throw new ArgumentNullException(nameof(passangers));
            }


            Id = id;
            FlightNumber = flightNumber;
            SeatCapacity = seatCapacity;
            MaxBaggagePerPassenger = maxBaggagePerPassenger;
            _checkedInPassengers = passangers.ToList();
        }

        public void CheckIn(Passenger passenger)
        {
            if (passenger == null)
                throw new ArgumentNullException(nameof(passenger));

            if (IsOverbooked)
                throw new BusinessRuleException("Flight is fully booked.");

            double totalBaggage = passenger.BaggageItems.Sum(b => b.WeightKg);
            if (totalBaggage > MaxBaggagePerPassenger)
                throw new BusinessRuleException("Baggage weight exceeds allowed limit.");

            _checkedInPassengers.Add(passenger);
        }
    }
}
