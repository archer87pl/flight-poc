namespace FlightPoc.API.DTOs
{
    public class CheckInRequestDto
    {
        public Guid FlightId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerUniqueId { get; set; }
        public List<float> BaggageWeights { get; set; } = new();
    }
}
