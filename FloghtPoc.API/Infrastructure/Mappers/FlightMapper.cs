using FlightPoc.API.Infrastructure.Entities;
using FlightPoc.Models;
namespace FlightPoc.API.Infrastructure.Mappers
{
    public static class FlightMapper
    {
        public static Flight? ToDomainModel(FlightEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            var passangers = entity.Passangers.Select(x => new Passenger(x.Id, x.Name, x.PassangerUniqueId));
            return new Flight(entity.Id, entity.FlightNumber, entity.SeatCapacity, entity.MaxBaggagePerPassanger, passangers);
        }

        public static FlightEntity? ToDatabaseModel(Flight domainModel)
        {
            if (domainModel == null) { return null; }

            return new FlightEntity
            {
                Id = domainModel.Id,
                FlightNumber = domainModel.FlightNumber,
                MaxBaggagePerPassanger = domainModel.MaxBaggagePerPassenger,
                SeatCapacity = domainModel.SeatCapacity
            };
        }
    }
}
