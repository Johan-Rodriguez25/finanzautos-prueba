namespace UserMicroservice.Modules.Users.Domain.ValueObjects
{
    public class PasswordHash
    {
        public string Value { get; }

        private PasswordHash(string value)
        {
            Value = value;
        }

        public static PasswordHash FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new PasswordHashException("Password hash cannot be empty");
            }

            return new PasswordHash(value);
        }

        public override bool Equals(object? obj)
        {
            return obj is PasswordHash other && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class PasswordHashException : Exception
    {
        public PasswordHashException(string message) : base(message)
        {
        }
    }
}
