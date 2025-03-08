namespace PublicationMicroservice.Modules.Publications.Domain.ValueObjects
{
    public class PublicationId
    {
        public string Value { get; }

        private PublicationId(string value)
        {
            Value = value;
        }

        public static PublicationId FromString(string value)
        {
            if (!Guid.TryParse(value, out _))
            {
                throw new PublicationIdException("PublicationId must be a valid UUID.");
            }

            return new PublicationId(value);
        }

        public static PublicationId New()
        {
            return new PublicationId(Guid.NewGuid().ToString());
        }
    }

    public class PublicationIdException : Exception
    {
        public PublicationIdException(string message) : base(message) { }
    }
}
