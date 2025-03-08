namespace PublicationMicroservice.Modules.Publications.Domain.ValueObjects
{
    public class PublicationContent
    {
        public string Value { get; }

        private PublicationContent(string value)
        {
            Value = value;
        }

        public static PublicationContent FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= 3)
            {
                throw new PublicationContentException("Publication content must be at least 3 characters long.");
            }

            return new PublicationContent(value);
        }
    }

    public class PublicationContentException : Exception
    {
        public PublicationContentException(string message) : base(message) { }
    }
}