using System;
using FlightPoc.Models;

namespace FlightPoc.Testss
{
    public class BaggageTests
    {
        [Fact]
        public void Constructor_WithValidWeight_SetsWeight()
        {
            // Arrange
            double weight = 20.0;

            // Act
            var baggage = new Baggage(weight);

            // Assert
            Assert.Equal(weight, baggage.WeightKg);
        }

        [Fact]
        public void Constructor_WithNegativeWeight_ThrowsArgumentException()
        {
            // Arrange
            double negativeWeight = -5.0;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Baggage(negativeWeight));
            Assert.Equal("Weight cannot be negative. (Parameter 'weightKg')", ex.Message);
        }

        [Fact]
        public void Create_WithValidWeight_ReturnsBaggageInstance()
        {
            // Arrange
            double weight = 15.0;

            // Act
            var baggage = Baggage.Create(weight);

            // Assert
            Assert.NotNull(baggage);
            Assert.Equal(weight, baggage.WeightKg);
        }

        [Theory]
        [InlineData(10.0, 15.0, false)]
        [InlineData(20.0, 15.0, true)]
        [InlineData(15.0, 15.0, false)]
        public void IsOverweight_ReturnsCorrectResult(double baggageWeight, double limit, bool expected)
        {
            // Arrange
            var baggage = new Baggage(baggageWeight);

            // Act
            var result = baggage.IsOverweight(limit);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}