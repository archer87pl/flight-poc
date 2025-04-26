using FlightPoc.API.Application.Exceptions;
using FlightPoc.API.Domain.Repository;
using FlightPoc.API.Infrastructure.Context;
using FlightPoc.API.Infrastructure.Entities;
using FlightPoc.API.Infrastructure.Mappers;
using FlightPoc.Exceptions;
using FlightPoc.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPoc.API.Infrastructure.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FlightRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Flight?> GetById(Guid flightId)
        {
            var entity = await _dbContext.Flights.AsNoTracking().Include(x => x.Passangers).FirstOrDefaultAsync(f => f.Id == flightId);
            
            if (entity == null)
            {
                return null;
            }

            return FlightMapper.ToDomainModel(entity);
        }

        public async Task Save(Flight flight)
        {
            try
            {
                var existingFlight = await _dbContext.Flights.Include(x => x.Passangers)
                 .FirstOrDefaultAsync(f => f.Id == flight.Id);

                if (existingFlight != null)
                {
                    var newPassagner = flight.CheckedInPassengers.Where(x => x.Id == Guid.Empty).FirstOrDefault();

                    if (existingFlight.Passangers.Any(x => x.PassangerUniqueId == newPassagner.UniqueId))
                    {
                        throw new BusinessRuleException("Passanger has been already checked in.");
                    }

                    var newPassangerEntity = new PassangerEntity()
                    {
                        Name = newPassagner.Name,
                        PassangerUniqueId = newPassagner.UniqueId,
                        Baggages = newPassagner.BaggageItems.Select(x => new BaggageEntity() { WeightKg = x.WeightKg }).ToList(),
                    };

                    if (newPassagner != null)
                    {
                        existingFlight.Passangers.Add(newPassangerEntity);
                        existingFlight.TotalPassangers++;
                    }

                    existingFlight.RowVersion = IncrementRowVersion(existingFlight.RowVersion);

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {

                foreach (var entry in _dbContext.ChangeTracker.Entries().ToList())
                {
                    entry.State = EntityState.Detached;
                }

                throw new DbRetryNeededException();
            }
        
        }

        private byte[] IncrementRowVersion(byte[] currentRowVersion)
        {
            byte[] newRowVersion = new byte[currentRowVersion.Length];
            for (int i = 0; i < currentRowVersion.Length; i++)
            {
                newRowVersion[i] = (byte)(currentRowVersion[i] + 1); // Just an example of modifying it
            }
            return newRowVersion;
        }
    }
}
