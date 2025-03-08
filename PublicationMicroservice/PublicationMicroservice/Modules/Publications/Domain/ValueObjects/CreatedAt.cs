namespace PublicationMicroservice.Modules.Publications.Domain.ValueObjects
{
    public class CreatedAt
    {
        public DateTime Value { get; }

        private CreatedAt(DateTime value)
        {
            Value = value;
        }

        public static CreatedAt FromDateTime(DateTime value)
        {
            if (value == default)
            {
                throw new ArgumentException("The created at date cannot be empty", nameof(value));
            }

            return new CreatedAt(value);
        }

        public static CreatedAt Now()
        {
            return new CreatedAt(DateTime.UtcNow);
        }

        public override bool Equals(object? obj)
        {
            return obj is CreatedAt other && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("O"); // Formato ISO 8601
        }
    }
}
