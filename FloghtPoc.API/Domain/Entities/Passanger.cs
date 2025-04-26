namespace FlightPoc.Models
{
    public class Passenger
    {
        public Guid Id { get; }

        private string _uniqueId;

        public string UniqueId
        {
            get => _uniqueId;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Unique Id cannot be null or empty.");
                _uniqueId = value;
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be null or empty.");
                _name = value;
            }
        }

        private List<Baggage> _baggageItems;
        public IReadOnlyList<Baggage> BaggageItems => _baggageItems.AsReadOnly();

        public Passenger(Guid id, string name, string uniqueId)
        {
            Id = id;
            Name = name;
            UniqueId = uniqueId;
            _baggageItems = new List<Baggage>();
        }

        public Passenger(string name, string uniqueId)
        {
            Id = Guid.Empty; 
            Name = name;
            UniqueId = uniqueId;
            _baggageItems = new List<Baggage>();
        }

        public Passenger(string name, string uniqueId, List<Baggage> baggages)
        {
            Id = Guid.Empty;
            Name = name;
            UniqueId = uniqueId;
            _baggageItems = baggages;
        }

        // Method to add baggage
        public void AddBaggage(Baggage baggage)
        {
            if (baggage == null)
                throw new ArgumentNullException(nameof(baggage));

            _baggageItems.Add(baggage);
        }

        // Method to remove baggage
        public bool RemoveBaggage(Baggage baggage)
        {
            if (baggage == null)
                throw new ArgumentNullException(nameof(baggage));

            return _baggageItems.Remove(baggage);
        }
    }
}
