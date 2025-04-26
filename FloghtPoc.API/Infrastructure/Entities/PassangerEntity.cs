namespace FlightPoc.API.Infrastructure.Entities
{
    public class PassangerEntity
    {
        public Guid Id { get; set; }
        public string PassangerUniqueId { get; set; }
        public string Name { get; set; }
        public ICollection<BaggageEntity> Baggages { get; set; }

        public Guid FlightId { get; set; }
        public FlightEntity Flight
        {
            get; set;
        }
    }
}
