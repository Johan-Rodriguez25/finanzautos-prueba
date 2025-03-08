namespace PublicationMicroservice.Modules.Publications.Domain.ValueObjects
{
    public class UpdatedAt
    {
        public DateTime Value { get; }

        private UpdatedAt(DateTime value)
        {
            Value = value;
        }

        public static UpdatedAt FromDateTime(DateTime value)
        {
            if (value == default)
            {
                throw new ArgumentException("The updated at date cannot be empty", nameof(value));
            }

            return new UpdatedAt(value);
        }

        public static UpdatedAt Now()
        {
            return new UpdatedAt(DateTime.UtcNow);
        }

        public override bool Equals(object? obj)
        {
            return obj is UpdatedAt other && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString("O"); // ISO 8601 format
        }
    }
}
