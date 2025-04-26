using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlightPoc.API.Infrastructure.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace FlightPoc.Tests.integration
{
    public class CheckInPassengerTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScope _scope;

        public CheckInPassengerTests(WebApplicationFactory<Program> factory)
        {
            var scopedFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // You can override services here if needed
                });
            }); var scopeFactory = scopedFactory.Services.GetRequiredService<IServiceScopeFactory>();
            _scope = scopeFactory.CreateScope();
            _client = factory.CreateClient(); _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        }


        public async Task InitializeAsync()
        {
            // Clear the database before each test
            _dbContext.RemoveRange(_dbContext.BaggageItems); // Replace with your actual DbSet names
            _dbContext.RemoveRange(_dbContext.Passangers);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CheckInPassenger_ReturnsOk_WhenRequestIsValid()
        {
            // Arrange
            var request = new
            {
                FlightId = "11111111-1111-1111-1111-111111111111",
                PassengerName = "John Doe",
                PassangerId = 870714322,
                PassengerUniqueId = "SSN01",
                BaggageWeights = new List<double> { 15.5, 20.0 }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/checkin", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("successfully checked in");
        }

        [Fact]
        public async Task CheckInPassenger_ReturnsBadRequest_WhenRequestIsValid()
        {
            // Arrange
            var request = new
            {
                FlightId = "11111111-1111-1111-1111-111111111111",
                PassengerName = "John Doe",
                PassangerId = 870714322,
                PassengerUniqueId = "XSSN03",
                BaggageWeights = new List<double> { 15.5, 20.0 }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/checkin", content);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("successfully checked in");

            var responseSecond = await _client.PostAsync("/api/checkin", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseSecond.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task CheckInPassenger_ReturnsBadRequest_WhenBaggageIsOverweight()
        {
            // Arrange
            var request = new
            {
                FlightId = "11111111-1111-1111-1111-111111111111",
                PassengerName = "John Doe",
                PassangerId = 870714322,
                PassengerUniqueId = "SSN02",
                BaggageWeights = new List<double> { 445.5, 20.0 }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/checkin", content);


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("Total baggage weight exceeds limit");
        }

        [Fact]
        public async Task CheckInPassenger_ShouldAcceptAll_WhenPassengerUniqueIdsAreUnique()
        {
            // Arrange
            var flightId = "11111111-1111-1111-1111-111111111111";

            var tasks = Enumerable.Range(0, 3).Select(i =>
            {
                var request = new
                {
                    FlightId = flightId,
                    PassengerName = $"John Doe {i}",
                    PassangerId = 870714322 + i,
                    PassengerUniqueId = $"SSN{i:D4}", // SSN0000, SSN0001, ..., SSN0099
                    BaggageWeights = new List<double> { 15.5, 20.0 }
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                return _client.PostAsync("/api/checkin", content);
            }).ToArray();

            // Act
            var responses = await Task.WhenAll(tasks);

            // Assert
            foreach (var response in responses)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task CheckInPassenger_ShouldOneFail_WhenPassengerUniqueIdsAreUnique()
        {
            // Arrange
            var flightId = "11111111-1111-1111-1111-111111111111";

            var tasks = Enumerable.Range(0, 110).Select(i =>
            {
                var request = new
                {
                    FlightId = flightId,
                    PassengerName = $"John Doe {i}",
                    PassangerId = 870714322 + i,
                    PassengerUniqueId = $"SSN{i:D4}", // SSN0000, SSN0001, ..., SSN0099
                    BaggageWeights = new List<double> { 15.5, 20.0 }
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                return _client.PostAsync("/api/checkin", content);
            }).ToArray();

            // Act
            var responses = await Task.WhenAll(tasks);

            // Assert
            responses.Any(x => !x.IsSuccessStatusCode).Should().BeTrue();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
