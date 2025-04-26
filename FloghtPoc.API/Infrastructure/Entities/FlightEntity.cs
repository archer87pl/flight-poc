using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace FlightPoc.API.Infrastructure.Entities
{
    public class FlightEntity
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; }
        public int SeatCapacity { get; set; }
        public double MaxBaggagePerPassanger { get; set; }

        public ICollection<PassangerEntity> Passangers { get; set; }
        public int TotalPassangers { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
