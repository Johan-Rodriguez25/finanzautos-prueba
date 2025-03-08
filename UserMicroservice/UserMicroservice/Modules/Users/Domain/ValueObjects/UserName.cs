namespace UserMicroservice.Modules.Users.Domain.ValueObjects
{
    public class UserName
    {
        public string Value { get; }

        private UserName(string value)
        {
            Value = value;
        }

        public static UserName FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= 3)
            {
                throw new UserNameException("Username must be at least 3 characters long");
            }
            return new UserName(value);
        }
    }

    public class UserNameException : Exception
    {
        public UserNameException(string message) : base(message) { }
    }
}
