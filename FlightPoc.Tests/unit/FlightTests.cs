namespace FlightPoc.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using FluentAssertions;
    using FlightPoc.Models;
    using FlightPoc.Exceptions;

    public class FlightTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithValidParameters()
        {
            // Arrange
            var id = Guid.NewGuid();
            var flightNumber = "AB123";
            var seatCapacity = 100;
            var maxBaggage = 20.0;

            // Act
            var flight = new Flight(id, flightNumber, seatCapacity, maxBaggage, new List<Passenger>());

            // Assert
            flight.Id.Should().Be(id);
            flight.FlightNumber.Should().Be(flightNumber);
            flight.SeatCapacity.Should().Be(seatCapacity);
            flight.MaxBaggagePerPassenger.Should().Be(maxBaggage);
            flight.CheckedInPassengers.Should().BeEmpty();
        }

        [Fact]
        public void CheckIn_ShouldAddPassenger_WhenValid()
        {
            // Arrange
            var flight = CreateFlightWithCapacity(1);
            var passenger = CreatePassengerWithBaggageWeight(10);

            // Act
            flight.CheckIn(passenger);

            // Assert
            flight.CheckedInPassengers.Should().ContainSingle().Which.Should().Be(passenger);
        }

        [Fact]
        public void CheckIn_ShouldThrow_WhenFlightIsOverbooked()
        {
            // Arrange
            var flight = CreateFlightWithCapacity(1);
            var p1 = CreatePassengerWithBaggageWeight(10);
            var p2 = CreatePassengerWithBaggageWeight(10);
            flight.CheckIn(p1);

            // Act
            Action act = () => flight.CheckIn(p2);

            // Assert
            act.Should().Throw<BusinessRuleException>().WithMessage("Flight is fully booked.");
        }

        [Fact]
        public void CheckIn_ShouldThrow_WhenBaggageExceedsLimit()
        {
            // Arrange
            var flight = CreateFlightWithCapacity(1, maxBaggage: 15);
            var passenger = CreatePassengerWithBaggageWeight(16);

            // Act
            Action act = () => flight.CheckIn(passenger);

            // Assert
            act.Should().Throw<BusinessRuleException>().WithMessage("Baggage weight exceeds allowed limit.");
        }

        [Fact]
        public void CheckIn_ShouldThrow_WhenPassengerIsNull()
        {
            // Arrange
            var flight = CreateFlightWithCapacity(1);

            // Act
            Action act = () => flight.CheckIn(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        // Helpers
        private static Flight CreateFlightWithCapacity(int seatCapacity, double maxBaggage = 20.0) =>
            new(Guid.NewGuid(), "XY789", seatCapacity, maxBaggage, new List<Passenger>());

        private static Passenger CreatePassengerWithBaggageWeight(double weightKg) =>
            new Passenger("test", "123", new List<Baggage>() { new Baggage(weightKg) });
    }
}