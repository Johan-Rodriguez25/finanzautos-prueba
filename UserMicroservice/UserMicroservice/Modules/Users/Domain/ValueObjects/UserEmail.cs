namespace UserMicroservice.Modules.Users.Domain.ValueObjects
{
    public class UserEmail
    {
        public string Value { get; }

        private UserEmail(string value)
        {
            Value = value;
        }

        public static UserEmail FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.Contains("@"))
            {
                throw new UserEmailException("UserEmail must be a valid email address.");
            }

            return new UserEmail(value);
        }
    }

    public class UserEmailException : Exception
    {
        public UserEmailException(string message) : base(message) { }
    }
}
