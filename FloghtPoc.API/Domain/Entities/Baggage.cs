namespace FlightPoc.Models
{
    public class Baggage
    {
        public double WeightKg { get; private set; }

        public Baggage(double weightKg)
        {
            if (weightKg < 0)
                throw new ArgumentException("Weight cannot be negative.", nameof(weightKg));

            WeightKg = weightKg;
        }

        public static Baggage Create(double weightKg)
        {
            return new Baggage(weightKg);
        }

        public bool IsOverweight(double limitKg) => WeightKg > limitKg;
    }
}
