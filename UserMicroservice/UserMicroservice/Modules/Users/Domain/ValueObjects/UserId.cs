namespace UserMicroservice.Modules.Users.Domain.ValueObjects
{
    public class UserId
    {
        public string Value { get; }

        private UserId(string value)
        {
            Value = value;
        }

        public static UserId FromString(string value)
        {
            if (!Guid.TryParse(value, out _))
            {
                throw new UserIdException("UserId must be a valid UUID.");
            }

            return new UserId(value);
        }

        public static UserId New()
        {
            return new UserId(Guid.NewGuid().ToString());
        }
    }

    public class UserIdException : Exception
    {
        public UserIdException(string message) : base(message) { }
    }
}
