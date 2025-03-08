namespace PublicationMicroservice.Modules.Publications.Domain.ValueObjects
{
    public class PublicationTitle
    {
        public string Value { get; }

        private PublicationTitle(string value)
        {
            Value = value;
        }

        public static PublicationTitle FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= 3)
            {
                throw new PublicationTitleException("Title must be at least 3 characters long.");
            }

            return new PublicationTitle(value);
        }
    }

    public class PublicationTitleException : Exception
    {
        public PublicationTitleException(string message) : base(message) { }
    }
}
