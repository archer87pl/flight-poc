namespace FlightPoc.API.Application.Commands
{
    public class CheckInPassengerCommand
    {
        public Guid FlightId { get; set; }
        public string PassengerName { get; set; }
        public string PassengerUniqueId { get; set; }
        public List<float> BaggageWeights { get; set; } = new();
    }
}